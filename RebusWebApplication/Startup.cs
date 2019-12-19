using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using RebusWebApplication.MessageBus;
using Zylinc.Common.MessageBus.Messages.RequestResponses.Organization;

namespace RebusWebApplication
{
    public class Startup
    {
        private const string InputQueueName = "RebusWebApplication";

        private readonly ILoggerFactory _loggerFactory;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // Register handlers 
            services.AutoRegisterHandlersFromAssemblyOf<MessageHandler>();

            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(Configuration.GetConnectionString("RabbitMQ"), InputQueueName))
                .Routing(r => r.TypeBased().MapAssemblyOf<UserLoginRequest>(InputQueueName)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.ApplicationServices.UseRebus(async bus => await bus.Subscribe<UserLoginResponse>());

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
