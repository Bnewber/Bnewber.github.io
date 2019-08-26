using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DadJokeMania.Models.ApiModels;
using DadJokeMania.Clients;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Mvc.RouteAttribute;

namespace DadJokeMania.Controllers.api
{
    public class DadJokeController : ApiController
    {
        GetJokes _getJokes;
        public DadJokeController()
        {
            _getJokes = new GetJokes();
        }
        // GET: DadJoke/DadRandomJoke
        [Route("api/DadJoke/RandomJoke")]
        [HttpGet]
        public IHttpActionResult RandomJoke()
        {
            try
            {
                var joke = _getJokes.GetRandomDadJoke();
                if (joke.Id == "") { return ServerError(); }

                return Ok(new { success = true, Joke = new Models.ApiModels.DadJokeModel { Id = joke.Id, Joke = joke.Joke } });
            }
            catch
            {
                return ServerError();
            }
        }

        // GET: DadJoke/SearchForJokes
        [Route("api/DadJoke/SearchForJokes")]
        [HttpGet]
        public IHttpActionResult SearchForJokes(string queryTerms, int page)
        {
            string[] terms = queryTerms.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Want to make sure the user provides a valid search term
            if (terms.Length == 0) { return Ok(new { success = false, responseText = "Please provide a word to search for." }); }
            try
            {
                var jokes = _getJokes.SearchForDadJokes(terms, page);
                var newJokes = new List<DadJokeModel>();

                foreach (var joke in jokes.Results)
                {
                    string[] jokeSentance = joke.Joke.Split(' ');
                    bool foundWord = false;
                    for (int i = 0; i < jokeSentance.Length; i++)//Loop through each word in the joke 
                    {
                        for (int x = 0; x < terms.Length; x++)//foreach word loop through the search terms and see if we get a hit
                        {
                            var firstIndex = jokeSentance[i].IndexOf(terms[x], 0, StringComparison.OrdinalIgnoreCase);
                            if (firstIndex != -1)
                            {
                                foundWord = true;
                                string word = jokeSentance[i].Substring(firstIndex, terms[x].Length);

                                jokeSentance[i] = jokeSentance[i].Replace(word, $"<strong>{word}</strong>");
                            }
                        }
                    }
                    string newJoke = string.Empty;
                    if (foundWord)
                    {
                        foreach (var word in jokeSentance)
                        {
                            newJoke += $" {word}";
                        }
                    }
                    newJokes.Add(new DadJokeModel { Id = joke.Id, Joke = foundWord ? newJoke : joke.Joke });
                }

                return Ok(new { success = true,
                                  Jokes = newJokes,
                                  currentPageNumber = jokes.Current_Page,
                                  totalPages = jokes.Total_Pages,
                                  nextPageNumber = jokes.Next_Page,
                                  previousPageNumber = jokes.Previous_Page
                                });
            }
            catch
            {
                return ServerError();
            }
        }

        public IHttpActionResult ServerError()
        {
            return Ok(new { success = false, responseText = "Aww snap something went wrong, please reload the page and try again." });
        }
    }
}
