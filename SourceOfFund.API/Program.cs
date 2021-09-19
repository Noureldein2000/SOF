using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.File;
using SourceOfFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                //.WriteTo.Http("http://localhost:8080")
                .CreateLogger();

            //Log.Logger = new LoggerConfiguration()
            //       .Enrich.FromLogContext()
            //       //.WriteTo.Console()
            //       //.WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
            //       .WriteTo.File("/Logs/log.txt", rollingInterval: RollingInterval.Day)
            //       .CreateLogger();
            try
            {
                Log.Information("Application Started.");
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Log.Information($"Application running on environment {environment}");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                
    }
}
