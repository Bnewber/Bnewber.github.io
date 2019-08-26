using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using DadJokeMania.Models.ApiModels;
using Newtonsoft.Json;

namespace DadJokeMania.Clients
{
    public class GetJokes : JokeClient
    {
        /// <summary>
        /// Get random dad joke from icanhazdadjoke.com
        /// </summary>
        /// <returns>RandomDadJokeModel</returns>
        public RandomDadJokeModel GetRandomDadJoke()
        {
            RandomDadJokeModel randomJoke = new RandomDadJokeModel();
            using (var httpClient = this.Client)
            {
                var response = httpClient.GetAsync(this.BaseUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var stream = response.Content.ReadAsStreamAsync().Result;
                    using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
                    {
                        randomJoke = new JsonSerializer().Deserialize<RandomDadJokeModel>(jsonReader);
                    }
                }
            }
            return randomJoke;
        }

        /// <summary>
        /// Search for jokes from icanhazdadjokes.com
        /// </summary>
        /// <param name="queryTerms">String array of terms used to search for jokes</param>
        /// <param name="page">page number to get jokes from</param>
        /// <returns>SearchedDadJokesModel</returns>
        public SearchedDadJokesModel SearchForDadJokes(string[] queryTerms, int page)
        {
            SearchedDadJokesModel searchedJokes = new SearchedDadJokesModel();
            
            //Build query string params to pass
            string terms = queryTerms.Length > 0 ? "term=" : string.Empty;
            bool firstParam = true;
            foreach (var term in queryTerms)
            {
                if (firstParam)
                {
                    terms += term;
                    firstParam = false;
                }
                else
                {
                    terms += $"+{term}";
                }
            }
            string pageNumer = $"&page={page}";
            string relativeUri = $"search?{terms}&limit=30{pageNumer}";
            using (var httpClient = this.Client)
            {
                var response = httpClient.GetAsync(new Uri(this.BaseUri, relativeUri)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var stream = response.Content.ReadAsStreamAsync().Result;
                    using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
                    {
                        searchedJokes = new JsonSerializer().Deserialize<SearchedDadJokesModel>(jsonReader);
                    }
                }
            }
            return searchedJokes;
        }
    }
}