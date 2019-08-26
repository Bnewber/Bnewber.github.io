using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DadJokeMania.Models.ApiModels;

namespace DadJokeMania.ViewModels
{
    public class DadJokeViewModel
    {
        public List<DadJokeModel> Jokes { get; set; }
    }
}