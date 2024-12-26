using DotNetCore.CAP.Internal;
using Hangfire;
using Hangfire.Redis;
using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using DragonAPI.Common;
using DragonAPI.Common.Calculator;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Filters;
using DragonAPI.IntegrationEvent.Handlers;
using DragonAPI.Permission;
using DragonAPI.Services;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.System.Text.Json;
using System.Net;
using System.Text.Json;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Volo.Abp.Linq;
using Volo.Abp.MongoDB;
using DragonAPI.Data;
using StackExchange.Redis.KeyspaceIsolation;
using DragonAPI.Repositories;

namespace DragonAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettingsSection = configuration.GetSection("RedisSettings");
            services.Configure<RedisSettings>(redisSettingsSection);
            var redisSettings = redisSettingsSection.Get<RedisSettings>();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisSettings.ConnectionString, conf =>
            {
                conf.DefaultDatabase = redisSettings.DefaultDatabase;
                conf.ChannelPrefix = redisSettings.Prefix;
                conf.AbortOnConnectFail = false;
            });
            redisConnectionMultiplexer.ConnectionFailed += (_, e) =>
            {
                Console.WriteLine($"Connection to Redis failed. {e.ToRawJson()}");
            };
            if (redisConnectionMultiplexer.IsConnected)
            {
                Console.WriteLine("connected to Redis.");
            }
            else
            {
                Console.WriteLine("Did not connect to Redis");
            }

            services
                .AddLogging()
                .AddJwtAuthentication(configuration)
                .AddAuthorization()
                .AddAutoMapper(typeof(Startup).Assembly)
                // .ConfigureDatabaseModels(configuration)
                .ConfigureRedis(redisConnectionMultiplexer, redisSettings)
                .AddHangfireServer(redisConnectionMultiplexer, redisSettings)
                .AddIntegrationEventHandler(configuration)
                .ConfigClientAccessToken(configuration)
                // .AddCustomApiVersioning()
                .AddCustomHealthCheck(configuration, redisSettings)
                .AddCustomSwagger(configuration)
                .ConfigureCors(configuration)
                .AddMediatR(typeof(Startup));
            services.ConfigureBusinessServices(configuration);
            return services;
        }

        public static IServiceCollection AddHangfireServer(
            this IServiceCollection services,
            ConnectionMultiplexer redisConn,
            RedisSettings redisSettings)
        {
            services.AddHangfire(c =>
            {
                c.UseRedisStorage(redisConn, new RedisStorageOptions
                {
                    Db = redisSettings.DefaultDatabase,
                    Prefix = redisSettings.Prefix + RedisStorageOptions.DefaultPrefix,
                });
            });
            services.AddHangfireServer(opts =>
            {
                opts.SchedulePollingInterval = TimeSpan.FromSeconds(2);
                opts.ServerCheckInterval = TimeSpan.FromSeconds(2);
                opts.ServerName = string.Format(
                    "{0}.{1}",
                    Environment.MachineName,
                    Guid.NewGuid().ToString());
            });
            return services;
        }

        public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConnectionMultiplexer redisConn, RedisSettings settings)
        {
            services.AddSingleton(redisConn);
            var redlockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new RedLockMultiplexer(redisConn)
                {
                    RedisDatabase = settings.DefaultDatabase,
                    RedisKeyFormat = $"{settings.Prefix}locker:{{0}}"
                },
            });
            services.AddSingleton<IDistributedLockFactory>(redlockFactory);
            services.AddScoped<IDatabase>(sp =>
            {
                return sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase().WithKeyPrefix(settings.Prefix);
            });
            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(new StackExchange.Redis.Extensions.Core.Configuration.RedisConfiguration
            {
                ConnectionString = settings.ConnectionString,
                Database = settings.DefaultDatabase,
                KeyPrefix = settings.Prefix,
            });
            return services;
        }

        // public static IServiceCollection AddCustomSignalR(this IServiceCollection services, IConnectionMultiplexer conn)
        // {
        //     services.AddSignalR().AddStackExchangeRedis(opts =>
        //     {
        //         opts.ConnectionFactory = writer =>
        //         {
        //             return Task.FromResult(conn);
        //         };
        //     });
        //     return services;
        // }


        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(opts =>
            {
                var identityServerUrl = configuration["JwtBearerConfig:Authority"];
                opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Common APIs", Version = "v1" });
                opts.AddEnumsWithValuesFixFilters();
                opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{identityServerUrl}/connect/authorize", UriKind.Absolute),
                            TokenUrl = new Uri($"{identityServerUrl}/connect/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                {"rongos:api", "access common api"},
                                {"rongos:hub", "access common hub"},
                                {"openid", "get user info"},
                                {"profile", "get user info"}
                            }
                        }
                    }
                });
                opts.OperationFilter<AuthorizeOperationFilter>();
            });
            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            return services;
        }

        public static void UseApiExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    //if any exception then report it and log it
                    if (contextFeature != null)
                    {
                        var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();

                        //Technical Exception for troubleshooting
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");

                        var response = new ApiResultDTO<object>();
                        if (contextFeature.Error is BusinessException be)
                        {
                            context.Response.StatusCode = be.HttpErrCode ?? (int)HttpStatusCode.InternalServerError;
                            response.AddError(be.Error, be.Message);
                        }
                        else
                        {
                            response.AddError(ErrorCodeEnum.UnexpectedErr, contextFeature.Error.Message);
                        }

                        var errorLog = JsonSerializer.Serialize(response, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        //Technical Exception for troubleshooting
                        logger.LogError($"Something went wrong: {errorLog}");

                        //Business exception - exit gracefully
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }

        public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app)
        {
            var routePrefix = "rongos-api-swagger";
            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{routePrefix}/" + "{documentname}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = routePrefix;
                c.SwaggerEndpoint($"/{routePrefix}/v1/swagger.json", "DragonAPI v1");
                c.OAuthClientId("rongos-api-swagger");
                c.OAuthClientSecret("OsN8NtqTYLnMvA4cnk9FnSG7Sh7kxQiQTJu8onnU2f");
                c.OAuthAppName("OAuth-app");
                c.OAuthUsePkce();
            });
            return app;
        }

        // public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        // {
        //     services.AddApiVersioning(config =>
        //     {
        //         config.DefaultApiVersion = new ApiVersion(1, 0);
        //         config.AssumeDefaultVersionWhenUnspecified = true;
        //         config.ReportApiVersions = true;
        //     });
        //     return services;
        // }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration, RedisSettings redisSettings)
        {
            var kafkaSettings = configuration.GetSection("KafkaSettings").Get<KafkaSettings>();

            //adding health check services to container
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRedis(redisSettings.ConnectionString, name: "redis", tags: new[] { "services" })
                .AddMongoDb(configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>().ConnectionString, name: "mongodb", tags: new[] { "services" })
                .AddKafka(new Confluent.Kafka.ProducerConfig() { BootstrapServers = kafkaSettings.Servers }, tags: new[] { "services" });
            return services;
        }


        public static IServiceCollection AddIntegrationEventHandler(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<ConfigUpdateHandler>();
            services.AddScoped<MasterIntegrationEventHandlers>();
            services.AddScoped<InventoryIntegrationEventHandlers>();
            services.AddScoped<BlockchainIntegrationEventsHandler>();
            services.AddScoped<IntegrationEventHandlers>();
            services.AddScoped<AssetsGenerationIntegrationEventsHandler>();
            services.AddScoped<BattleIntegrationEventsHandler>();
            var kafkaSettings = configuration.GetSection("KafkaSettings").Get<KafkaSettings>();
            services.AddCap(conf =>
            {
                conf.TopicNamePrefix = kafkaSettings.TopicNamePrefix;
                conf.UseMongoDB(opt =>
                {
                    var dbSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
                    opt.DatabaseConnection = dbSettings.ConnectionString;
                    opt.DatabaseName = dbSettings.DatabaseName;
                });
                conf.UseKafka(opt =>
                {
                    opt.Servers = kafkaSettings.Servers;
                    opt.CustomHeaders = e => new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(DotNetCore.CAP.Messages.Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                        new KeyValuePair<string, string>(DotNetCore.CAP.Messages.Headers.MessageName, e.Topic),
                    };
                });
            });
            return services;
        }

        public static IServiceCollection ConfigureBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSingleton<DragonMongoDbContext>();
            services.AddScoped<IMongoContext, DragonMongoDbContext>();
            services.AddScoped<IdentityService>();
            services.AddScoped<MasterService>();
            services.AddScoped<UserService>();
            services.AddScoped<UserActionService>();
            services.AddScoped<GameService>();
            services.AddScoped<BattleService>();
            services.AddScoped<WorldmapService>();
            services.AddScoped<DragonService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<IMarketPlaceService, MarketPlaceService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IItemMasterRepository, ItemMasterRepository>();
            services.AddScoped<IMasterRepository, MasterRepository>();
            // services.AddScoped<IAuthorizationHandler, PermissionsAuthorizationHandler>();

            // services.AddScoped<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
            services.AddSingleton<ConfigLoader>();
            services.AddSingleton<AsyncQueryableExecuter>();
            services.AddSingleton<MongoDbAsyncQueryableProvider>();
            services.AddSingleton<DragonCalculator>(services =>
                        {
                            var configloader = services.GetRequiredService<ConfigLoader>();
                            return new DragonCalculator(configloader.CalculatorConfigs);
                        });
            services
                .AddHostedService<StartingHostedService>()
                .AddHostedService<ConfigureMongoDbIndexesExtension>();
            return services;
        }

        public static IServiceCollection ConfigClientAccessToken(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddClientAccessTokenManagement(opts =>
            {
                var credentialsConf = configuration.GetSection("DragonApiClientCredentials");
                opts.Clients.Add("rongos-api", new ClientCredentialsTokenRequest
                {
                    Address = credentialsConf["TokenAddress"],
                    ClientId = credentialsConf["ClientId"],
                    ClientSecret = credentialsConf["ClientSecret"],
                    Scope = credentialsConf["Scope"]
                });
            });
            services.AddClientAccessTokenHttpClient("DragonApiClient", configureClient: client =>
            {
                var url = configuration["JwtBearerConfig:Authority"];
                client.BaseAddress = new Uri(url);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseCookies = false });
            return services;
        }
    }
}