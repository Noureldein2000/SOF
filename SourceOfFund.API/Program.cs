using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SourceOfFund.Data;
using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using Serilog.Events;

namespace SourceOfFund.API
{
    public class Program
    {
        public static void Main(string[] args)

        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var environment = services.GetRequiredService<IWebHostEnvironment>();
            var envPath = AppDomain.CurrentDomain.BaseDirectory;//environment.ContentRootPath;

            //string path = Directory.GetCurrentDirectory();
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile(Path.Combine(path, "appsettings.json"))
            //    .Build();

            string outputTemplate = "{Timestamp:yyyy-MM-dd HH: mm: ss.fff} [{Level}] {Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                //.ReadFrom.Configuration(config)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .WriteTo.File(Path.Combine(envPath, "Logs/applog_.log"),
                rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                .Enrich.FromLogContext()
                //.WriteTo.Http("http://localhost:8080")
                .CreateLogger();

            //Log.Logger = new LoggerConfiguration()
            //       .Enrich.FromLogContext()
            //       //.WriteTo.Console()
            //       //.WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
            //       .WriteTo.File("/Logs/log.txt", rollingInterval: RollingInterval.Day)
            //       .CreateLogger();
            //var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {



                Log.Information("Application Started.");
                var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Log.Information($"Application running on environment {environmentVariable}");

                Log.Information($"Application version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}");

                if (environment.IsDevelopment() || environment.IsEnvironment("Release"))
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    Log.Information($"Database run migration");
                }

                host.Run();
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Stopped program because of exception");
                Log.Fatal(ex, "Application start-up failed");
               
            }
            finally
            {
                //LogManager.Shutdown();
                Log.CloseAndFlush();
            }


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            //.ConfigureLogging(logging =>
            //    {
            //        logging.ClearProviders();
            //        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            //    })
            //    .UseNLog();
            ;
    }
}
