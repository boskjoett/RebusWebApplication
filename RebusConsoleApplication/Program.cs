using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Polly;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Zylinc.Common.MessageBus.Messages.DataTypes.Organization;
using Zylinc.Common.MessageBus.Messages.RequestResponses.ConfigurationManager;
using Zylinc.Common.MessageBus.Messages.RequestResponses.Organization;

namespace RebusConsoleApplication
{
    class Program
    {
        private const string InputQueueName = "RebusConsoleApplication";
        private static BuiltinHandlerActivator _activator;
        private static IBus _bus;

        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Configuration = LoadConfiguration();

            _activator = new BuiltinHandlerActivator();

            string rabbitMqConnectionString = Configuration.GetConnectionString("RabbitMq");
            ConnectToRebus(rabbitMqConnectionString, _activator);

            // Register handlers for the messages this service must act on.
            _activator.Handle<UserLoginRequest>(async msg =>
            {
                await HandleUserLoginRequest(msg);
            });

            // Subscribe to messages we want to handle
            _bus.Subscribe<UserLoginRequest>().Wait();
            _bus.Subscribe<ServiceConfigurationRequest>().Wait();

            _bus.Dispose();
            _activator.Dispose();
        }

        private static async Task HandleUserLoginRequest(UserLoginRequest msg)
        {
            Console.WriteLine($"UserLoginRequest received. Request ID: {msg.RequestMessageId}, Email: {msg.Email}");

            Console.WriteLine("Publishing UserLoginResponse");
            await _bus.Publish(new UserLoginResponse(msg.RequestMessageId, LoginResultCode.LoginGranted, "user1", "bcs@zylinc.com", "Bo", "S"));
        }


        private static void ConnectToRebus(string rabbitMqConnectionString, BuiltinHandlerActivator activator)
        {
            Console.WriteLine($"Connecting to Rebus using RabbitMQ connection string: {rabbitMqConnectionString}");

            // Retry forever with a 10 seconds delay
            Policy
              .Handle<Exception>()
              .WaitAndRetryForever(sleepDurationProvider: (retryCount) =>
              {
                  // Wait 10 seconds between each retry
                  return TimeSpan.FromSeconds(10);
              },
              onRetry: (Exception, TimeSpan) => 
              {
                  // Action to perform on each retry
                  _bus = Configure.With(activator)
                      .Logging(l => l.Console(LogLevel.Info))
                      .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, InputQueueName))
                      .Routing(r => r.TypeBased()
                          .Map<UserLoginResponse>(InputQueueName)
                          .Map<ServiceConfigurationResponse>(InputQueueName))
                      .Start();
              });

            Console.WriteLine("Connected to Rebus");
        }


        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            return builder.Build();
        }
    }
}
