using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Block.Processing.Configuration;
using Block.Processing.Services;
using Block.Processing.Models;
using Block.Processing.Providers;
using Block.Processing.Listeners;
using Block.Processing.DTO;
using System;

namespace Block.Processing
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            Configuration = GetConfiguration();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            Log.Logger.Information("Starting Block.Processing App ...");
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
            var envConfig = new EnviromentConfiguration
            {
                DBConnection = Environment.GetEnvironmentVariable("DB_CONNECTION"),
                RmqConnection = Environment.GetEnvironmentVariable("RMQ_CONNECTION"),
                RedisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
            };
            Console.WriteLine(envConfig.DBConnection);
            service.AddRabbitMQ(envConfig.RmqConnection);
            service.AddSingleton(envConfig);
            service.AddSingleton<IBlockNotificationSubscriber<BlockInfo>, BlockSubscriber>();

            service.AddSingleton<BlockProviderPg>();
            service.AddSingleton<BlockModel>();
            service.AddSingleton<BlockProviderPg>();

            service.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = envConfig.RedisConnection;
                options.InstanceName = "BlockInstance01";
            });

            service.AddHostedService<BlockRmqSubscriber>();
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