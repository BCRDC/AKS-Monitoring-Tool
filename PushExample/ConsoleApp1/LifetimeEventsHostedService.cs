using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
   internal  class LifetimeEventsHostedService: IHostedService
    {
        private readonly ILogger _logger;


        private Gauge UsersLoggedIn = null;

        private  CollectorRegistry _reg = new CollectorRegistry();



        public LifetimeEventsHostedService(
        ILogger<LifetimeEventsHostedService> logger,
        IHostApplicationLifetime appLifetime)
        {
            _logger = logger;

            _reg.SetStaticLabels(new Dictionary<string, string>
{
  // Labels applied to all metrics in the registry.
  { "environment", "testing" }
});



        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
          var factory =   Metrics.WithCustomRegistry(_reg);

            UsersLoggedIn = factory.CreateGauge("myapp_users_logged_in", "Number of active user sessions",
            new GaugeConfiguration
            {
            SuppressInitialValue = true
            });

            // throw new NotImplementedException();
            var pusher = new MetricPusher(new MetricPusherOptions
            {
                Endpoint = "http://prom-gateway.chinaeast2.cloudapp.chinacloudapi.cn:8000/metrics",
                Job = "some_job1",
                Instance = "Instance1",
                AdditionalLabels = new[] { new Tuple<string, string>("t1", "t2")},
                Registry = _reg,
                OnError = ex =>
                {
                    Console.WriteLine(ex.Message);
                }
            });

            pusher.Start();


            UsersLoggedIn.Set(1);
            UsersLoggedIn.Publish();
            return Task.Delay(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }
    }
}
