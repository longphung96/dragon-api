using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using DragonAPI.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragonAPI.IntegrationEvent.Handlers
{
    public class CapSubscribeRandomGroupForConfigUpdateAttribute : CapSubscribeAttribute
    {
        public CapSubscribeRandomGroupForConfigUpdateAttribute(string name, bool isPartial = false)
        : base(name, isPartial)
        {
            Group = $"cap.queue.{typeof(Program).Assembly.GetName().Name.ToLower()}.configUpdate.{Guid.NewGuid().ToString("N")}";
        }
    }
    public class ConfigUpdateHandler : ICapSubscribe
    {
        private const string RoutingKey = "Blueprint.Config.Updated";
        private readonly ILogger<ConfigUpdateHandler> logger;
        protected readonly ConfigLoader cfgLoader;
        public ConfigUpdateHandler(ConfigLoader cfgLoader, ILogger<ConfigUpdateHandler> logger)
        {
            this.cfgLoader = cfgLoader;
            this.logger = logger;
        }
        [CapSubscribeRandomGroupForConfigUpdateAttribute(RoutingKey)]
        public async Task OnConfigVersionUpdated(bool UpdateConfig)
        {
            if (UpdateConfig)
            {
                logger.LogInformation($"OnConfigVersionUpdated");
                await cfgLoader.Reload();
            }
        }
    }
}
