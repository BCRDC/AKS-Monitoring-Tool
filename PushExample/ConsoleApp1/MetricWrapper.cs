﻿using ConsoleApp1.Http;
using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class MetricWrapper: IDisposable
    {
        private const string Url = "http://prom-gateway.chinaeast2.cloudapp.chinacloudapi.cn:8000/metrics";

        private CollectorRegistry _reg = new CollectorRegistry();


        private IMetricServer _metricServer;

        private MetricFactory _metricFactory = null;

        private ConcurrentDictionary<string, Gauge> _gaugeDic = new ConcurrentDictionary<string, Gauge>();

        private ConcurrentDictionary<string, Histogram> _histogramDic = new ConcurrentDictionary<string, Histogram>();

        private ConcurrentDictionary<string, Counter> _counterDic = new ConcurrentDictionary<string, Counter>();

        private ConcurrentDictionary<string, Summary> _summaryDic = new ConcurrentDictionary<string, Summary>();

        private string[] _labels = null;

        static MetricWrapper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public MetricWrapper(string category, string item, params string[] labelNames)
        {

            _metricFactory = Metrics.WithCustomRegistry(_reg);

            _metricServer = new MetricPusher(new MetricPusherOptions
            {
                Endpoint = Url,
                Job = category,
                Instance = item,
                // AdditionalLabels = new[] { new Tuple<string, string>("t1", "t2")},
                Registry = _reg,
                OnError = ex =>
                {
                    Debug.WriteLine(ex.Message);
                },
                HttpClientProvider = () => LocalHttpClient.Instance
            });

            _labels = labelNames;
            _metricServer.Start();
        }


        public bool SendGaugeValue(double value, string name, params string[] labelNames)
        {
            if (labelNames == null || _labels == null)
            {
                return false;
            }

            if (labelNames != null && _labels != null &&  labelNames.Count() != _labels.Count())
            {
                return false;
            }

            var key = name + "__" + string.Join("__", labelNames);

            // var dic = labelNames.to

            var gauge = _gaugeDic.GetOrAdd(key, _key => 
            { 

                return _metricFactory.CreateGauge(name, name, new GaugeConfiguration
                {
                    LabelNames = _labels
                });
            });

            gauge.WithLabels(labelNames).Set(value);

            return true;
        }



        public bool SendHistogramValue(double value, string name, params string[] labelNames)
        {
            if (labelNames == null || _labels == null)
            {
                return false;
            }

            if (labelNames != null && _labels != null && labelNames.Count() != _labels.Count())
            {
                return false;
            }

            var key = name + "__" + string.Join("__", labelNames);

            // var dic = labelNames.to

            var gauge = _histogramDic.GetOrAdd(key, _key =>
            {

                return _metricFactory.CreateHistogram(name, name, new HistogramConfiguration
                {
                    LabelNames = _labels,
                    Buckets = Histogram.LinearBuckets(start: 1, width: 1, count: 40)
                });
            });

            gauge.WithLabels(labelNames).Observe(value);

            return true;
        }


        public bool SendCounterValue(double value, string name, params string[] labelNames)
        {
            if (labelNames == null || _labels == null)
            {
                return false;
            }

            if (labelNames != null && _labels != null && labelNames.Count() != _labels.Count())
            {
                return false;
            }

            var key = name + "__" + string.Join("__", labelNames);

            // var dic = labelNames.to

            var gauge = _counterDic.GetOrAdd(key, _key =>
            {

                return _metricFactory.CreateCounter(name, name, new CounterConfiguration
                {
                    LabelNames = _labels,
                });
            });

            gauge.WithLabels(labelNames).Inc(value);

            return true;
        }

        public bool SendSummaryValue(double value, string name, params string[] labelNames)
        {
            if (labelNames == null || _labels == null)
            {
                return false;
            }

            if (labelNames != null && _labels != null && labelNames.Count() != _labels.Count())
            {
                return false;
            }

            var key = name + "__" + string.Join("__", labelNames);

            // var dic = labelNames.to

            var summary = _summaryDic.GetOrAdd(key, _key =>
            {

                return _metricFactory.CreateSummary(name, name, new SummaryConfiguration
                {
                    LabelNames = _labels,
                    //Objectives = new[]
                    //    {
                    //        new QuantileEpsilonPair(0.5, 0.05),
                    //        new QuantileEpsilonPair(0.9, 0.05),
                    //        new QuantileEpsilonPair(0.95, 0.01),
                    //        new QuantileEpsilonPair(0.99, 0.005),
                    //    }
                });
            });

            summary.WithLabels(labelNames).Observe(value);

            return true;
        }


        public void Dispose()
        {
            _metricServer?.Dispose();
        }
    }
}
