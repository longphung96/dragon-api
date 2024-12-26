// using Hangfire;
// using JohnKnoop.MongoRepository;
// using Microsoft.Extensions.Logging;
// using MongoDB.Driver;
// using DragonAPI.Configurations;
// using DragonAPI.Extensions;
// using DragonAPI.Models.DAOs;
// using DragonAPI.Services;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using TrueSight.Lite.Common;

// namespace DragonAPI.Tasks
// {
//     public class DragonTasks : IServiceScoped
//     {
//         private readonly ILogger<DragonTasks> logger;
//         private readonly IBackgroundJobClient backgroundJobClient;
//         private readonly GameConfigs gameConfigs;
//         private readonly IRepository<DragonDAO> rongosRepo;
//         public DragonTasks(
//             ILogger<DragonTasks> logger,
//             IBackgroundJobClient backgroundJobClient,
//             IRepository<DragonDAO> rongosRepo,
//             ConfigLoader cfgLoader)
//         {
//             this.logger = logger;
//             this.backgroundJobClient = backgroundJobClient;
//             gameConfigs = cfgLoader.GameConfigs;
//             this.rongosRepo = rongosRepo;
//         }
//         public void TriggerBonusHappyPointTask(DragonDAO rongos, DateTimeOffset enqueueAt)
//         {
//             logger.LogDebug($"TriggerBonusHappyPointTask rongosId: {rongos.Id}");
//             backgroundJobClient.Schedule<DragonTasks>(t => t.BonusHappyPointScheduledHandler(rongos.Id), enqueueAt.AddSeconds(1));
//         }
//         public async Task BonusHappyPointScheduledHandler(string rongosId)
//         {
//             var rongos = await rongosRepo.FindOneAsync(m => m.Id == rongosId);
//             if (rongos == null)
//                 return;
//             logger.LogDebug($"BonusHappyPointScheduledHandler rongosId: {rongosId}");
//             var builder = Builders<DragonDAO>.Update;
//             var updateDefs = new List<UpdateDefinition<DragonDAO>>();
//             var newHappyPoint = rongos.HappyPoint + gameConfigs.DragonStressConfig.NumOfHappyPointRegeneration;
//             await UpdateHappyPoint(rongos, newHappyPoint);
//             var happyPointGeneration = new
//             {
//                 rongos.WalletId,
//                 rongos.NftId,
//                 Amount = gameConfigs.DragonStressConfig.NumOfHappyPointRegeneration,
//             };
//             logger.LogAnalysis(happyPointGeneration, LoggingContextEnum.DragonServiceContext, "rongos", "happy_point_regeneration");
//         }

//         public async Task UpdateHappyPoint(DragonDAO rongos, int happyPoint)
//         {
//             int maxHappyPoint = gameConfigs.DragonStressConfig.MaxHappyPoint;
//             var builder = Builders<DragonDAO>.Update;
//             var updateDefs = new List<UpdateDefinition<DragonDAO>>();

//             DateTime nextBonusHappyPointAt = DateTime.UtcNow;
//             bool shouldTriggerBonus = false;
//             if (happyPoint >= maxHappyPoint)
//             {
//                 logger.LogInformation($"The rongos {rongos.NftId} reached max Happy now {maxHappyPoint}");
//                 happyPoint = maxHappyPoint;
//             }
//             else
//             {
//                 if (happyPoint <= 0) happyPoint = 0;
//                 if (rongos.HappyPointBonusAt <= nextBonusHappyPointAt)
//                 {
//                     shouldTriggerBonus = true;
//                     if (rongos.HappyPoint >= gameConfigs.DragonStressConfig.NotHappyPoint)
//                         nextBonusHappyPointAt = nextBonusHappyPointAt.AddSeconds(gameConfigs.DragonStressConfig.SecondOfHappyPointRegeneration);
//                     else
//                     {
//                         nextBonusHappyPointAt = nextBonusHappyPointAt.AddSeconds(gameConfigs.DragonStressConfig.SecondOfNotHappyPointRegeneration);
//                     }
//                     updateDefs.Add(builder.Set(x => x.HappyPointBonusAt, nextBonusHappyPointAt));
//                 }
//             }
//             updateDefs.Add(builder.Set(x => x.HappyPoint, happyPoint));
//             var updateResult = await rongosRepo.UpdateOneAsync(rongos.Id, x => x.Combine(updateDefs));
//             if (updateResult.ModifiedCount > 0)
//             {
//                 //await serverHub.Clients.Group(rongos.WalletId).MasterEnergyChanged(happyPoint, (uint)maxHappyPoint);
//                 if (shouldTriggerBonus)
//                 {
//                     TriggerBonusHappyPointTask(rongos, new DateTimeOffset(nextBonusHappyPointAt));
//                     //await serverHub.Clients.Group(rongos.WalletId).MasterBonusEnergyAtChanged(nextBonusHappyPointAt);
//                 }
//             }
//         }
//     }
// }
