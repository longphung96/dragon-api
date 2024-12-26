using System;
using System.Collections.Generic;
using Dragon.Blueprints;
using DragonAPI.Data;
using DragonAPI.Extensions;
using DragonAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swagger.Filter.OAuth2;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Dragon.Integration.Handler;
using DragonAPI.Application.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hostingContext, loggerConfiguration)
    => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.Configure<DatabaseMongoSettings>(
    builder.Configuration.GetSection("DatabaseMongoSettings"));
builder.Services.AddControllers();

builder.Services.AddGrpcClient<HubForwarder.HubForwarderClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcHubForwarderUrl"]);
});

builder.Services.AddAutoMapper(typeof(Program).Assembly)
    .AddJwtAuthentication(builder.Configuration)
    .AddIdGen(builder.Configuration)
    .AddRedis(builder.Configuration)
    .AddHttpClient(builder.Configuration);

// Add services to the container.
#region register service
builder.Services.AddDbContext<DragonDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("DbContext")));
builder.Services.AddSingleton<AsyncQueryableExecuter>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService<StartingAppService>();
builder.Services.AddHostedService<ConfigureMongoDbIndexesExtension>();

builder.Services.AddTransient<BattleService>();

builder.Services.AddScoped<IdentityService>();

builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<UserActionService>();

// builder.Services.ConfigureMongoDatabaseEntities(builder.Configuration.GetSection("DatabaseMongoSettings").Get<DatabaseSettings>());
#endregion
#region Hangfire
builder.Services.AddHangfire(configuration => configuration

    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)

    .UseSimpleAssemblyNameTypeSerializer()

    .UseRecommendedSerializerSettings()

    .UseMemoryStorage());

builder.Services.AddHangfireServer();

#endregion

#region CAP
builder.Services.AddTransient<IntegrationEventHandler>();
builder.Services.AddCap(conf =>
{
    conf.TopicNamePrefix = builder.Configuration["KafkaSettings:TopicNamePrefix"];
    conf.UsePostgreSql(opts => opts.ConnectionString = builder.Configuration.GetConnectionString("DbContext"));
    conf.UseKafka(opt =>
    {
        opt.Servers = builder.Configuration["KafkaSettings:Servers"];
    });
});
#endregion
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(builder.Configuration["CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()).SetIsOriginAllowedToAllowWildcardSubdomains()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddEnumsWithValuesFixFilters(o =>
    {
        // add schema filter to fix enums (add 'x-enumNames' for NSwag) in schema
        o.ApplySchemaFilter = true;

        // add parameter filter to fix enums (add 'x-enumNames' for NSwag) in schema parameters
        o.ApplyParameterFilter = true;

        // add document filter to fix enums displaying in swagger document
        o.ApplyDocumentFilter = true;

        // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' for schema extensions) for applied filters
        o.IncludeDescriptions = true;

        // add remarks for descriptions from xml-comments
        o.IncludeXEnumRemarks = true;

        // get descriptions from DescriptionAttribute then from xml-comments
        o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
    });
    var authority = builder.Configuration["JwtBearerConfig:Authority"];
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{authority}/connect/authorize", UriKind.Absolute),
                TokenUrl = new Uri($"{authority}/connect/token", UriKind.Absolute),
                Scopes = new Dictionary<string, string>
                            {
                                {"Dragon:battleserver:hub", "access Dragon battleserver hub"},
                                {"Dragon:hub", "access Dragon hub"},
                                {"Dragon:api", "access Dragon api"},
                                {"profile", "access user profile"},
                                {"openid", "openid"},
                                {"roles", "roles"}
                            }
            }
        }
    });
    options.OperationFilter<AuthorizeOperationFilter>();
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    using (var context = scope.ServiceProvider.GetRequiredService<DragonDbContext>())
    {
        await context.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var swaggerRoutePrefix = "dragon-api-swagger";
    app.UseSwagger(c =>
    {
        c.RouteTemplate = $"{swaggerRoutePrefix}/" + "{documentname}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = swaggerRoutePrefix;
        c.SwaggerEndpoint($"/{swaggerRoutePrefix}/v1/swagger.json", "DragonAPI");
        c.OAuthClientId("swagger");
        c.OAuthClientSecret("OsN8NtqTYLnMvA4cnk9FnSG7Sh7kxQiQTJu8onnU2d");
        c.OAuthAppName("OAuth-app");
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();