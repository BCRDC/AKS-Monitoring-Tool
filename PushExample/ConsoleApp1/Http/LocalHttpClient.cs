using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Http
{
    internal class LocalHttpClient
    {

        private static  HttpClient _client;
        public static HttpClient Instance { 
            get
            {
                if (_client == null) return new HttpClient(new LocalHandler());
                else
                {
                    return _client;
                }
            } 
        }
    }
}
