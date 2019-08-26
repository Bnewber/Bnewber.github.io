using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DadJokeMania.Clients
{
    public class JokeClient : BaseClient  
    {
        static string apiUrl = ConfigurationManager.AppSettings["ICanHazDadJokesAPI"].ToString();
        public JokeClient():base(apiUrl)
        {
            ConfigureClient();
        }

        public override void ConfigureClient()
        {
            Client.DefaultRequestHeaders.Add("User-Agent", "Testing For Interview");
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.Timeout = new TimeSpan(0, 0, 30);// Timeout in 30 seconds
        }
    }
}