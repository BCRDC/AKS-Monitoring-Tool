using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
   internal  class LifetimeEventsHostedService: IHostedService
    {
        private readonly ILogger _logger;

        private static readonly List<string> Dimensions = new List<string>
        {
            "Cluster",
            "NodeName",
            "EngagementAccount",
            "SubscriptionId",
            "ServiceProvider"
        };


        private readonly MetricWrapper _wrapper = null;

        public LifetimeEventsHostedService(
        ILogger<LifetimeEventsHostedService> logger,
        IHostApplicationLifetime appLifetime)
        {
            _logger = logger;


            _wrapper = new MetricWrapper("CEFGlobalMDM", "GatewayMetrics", Dimensions.ToArray());



        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _wrapper.SendGaugeValue((DateTime.UtcNow.Second % 5) + 1, "BCCS_RequestSuccessCount", "dev", "bccsaccount", "evn", "ss", "ww" + DateTime.UtcNow.Second % 3);
                    _wrapper.SendHistogramValue((DateTime.UtcNow.Second % 9), "BCCS_RequestLatency", "dev", "bccsaccount", "evn", "ss", "ww" + DateTime.UtcNow.Second % 3);
                    await Task.Delay(1000 * 3);
                }
                

            });

            return Task.Delay(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }
    }
}
