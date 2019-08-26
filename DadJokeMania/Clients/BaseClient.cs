using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DadJokeMania.Clients
{
    public abstract class BaseClient : IClient
    {
        public HttpClient Client;
        public Uri BaseUri;

        public BaseClient(string url)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
            {
                Client = new HttpClient();
                BaseUri = new Uri(url);
                HttpClientHandler handler = new HttpClientHandler();
                Client = new HttpClient(handler, false);
            }
        }

        /// <summary>
        /// User this method to configure headers and timeout for the client
        /// </summary>
        public virtual void ConfigureClient()
        {
            Client.BaseAddress = BaseUri;
            Client.Timeout = new TimeSpan(0, 0, 30);// Timeout in 30 seconds
        }
    }
}