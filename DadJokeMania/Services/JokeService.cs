using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DadJokeMania.Services
{
    public class JokeService : IService  
    {
        public HttpClient Client;
        public Uri BaseUri;

        public JokeService(HttpClient client)
        {
            ConfigureService(client);
        }

        public void ConfigureService(HttpClient client)
        {
            Client = client;
            BaseUri = new Uri("https://icanhazdadjoke.com/");
            HttpClientHandler handler = new HttpClientHandler();
            Client = new HttpClient(handler, false);
            Client.BaseAddress = BaseUri;
            Client.DefaultRequestHeaders.Add("User-Agent", "Testing For Interview");
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.Timeout = new TimeSpan(0, 0, 30);// Timeout in 30 seconds
        }
    }
}