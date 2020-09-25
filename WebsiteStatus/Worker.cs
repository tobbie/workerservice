using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace WebsiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTimeOffset.Now}");
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
             client.Dispose();
            _logger.LogInformation("Worker service stopped.......");
            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            while (!stoppingToken.IsCancellationRequested)
            { 
                stopwatch.Start();
                try
                {
                    var result = await client.GetAsync("http://www.gcccng.org");
                    if (result.StatusCode == System.Net.HttpStatusCode.OK) {
                        _logger.LogInformation($"GCCC website is up. Status Code {result.StatusCode}");
                    }
                    else{
                        _logger.LogInformation($"GCCC website is down. Status Code {result.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }

                finally
                {
                    await Task.Delay(2500, stoppingToken);

                    stopwatch.Stop();
                    
                    var timeTaken = (stopwatch.ElapsedMilliseconds);
                    _logger.LogInformation($"Process took a total of {timeTaken} milliseconds");

                    stopwatch.Reset();
                }                
            }
        }
    }
}
