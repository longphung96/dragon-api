using DotNetCore.CAP;
using Microsoft.AspNetCore.SignalR;
using DragonAPI.IntegrationEvent.Events;
using System.Text.Json;
using DragonAPI.Services;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class BattleIntegrationEventsHandler : ICapSubscribe
    {
        private readonly ILogger<BattleIntegrationEventsHandler> _logger;
        private readonly BattleService battleService;
        public BattleIntegrationEventsHandler(
            BattleService battleService,
            ILogger<BattleIntegrationEventsHandler> logger
        )
        {
            _logger = logger;
            this.battleService = battleService;
        }
        // [CapSubscribe(EventNameConst.BattleServerRegisteredEvent)]
        // public async Task OnBattleRegistered(BattleServerRegisteredIntegrationEvent registedBattle)
        // {
        //     var dataString = JsonSerializer.Serialize(registedBattle);
        //     _logger.LogInformation($"OnBattleRegistered {dataString}");
        //     await battleService.ProcessBattleRegistered(registedBattle);
        // }
        [CapSubscribe(EventNameConst.BattleServerStateUpdatedEvent)]
        public async Task OnBattleStateUpdatedHandler(BattleServerStateChangedIntegrationEvent battleData)
        {
            var dataStr = JsonSerializer.Serialize(battleData);
            _logger.LogInformation($"OnBattleStateUpdatedHandler {dataStr}");
            await battleService.ProcessBattleStateUpdatedEvt(battleData);
        }
        [CapSubscribe(EventNameConst.BattleServerBattlesDestroyedEvent)]
        public async Task OnBattlesDestroyedHandler(BattlesDestroyedIntegrationEvent eventData)
        {
            var dataStr = JsonSerializer.Serialize(eventData);
            _logger.LogInformation($"OnBattlesDestroyedHandler {dataStr}");
            await battleService.ProcessBattlesDestroyedEvt(eventData);
        }

    }
}