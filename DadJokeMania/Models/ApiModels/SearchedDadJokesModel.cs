using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DadJokeMania.Models.ApiModels
{
    public class SearchedDadJokesModel
    {
        public int Current_Page { get; set; }
        public int Limit { get; set; }
        public int Next_Page { get; set; }
        public int Previous_Page { get; set; }
        public List<DadJokeModel> Results { get; set; }
        public string Search_Term { get; set; }
        public int Status { get; set; }
        public int Total_Jokes { get; set; }
        public int Total_Pages { get; set; }
    }
}