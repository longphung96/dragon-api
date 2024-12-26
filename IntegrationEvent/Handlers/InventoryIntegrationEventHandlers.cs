using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using DragonAPI.Extensions;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class InventoryIntegrationEventHandlers : ICapSubscribe
    {
        private readonly ILogger<InventoryIntegrationEventHandlers> logger;
        protected readonly DragonService rongosService;
        public InventoryIntegrationEventHandlers(DragonService rongosService, ILogger<InventoryIntegrationEventHandlers> logger)
        {
            this.logger = logger;
            this.rongosService = rongosService;
        }



    }
}
