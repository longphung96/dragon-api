using System.Linq.Dynamic.Core;
using AutoMapper;
using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DragonAPI.Common;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using DragonAPI.Models.DTOs;
using DragonAPI.Repositories;
using StackExchange.Redis;
using DragonAPI.Extensions;
using DragonAPI.Data;
using DragonAPI.IntegrationEvent.Events;
using Nethereum.Web3;
// using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Util;
using System.Numerics;
using RedLockNet;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.Extensions;
using Volo.Abp.Linq;



namespace DragonAPI.Services
{
    public interface IAdminService
    {
        Task<ApiResultDTO<bool>> VerifyWithDraw(ETHWithdrawVerifyRequest param);
        Task ProcessTransferItem(AdminTransferItemEto param);
        Task<ApiResultDTO<OffsetPagniationData<WithDrawVerificationDTO>>> GetList(WithDrawVerificationFilterDto request);
    }

    public class AdminService : BaseService<AdminService>, IAdminService
    {
        private readonly DragonMongoDbContext mongoDbContext;
        private IMasterRepository _masterRepository;
        private IItemMasterRepository _itemMasterRepository;
        private readonly IDatabase redisDatabase;
        private readonly IMongoCollection<MasterDAO> masterRepo;

        private readonly IMongoCollection<CurrencyTransactionDAO> currencyTransactionRepo;
        private readonly IMongoCollection<WithDrawVerificationDAO> withDrawVerificationRepo;
        private readonly IConfiguration configuration;
        private readonly IDistributedLockFactory redlockFactory;
        private readonly AsyncQueryableExecuter asyncQueryableExecuter;
        public AdminService(
            IConnectionMultiplexer redisConn,
            ICacheRepository CacheRepository,
            ILogger<AdminService> logger,
            IMongoClient client,
            ConfigLoader gameCfgLoader,
            IMapper mapper,
            DragonMongoDbContext mongoDbContext,
            ConfigLoader cfgLoader,
            ICapPublisher capBus,
            IOptions<RedisSettings> redisSettingsOption,
            IDatabase redisDatabase,
            IMasterRepository _masterRepository,
            IItemMasterRepository _itemMasterRepository,
            IdentityService identityService,
            IDistributedLockFactory redlockFactory,
            AsyncQueryableExecuter asyncQueryableExecuter,
            IConfiguration configuration)
            : base(logger, client, cfgLoader, mapper, capBus, CacheRepository, identityService)
        {
            this.redisDatabase = redisDatabase;
            this.masterRepo = mongoDbContext.GetCollection<MasterDAO>("masters");
            this.withDrawVerificationRepo = mongoDbContext.GetCollection<WithDrawVerificationDAO>("withdraw_verification");
            this.currencyTransactionRepo = mongoDbContext.GetCollection<CurrencyTransactionDAO>("item_master");
            this._masterRepository = _masterRepository;
            this._itemMasterRepository = _itemMasterRepository;
            this.redlockFactory = redlockFactory;
            this.configuration = configuration;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
        }
        public async Task<ApiResultDTO<OffsetPagniationData<WithDrawVerificationDTO>>> GetList(WithDrawVerificationFilterDto request)
        {
            var response = new ApiResultDTO<OffsetPagniationData<WithDrawVerificationDTO>>();
            var queryable = request.ApplyFilterTo(withDrawVerificationRepo.AsQueryable(new AggregateOptions { AllowDiskUse = true }));
            var count = await asyncQueryableExecuter.LongCountAsync(queryable);
            var items = await asyncQueryableExecuter.ToListAsync(queryable);

            logger.LogDebug($"query explain {queryable}");
            var data = new OffsetPagniationData<WithDrawVerificationDTO>(
               mapper.Map<IReadOnlyList<WithDrawVerificationDAO>, IReadOnlyList<WithDrawVerificationDTO>>(items),
               count, request.Page, request.Size);
            response.Data = data;
            return response;
        }
        public async Task<ApiResultDTO<bool>> VerifyWithDraw(ETHWithdrawVerifyRequest param)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = false;
            var master = await masterRepo.Find(m => m.MainUserId == identityService.CurrentUserId).FirstOrDefaultAsync();
            var transactionVerify = await withDrawVerificationRepo.Find(wd => wd.Id == param.Id && wd.Status == StatusVerify.Processing).FirstOrDefaultAsync();
            if (transactionVerify == null) return result;
            var currencyTransactionLog = await currencyTransactionRepo.Find(ct => ct.Id == transactionVerify.Id && ct.Status == Status.Processing).FirstOrDefaultAsync();
            if (currencyTransactionLog == null) return result;
            transactionVerify.Status = (StatusVerify)param.Status;
            transactionVerify.Description = param.Description;
            transactionVerify.ReviewerId = master.MainUserId;
            await withDrawVerificationRepo.ReplaceOneAsync(wd => wd.Id == transactionVerify.Id, transactionVerify);
            currencyTransactionLog.Status = Status.Processing;
            await currencyTransactionRepo.ReplaceOneAsync(ct => ct.Id == currencyTransactionLog.Id, currencyTransactionLog);
            await eventBus.PublishAsync(EventNameConst.PaymentItemEvent, new AdminTransferItemEto
            {
                ItemId = currencyTransactionLog.ItemId,
                Amount = currencyTransactionLog.Amount,
                ReceiveWalletId = currencyTransactionLog.WalletId,
                WithdrawVerifyId = transactionVerify.Id,
            });
            result.Data = true;
            return result;
        }
        public async Task ProcessTransferItem(AdminTransferItemEto param)
        {
            using (var redlock = await redlockFactory.CreateLockAsync($"withdraw_token_by_user{param.ReceiveWalletId}", TimeSpan.FromSeconds(5)))
            {
                if (redlock.IsAcquired)
                {
                    var check = false;
                    var transactionVerify = await withDrawVerificationRepo.Find(wd => wd.Id == param.WithdrawVerifyId).FirstOrDefaultAsync();
                    var currencyTransactionLog = await currencyTransactionRepo.Find(ct => ct.Id == transactionVerify.Id).FirstOrDefaultAsync();
                    if (transactionVerify.Status != StatusVerify.Processing && currencyTransactionLog.Status == Status.Processing)
                        using (var session = this.client.StartSession())
                        {
                            session.StartTransaction();
                            try
                            {
                                var userItemExchange = await _itemMasterRepository.GetByItemId(param.ItemId);
                                if (userItemExchange != null)
                                {
                                    var validItem = await _itemMasterRepository.TakeItem(session, userItemExchange, param.Amount, $"Withdraw ETH", logger);
                                    if (validItem != false)
                                    {
                                        check = true;
                                    }
                                }
                                if (check == true)
                                {
                                    var account = new Nethereum.Web3.Accounts.Account(configuration["Web3Settings:Privatekey"]);
                                    var web3 = new Web3(account, configuration["Web3Settings:PublicRpcUrl"]);
                                    var TokenContract = configuration["Web3Settings:TokenContract"];
                                    web3.TransactionManager.UseLegacyAsDefault = true;
                                    var withdrawFunction = new WithdrawFunction();
                                    var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
                                    var transfer = new TransferFunction()
                                    {
                                        To = param.ReceiveWalletId,
                                        TokenAmount = Nethereum.Web3.Web3.Convert.ToWei((BigInteger)param.Amount),
                                    };
                                    account.NonceService = new InMemoryNonceService(account.Address, web3.Client);

                                    transfer.Nonce = await account.NonceService.GetNextNonceAsync();
                                    var estimate = await transferHandler.EstimateGasAsync(configuration["Web3Settings:TokenContract"], transfer);
                                    transfer.Gas = (BigInteger)(Math.Round(Convert.ToDouble(estimate.Value.ToString()) * 1.2));

                                    transfer.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei);
                                    var signedTransaction1 = await transferHandler.SendRequestAndWaitForReceiptAsync(TokenContract, transfer);
                                    await session.CommitTransactionAsync();
                                    transactionVerify.Status = StatusVerify.Accepted;
                                    await withDrawVerificationRepo.ReplaceOneAsync(w => w.Id == transactionVerify.Id, transactionVerify);
                                    currencyTransactionLog.Status = Status.Success;
                                    await currencyTransactionRepo.ReplaceOneAsync(w => w.Id == currencyTransactionLog.Id, currencyTransactionLog);
                                }
                                else
                                {
                                    throw new BusinessException(ErrorCodeEnum.UnexpectedErr, $"{((Item)param.ItemId).ToString()} does not enough");
                                }

                            }
                            catch (System.Exception ex)
                            {
                                logger.LogError(ex.StackTrace);
                                await session.AbortTransactionAsync();
                                check = false;
                            }
                            if (check == false)
                            {
                                transactionVerify.Status = StatusVerify.Failed;
                                await withDrawVerificationRepo.ReplaceOneAsync(w => w.Id == transactionVerify.Id, transactionVerify);
                                currencyTransactionLog.Status = Status.Failed;
                                await currencyTransactionRepo.ReplaceOneAsync(w => w.Id == currencyTransactionLog.Id, currencyTransactionLog);
                            }

                        }

                }
            }
        }
    }
}