using DotNetCore.CAP;
using JohnKnoop.MongoRepository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DragonAPI.Data;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DAOs;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class AssetsGenerationIntegrationEventsHandler : ICapSubscribe
    {
        private readonly DragonMongoDbContext mongoDbContext;
        private readonly IMongoCollection<DragonDAO> rongosRepo;
        private readonly ILogger<AssetsGenerationIntegrationEventsHandler> logger;
        public AssetsGenerationIntegrationEventsHandler(
            DragonMongoDbContext mongoDbContext,
            ILogger<AssetsGenerationIntegrationEventsHandler> logger)
        {
            this.rongosRepo = mongoDbContext.GetCollection<DragonDAO>("dragons"); ;
            this.logger = logger;
        }
        [CapSubscribe(EventNameConst.NftAssetsGeneratedResponseEvent)]
        public async Task OnNftAssetsGeneratedResponseEvent(AssetsGenerationIntegrationEvent data)
        {
            if (data.Entity == "dragon")
            {
                logger.LogInformation($"OnNftAssetsGeneratedResponseEvent {JsonSerializer.Serialize(data)}");
                await handleDragonAssetsGenerated(data);
            }
        }
        private async Task handleDragonAssetsGenerated(AssetsGenerationIntegrationEvent data)
        {
            var rongos = await rongosRepo.Find(r => r.Id == data.Id).FirstOrDefaultAsync();
            rongos.AssetGeneratedAt = data.RenderedTime;
            await rongosRepo.ReplaceOneAsync(rongos.Id, rongos);
        }
    }
}
