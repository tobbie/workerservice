using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace WebsiteStatus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                              .MinimumLevel.Debug()
                              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                              .Enrich.FromLogContext()
                              .WriteTo.File("./LogFile.txt")
                              .WriteTo.Console(LogEventLevel.Information)
                              .CreateLogger();

            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem with the service");
                return;
            }

            finally {
                Log.CloseAndFlush();

            }
           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    
                }).UseSerilog();
    }
}
