using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Desafio.Models
{
    public class Data
    {
        public List<string> suspects { get; set; }
        public List<string> locals { get; set; }
        public List<string> guns { get; set; }
    }

    public class Responses
    {
        public List<Response> responses { get; set; }
    }

    public class Response
    {
        public string suspect { get; set; }
        public string local { get; set; }
        public string gun { get; set; }
    }
}