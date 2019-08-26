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
using DadJokeMania.Services;
using DadJokeMania.ViewModels;

namespace DadJokeMania.Controllers.api
{
    public class DadJokeController : Controller
    {
        GetJokes _getJokes;
        public static HttpClient _client;
        public DadJokeController()
        {
            _client = new HttpClient();
            _getJokes = new GetJokes(_client);
        }
        // GET: DadJoke/DadRandomJoke
        public ActionResult GetRandomJoke()
        {
            try
            {
                var joke = _getJokes.GetRandomDadJoke();
                if (joke.Id == "") { return Json(new { success = false, responseText = "Aww Snap something went wrong, please try again." }, JsonRequestBehavior.AllowGet); }

                return Json(new { success = true, Joke = new Models.ApiModels.DadJokeModel { Id = joke.Id, Joke = joke.Joke } }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(ServerError(), JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DadJoke/SearchForJokes
        public ActionResult SearchForJokes(string queryTerms, int page)
        {
            string[] terms = queryTerms.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Want to make sure the user provides a valid search term
            if (terms.Length == 0) { return Json(new { success = false, responseText = "Please provide a word to search for." }, JsonRequestBehavior.AllowGet); }
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

                return Json(new { success = true,
                                  Jokes = newJokes,
                                  currentPageNumber = jokes.Current_Page,
                                  totalPages = jokes.Total_Pages,
                                  nextPageNumber = jokes.Next_Page,
                                  previousPageNumber = jokes.Previous_Page
                                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(ServerError(), JsonRequestBehavior.AllowGet);
            }
        }

        public object ServerError()
        {
            return new { success = false, responseText = "Aww snap something went wrong, please reload the page and try again." };
        }
    }
}
