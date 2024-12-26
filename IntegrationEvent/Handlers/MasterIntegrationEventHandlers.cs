using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using DragonAPI.Extensions;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class MasterIntegrationEventHandlers : ICapSubscribe
    {
        private readonly ILogger<MasterIntegrationEventHandlers> logger;
        protected readonly ConfigLoader cfgLoader;
        protected readonly DragonService rongosService;
        protected readonly IAdminService adminService;
        protected readonly UserService userService;
        public MasterIntegrationEventHandlers(IAdminService adminService, DragonService rongosService, UserService userService, ConfigLoader cfgLoader, ILogger<MasterIntegrationEventHandlers> logger)
        {
            this.cfgLoader = cfgLoader;
            this.logger = logger;
            this.rongosService = rongosService;
            this.userService = userService;
            this.adminService = adminService;
        }

        [CapSubscribe(EventNameConst.MasterInGameCurrencyConsumeResponseEvent)]
        public async Task OnMasterInGameCurrencyConsumeResponseHandler(MasterInGameCurrencyConsumeRequestResponseETO evtData)
        {
            if (evtData.SourceEntity == DragonService.Entity)
            {
                logger.LogInformation(evtData.ToRawJson());
                await rongosService.ProcessMasterInGameCurrencyResponse(evtData);
            }

        }

        [CapSubscribe(EventNameConst.SyncMasterPremiumEvent)]
        public virtual async Task SyncUserHandler(MasterPremiumSyncEto evtData)
        {
            await userService.ProcessSyncPremiumMaster(evtData.MainUserId, evtData.IsPremium);
        }
        [CapSubscribe(EventNameConst.AdminTransferItemRequest)]
        public virtual async Task AdminTransferItem(AdminTransferItemEto evtData)
        {
            await adminService.ProcessTransferItem(evtData);
        }


    }
}
