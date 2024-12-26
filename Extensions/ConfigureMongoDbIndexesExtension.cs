using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using DragonAPI.Application.Settings;
using MongoDB.Driver;
using System.Linq;
using DragonAPI.Models.DAOs;

namespace DragonAPI.Extensions
{
    public class ConfigureMongoDbIndexesExtension : IHostedService
    {
        private readonly IMongoClient client;
        private readonly ILogger<ConfigureMongoDbIndexesExtension> _logger;

        private readonly IConfiguration configuration;
        public ConfigureMongoDbIndexesExtension(
            IMongoClient client,
            IConfiguration configuration,
            ILogger<ConfigureMongoDbIndexesExtension> logger
        )
        {
            this.client = client;
            this.configuration = configuration;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("begin configure index");
            var tasks = new Func<Task>[]
            {
                async () =>
                {
                    await BuildUserCollectionIndexes(cancellationToken);
                    await BuildMasterCollectionIndexes(cancellationToken);
                    await BuildMasterWalletsCollectionIndexes(cancellationToken);
                    await BuildMasterBalanceTxHistoryCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildDragonCollectionIndexes(cancellationToken);

                },
                async () =>
                {
                    await BuildSettingsCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildMasterStageTrackingCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildItemMasterCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildCurrencyTransactionCollectionIndexes(cancellationToken);
                    await BuildWithDrawVerificationCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildPvPHistoryCollectionIndexes(cancellationToken);
                },
                async () =>
                {
                    await BuildClaimableItemCollectionIndexes(cancellationToken);
                },
            }.AsParallel()
                .WithDegreeOfParallelism(14)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Select(async x => await x());
            await Task.WhenAll(tasks);
            _logger.LogDebug("finished configure index");
        }
        private async Task BuildUserCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var userCollection = client.GetCollection<UserDAO>("users");
            var indexOnCreatedAt = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.CreatedAt));
            var indexOnUserId = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.UserId));
            // var indexOnOffchainWalletId = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.OffchainWalletId));
            // var indexOnOnchainWalletIds = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.OnchainWalletIds));
            var indexOnMainUserId = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.MainUserId));
            var indexOnPermissions = new CreateIndexModel<UserDAO>(Builders<UserDAO>.IndexKeys.Ascending(m => m.Permissions));
            await userCollection.Indexes.CreateManyAsync(new[] { indexOnCreatedAt, indexOnUserId, indexOnMainUserId, indexOnPermissions }, cancellationToken);
            _logger.LogDebug("--BuildUserCollectionIndexes--");
        }
        private async Task BuildMasterWalletsCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var collection = client.GetCollection<MasterWalletDAO>("masterWallets");
            var indexOnMainUserId = new CreateIndexModel<MasterWalletDAO>(Builders<MasterWalletDAO>.IndexKeys.Ascending(m => m.MainUserId));
            var indexOnWalletId = new CreateIndexModel<MasterWalletDAO>(Builders<MasterWalletDAO>.IndexKeys.Ascending(m => m.WalletId));

            var indexUnique = new CreateIndexModel<MasterWalletDAO>
            (
                Builders<MasterWalletDAO>.IndexKeys.Combine(new[] { indexOnWalletId.Keys, indexOnMainUserId.Keys }),
                new CreateIndexOptions
                {
                    Unique = true
                }
            );

            await collection.Indexes.CreateManyAsync(new[] { indexOnMainUserId, indexOnWalletId, indexUnique }, cancellationToken);
            _logger.LogDebug("--BuildMasterWalletsCollectionIndexes--");



        }
        private async Task BuildMasterBalanceTxHistoryCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var collection = client.GetCollection<MasterBalanceTransactionHistoryDAO>("masterBalanceTransactionHistories");
            var indexOnMainUserId = new CreateIndexModel<MasterBalanceTransactionHistoryDAO>(Builders<MasterBalanceTransactionHistoryDAO>.IndexKeys.Ascending(m => m.MainUserId));
            await collection.Indexes.CreateManyAsync(new[] { indexOnMainUserId }, cancellationToken);
            _logger.LogDebug("--index BuildMasterBalanceTxHistoryCollectionIndexes done--");
        }
        private async Task BuildSettingsCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var settingsCollection = client.GetCollection<SettingsDAO>("settings");
            var indexOnMasterId = new CreateIndexModel<SettingsDAO>(Builders<SettingsDAO>.IndexKeys.Ascending(m => m.UserId));
            await settingsCollection.Indexes.CreateOneAsync(indexOnMasterId, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildSettingsCollectionIndexes--");
        }
        private async Task BuildMasterCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var masterCollection = client.GetCollection<MasterDAO>("masters");
            var indexOnCreatedAt = new CreateIndexModel<MasterDAO>(Builders<MasterDAO>.IndexKeys.Ascending(m => m.CreatedAt));
            var indexOnMainUserId = new CreateIndexModel<MasterDAO>(Builders<MasterDAO>.IndexKeys.Ascending(m => m.MainUserId), new CreateIndexOptions { Unique = true });
            var indexOnWallets = new CreateIndexModel<MasterDAO>(Builders<MasterDAO>.IndexKeys.Ascending(m => m.Wallets));
            await masterCollection.Indexes.CreateManyAsync(new[] { indexOnCreatedAt, indexOnWallets, indexOnMainUserId }, cancellationToken);
            _logger.LogDebug("--BuildMasterCollectionIndexes--");
        }

        private async Task BuildMasterStageTrackingCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var masterStageTrackingCollection = client.GetCollection<MasterStageTrackingDAO>("master_stage_tracking");
            var indexOnMasterId = new CreateIndexModel<MasterStageTrackingDAO>(Builders<MasterStageTrackingDAO>.IndexKeys.Ascending(_ => _.MasterId));
            var indexOnMainUserId = new CreateIndexModel<MasterStageTrackingDAO>(Builders<MasterStageTrackingDAO>.IndexKeys.Ascending(_ => _.MainUserId));
            var indexOnStageId = new CreateIndexModel<MasterStageTrackingDAO>(Builders<MasterStageTrackingDAO>.IndexKeys.Ascending(_ => _.StageId));
            var combineIndex = new CreateIndexModel<MasterStageTrackingDAO>(Builders<MasterStageTrackingDAO>.IndexKeys.Combine(
                new[]
                {
                    indexOnMasterId.Keys,
                    indexOnMainUserId.Keys,
                    indexOnStageId.Keys,
                }
            ));
            await masterStageTrackingCollection.Indexes.CreateManyAsync(new[] { indexOnMasterId, indexOnMainUserId, indexOnStageId, combineIndex }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildMasterStageTrackingCollectionIndexes--");
        }
        private async Task BuildItemMasterCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var itemMasterCollection = client.GetCollection<ItemMasterDAO>("item_master");
            var indexOnMainUserId = new CreateIndexModel<ItemMasterDAO>(Builders<ItemMasterDAO>.IndexKeys.Ascending(_ => _.MainUserId));
            var indexOnStageId = new CreateIndexModel<ItemMasterDAO>(Builders<ItemMasterDAO>.IndexKeys.Ascending(_ => _.ItemId));
            var combineIndex = new CreateIndexModel<ItemMasterDAO>(Builders<ItemMasterDAO>.IndexKeys.Combine(
                new[]
                {
                    indexOnMainUserId.Keys,
                    indexOnStageId.Keys,
                }
            ));
            await itemMasterCollection.Indexes.CreateManyAsync(new[] { indexOnMainUserId, indexOnStageId, combineIndex }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildItemMasterCollectionIndexes--");
        }
        private async Task BuildDragonCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var dragonCollection = client.GetCollection<DragonDAO>("dragons");


            // var indexOnNftIdDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.NftId);
            var indexOnNftIDescdDef = Builders<DragonDAO>.IndexKeys.Descending(m => m.NftId);

            var indexOnBirthdayDescDef = Builders<DragonDAO>.IndexKeys.Descending(m => m.Birthday);
            var indexOnSaleStatusDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.SaleStatus);
            var indexOnLevelDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.Level);
            var indexOnCooldownEndAtDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.CooldownEndAt);
            var indexOnBodypartsDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.Bodyparts);

            var indexOnClassDef = Builders<DragonDAO>.IndexKeys.Ascending(m => m.Class);
            var indexOnCreatedAt = new CreateIndexModel<DragonDAO>(Builders<DragonDAO>.IndexKeys.Ascending(m => m.CreatedAt));
            var indexOnWalletId = new CreateIndexModel<DragonDAO>(Builders<DragonDAO>.IndexKeys.Ascending(m => m.WalletId));
            var indexOnNftId = new CreateIndexModel<DragonDAO>(indexOnNftIDescdDef, new CreateIndexOptions<DragonDAO> { Unique = true });
            var indexOnBirthday = new CreateIndexModel<DragonDAO>(indexOnBirthdayDescDef);

            var indexOnSaleStatus = new CreateIndexModel<DragonDAO>(indexOnSaleStatusDef);
            var indexOnLevel = new CreateIndexModel<DragonDAO>(indexOnLevelDef);
            var indexOnCooldownEndAt = new CreateIndexModel<DragonDAO>(indexOnCooldownEndAtDef);

            var indexOnClass = new CreateIndexModel<DragonDAO>(indexOnClassDef);
            var indexOnBodyparts = new CreateIndexModel<DragonDAO>(indexOnBodypartsDef);

            var indexCombineBirthdayNftIdDesc = new CreateIndexModel<DragonDAO>(Builders<DragonDAO>.IndexKeys.Combine(new[] { indexOnBirthdayDescDef, indexOnNftIDescdDef }));

            await dragonCollection.Indexes.CreateManyAsync(new[] {
                indexCombineBirthdayNftIdDesc,
                indexOnCreatedAt, indexOnWalletId, indexOnNftId, indexOnBirthday,
                indexOnSaleStatus, indexOnLevel, indexOnCooldownEndAt, indexOnClass, indexOnBodyparts
            }, cancellationToken);
            _logger.LogDebug("--BuildDragonCollectionIndexes--");
        }


        // private async Task BuildDragonSealCollectionIndexes(CancellationToken cancellationToken)
        // {
        //     var dragonSealCollection = client.GetCollection<DragonSealDAO>();
        //     var indexOnCreatedAt = new CreateIndexModel<DragonSealDAO>(Builders<DragonSealDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
        //     var indexOnWalletId = new CreateIndexModel<DragonSealDAO>(Builders<DragonSealDAO>.IndexKeys.Ascending(_ => _.WalletId));
        //     var indexOnNftId = new CreateIndexModel<DragonSealDAO>(Builders<DragonSealDAO>.IndexKeys.Ascending(_ => _.NftId), new CreateIndexOptions<DragonSealDAO> { Unique = true });
        //     await dragonSealCollection.Indexes.CreateManyAsync(new[] { indexOnWalletId, indexOnCreatedAt, indexOnNftId }, cancellationToken: cancellationToken);
        //     _logger.LogDebug("--BuildDragonSealCollectionIndexes--");
        // }
        // private async Task BuildInventoryCollectionIndexes(CancellationToken cancellationToken)
        // {
        //     _logger.LogDebug("--BuildInventoryCollectionIndexes--");
        //     var inventoryCollection = client.GetCollection<InventoryItemDAO>();
        //     var indexOnCreatedAt = new CreateIndexModel<InventoryItemDAO>(Builders<InventoryItemDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
        //     var indexOnWalletId = new CreateIndexModel<InventoryItemDAO>(Builders<InventoryItemDAO>.IndexKeys.Ascending(_ => _.WalletId));
        //     await inventoryCollection.Indexes.CreateManyAsync(new[] { indexOnWalletId, indexOnCreatedAt }, cancellationToken: cancellationToken);
        // }
        // private async Task BuildLandedItemCollectionIndexes(CancellationToken cancellationToken)
        // {
        //     _logger.LogDebug("--BuildLandedItemCollectionIndexes--");
        //     var landedItemsCollection = client.GetCollection<LandedItemDAO>();
        //     var indexOnCreatedAt = new CreateIndexModel<LandedItemDAO>(Builders<LandedItemDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
        //     var indexOnMasterId = new CreateIndexModel<LandedItemDAO>(Builders<LandedItemDAO>.IndexKeys.Ascending(_ => _.MasterId));
        //     var indexOnItemId = new CreateIndexModel<LandedItemDAO>(Builders<LandedItemDAO>.IndexKeys.Ascending(_ => _.ItemId));
        //     await landedItemsCollection.Indexes.CreateManyAsync(new[] { indexOnItemId, indexOnMasterId, indexOnCreatedAt }, cancellationToken: cancellationToken);
        // }

        // private async Task BuildBattleCollectionIndexes(CancellationToken cancellationToken)
        // {
        //     var battleCollection = client.GetCollection<BattleDAO>();
        //     var indexOnCreatedAt = new CreateIndexModel<BattleDAO>(Builders<BattleDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
        //     var indexOnMasters = new CreateIndexModel<BattleDAO>(Builders<BattleDAO>.IndexKeys.Ascending(_ => _.Masters));
        //     var indexOnType = new CreateIndexModel<BattleDAO>(Builders<BattleDAO>.IndexKeys.Ascending(_ => _.Type));
        //     await battleCollection.Indexes.CreateManyAsync(new[] { indexOnType, indexOnMasters, indexOnCreatedAt }, cancellationToken: cancellationToken);
        //     _logger.LogDebug("--BuildBattleCollectionIndexes--");
        // }
        private async Task BuildRankPositionCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var RankPositionCollection = client.GetCollection<RankPositionDAO>("rankPosition");
            var seasonIdIndexKey = Builders<RankPositionDAO>.IndexKeys.Ascending(_ => _.SeasonId);
            var masterIdIndexKey = Builders<RankPositionDAO>.IndexKeys.Ascending(_ => _.MainUserId);
            var rankPositionIndexKey = Builders<RankPositionDAO>.IndexKeys.Ascending(_ => _.RankPosition);
            var tierIndexKey = Builders<RankPositionDAO>.IndexKeys.Ascending(_ => _.Tier);
            var indexOnCreatedAt = new CreateIndexModel<RankPositionDAO>(Builders<RankPositionDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
            var indexOnSessionId = new CreateIndexModel<RankPositionDAO>(seasonIdIndexKey);
            var indexOnMasterId = new CreateIndexModel<RankPositionDAO>(masterIdIndexKey);
            var indexOnRankPosition = new CreateIndexModel<RankPositionDAO>(rankPositionIndexKey);
            var indexOnTier = new CreateIndexModel<RankPositionDAO>(tierIndexKey);
            var indexOnSeasonIdAndRankPosition = new CreateIndexModel<RankPositionDAO>(Builders<RankPositionDAO>.IndexKeys.Combine(new[] { seasonIdIndexKey, rankPositionIndexKey }), new CreateIndexOptions
            {
                Unique = true
            });
            var indexOnSeasonIdAndMasterId = new CreateIndexModel<RankPositionDAO>(Builders<RankPositionDAO>.IndexKeys.Combine(new[] { seasonIdIndexKey, masterIdIndexKey }), new CreateIndexOptions
            {
                Unique = true
            });
            var indexOnTierAndRankPosition = new CreateIndexModel<RankPositionDAO>(Builders<RankPositionDAO>.IndexKeys.Combine(new[] { rankPositionIndexKey, tierIndexKey }), new CreateIndexOptions
            {
                Unique = true
            });
            await RankPositionCollection.Indexes.CreateManyAsync(new[] { indexOnSessionId, indexOnSeasonIdAndMasterId, indexOnSeasonIdAndRankPosition, indexOnTierAndRankPosition, indexOnMasterId, indexOnCreatedAt }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildRankPositionCollectionIndexes--");
        }
        private async Task BuildWithDrawVerificationCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var battleCollection = client.GetCollection<WithDrawVerificationDAO>("withdraw_verification");
            var indexOnCreatedAt = new CreateIndexModel<WithDrawVerificationDAO>(Builders<WithDrawVerificationDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
            var indexOnTransactionId = new CreateIndexModel<WithDrawVerificationDAO>(Builders<WithDrawVerificationDAO>.IndexKeys.Ascending(_ => _.CurrencyTransactionId));
            var indexOnReviewerId = new CreateIndexModel<WithDrawVerificationDAO>(Builders<WithDrawVerificationDAO>.IndexKeys.Ascending(_ => _.ReviewerId));
            await battleCollection.Indexes.CreateManyAsync(new[] { indexOnReviewerId, indexOnTransactionId, indexOnCreatedAt }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildWithDrawVerificationCollectionIndexes--");
        }
        private async Task BuildCurrencyTransactionCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var battleCollection = client.GetCollection<CurrencyTransactionDAO>("currency_transaction");
            var indexOnCreatedAt = new CreateIndexModel<CurrencyTransactionDAO>(Builders<CurrencyTransactionDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
            var indexOnItemId = new CreateIndexModel<CurrencyTransactionDAO>(Builders<CurrencyTransactionDAO>.IndexKeys.Ascending(_ => _.ItemId));
            var indexOnUserId = new CreateIndexModel<CurrencyTransactionDAO>(Builders<CurrencyTransactionDAO>.IndexKeys.Ascending(_ => _.MainUserId));
            await battleCollection.Indexes.CreateManyAsync(new[] { indexOnItemId, indexOnUserId, indexOnCreatedAt }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildCurrencyTransactionCollectionIndexes--");
        }
        private async Task BuildPvPHistoryCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var battleCollection = client.GetCollection<PvPHistoryDAO>("pvp_history");
            var indexOnCreatedAt = new CreateIndexModel<PvPHistoryDAO>(Builders<PvPHistoryDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
            var masterIdIndexKey = Builders<PvPHistoryDAO>.IndexKeys.Ascending(_ => _.MainUserId);
            var battleIdIndexKey = Builders<PvPHistoryDAO>.IndexKeys.Ascending(_ => _.BattleId);
            var indexOnUserId = new CreateIndexModel<PvPHistoryDAO>(masterIdIndexKey);
            var indexOnBattleId = new CreateIndexModel<PvPHistoryDAO>(battleIdIndexKey);
            await battleCollection.Indexes.CreateManyAsync(new[] { indexOnUserId, indexOnCreatedAt, indexOnBattleId }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildPvPHistoryCollectionIndexes--");
        }
        private async Task BuildClaimableItemCollectionIndexes(CancellationToken cancellationToken)
        {
            DatabaseMongoSettings databaseMongoSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseMongoSettings>();
            var mongoClient = new MongoClient(
            databaseMongoSettings.ConnectionString);

            var client = mongoClient.GetDatabase(
                databaseMongoSettings.DatabaseName);
            var battleCollection = client.GetCollection<ClaimableItemDAO>("claim_items");
            var indexOnCreatedAt = new CreateIndexModel<ClaimableItemDAO>(Builders<ClaimableItemDAO>.IndexKeys.Ascending(_ => _.CreatedAt));
            var masterIdIndexKey = Builders<ClaimableItemDAO>.IndexKeys.Ascending(_ => _.MainUserId);
            var RequestIdIndexKey = Builders<ClaimableItemDAO>.IndexKeys.Ascending(_ => _.RequestId);
            var indexOnUserId = new CreateIndexModel<ClaimableItemDAO>(masterIdIndexKey);
            var indexOnRequestId = new CreateIndexModel<ClaimableItemDAO>(RequestIdIndexKey);
            await battleCollection.Indexes.CreateManyAsync(new[] { indexOnUserId, indexOnCreatedAt, indexOnRequestId }, cancellationToken: cancellationToken);
            _logger.LogDebug("--BuildClaimableItemCollectionIndexes--");
        }
        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}