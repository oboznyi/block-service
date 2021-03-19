using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Block.Sender.Configuration;
using Block.Sender.Services;
using Block.Sender.Models;
using System;

namespace Block.Sender
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            Configuration = GetConfiguration();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            Log.Logger.Information("Starting BlockSender App ...");
            var host = CreateHostBuilder(args);

            host.Build().Run();
            Log.Logger.Information("Application is successfully started");
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((ILoggingBuilder loggingConfiguration) => loggingConfiguration.ClearProviders())
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureServices((hostContext, services) =>
                     {
                         ConfigureServices(services);
                     });
        }
        private static void ConfigureServices(IServiceCollection service)
        {
            var rmqConnection = Environment.GetEnvironmentVariable("RMQ_CONNECTION");
            if(rmqConnection == null)
                throw new ArgumentException("Enviroment variable RMQ_CONNECTION does not set RMQ_CONNECTION");

            Console.WriteLine(rmqConnection);
            service.AddRabbitMQ(rmqConnection);
            service.Configure<SenderConfiguration>((x) =>
            {
                x.BlockDelayInSeconds = int.Parse(Environment.GetEnvironmentVariable("BLOCK_DELAY_IN_SECONDS"));
            });
            service.AddSingleton<BlockPublisher>();
            service.AddSingleton<IBlockModel, BlockModel>();
            service.AddHostedService<BlockService>();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            
            return builder.Build();
        }
    }
}