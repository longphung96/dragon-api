using System;
using AutoMapper;
using MediatR;
using Hangfire;

namespace DragonAPI.Services
{
    public class StartingHostedService : BackgroundService
    {
        protected readonly ILogger<StartingHostedService> logger;
        protected readonly ConfigLoader cfgLoader;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly IServiceProvider serviceProvider;
        public StartingHostedService(IMediator mediator, ILogger<StartingHostedService> logger, ConfigLoader cfgLoader, IMapper mapper, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.cfgLoader = cfgLoader;
            this.mapper = mapper;
            this.mediator = mediator;
            this.serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("StartAsync");
            await cfgLoader.Reload();
            var scope = this.serviceProvider.CreateScope();

            //////////////
            var masterService = scope.ServiceProvider.GetRequiredService<MasterService>();
            var inventoryService = scope.ServiceProvider.GetRequiredService<InventoryService>();
            RecurringJob.AddOrUpdate("master_check_vip_hourly", () => masterService.RecheckMasterVipLevelJobHourly(), Cron.Hourly);
            RecurringJob.AddOrUpdate("daily_reward_send", () => inventoryService.ProcessSendDailyRankingReward(), Cron.Daily(21));
            RecurringJob.AddOrUpdate("seasin_reward_send", () => inventoryService.ProcessSendSeasonRankingReward(), Cron.Daily());
            //////////////

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("StopAsync");
            await base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("ExecuteAsync");
            return Task.CompletedTask;
        }
    }
}