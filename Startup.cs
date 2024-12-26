using Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using TrueSight.Lite.Common;
using System.Reflection;
using DragonAPI.Repositories;
using DragonAPI.Extensions;
using DragonAPI.Filters;
using DragonAPI.Data;
using MongoDB.Bson.Serialization.Conventions;

namespace DragonAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // License.Activate("SRHFONpPQfAQdBq7xD8xsInxiM0Z1pjLW4rsg+D35a0=");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.ConfigureServices(Configuration);
            services.AddSingleton<IRedisStore, RedisStore>();
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddGrpcClient<HubForwarder.HubForwarderClient>(o =>
            {
                o.Address = new Uri(Configuration["GrpcHubForwarderUrl"]);
            });

            // Assembly[] assemblies = new[] { Assembly.GetAssembly(typeof(IServiceScoped)),
            //     Assembly.GetAssembly(typeof(Startup)) };
            // services.Scan(scan => scan
            //     .FromAssemblies(assemblies)
            //     .AddClasses(classes => classes.AssignableTo<IServiceScoped>())
            //          .AsImplementedInterfaces()
            //          .WithScopedLifetime());


            ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.ConfigureSwagger();
                // app.UseElasticApm(Configuration);
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseApiExceptionHandler();
            // app.UseHttpsRedirection();
            // app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            // app.UseSentryTracing();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] {
                    new HangfireAuthorizationFilter ("admin")
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.UseHealthChecks("/self", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self"),
            });
            app.UseHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("services"),
            });
        }
    }
}
