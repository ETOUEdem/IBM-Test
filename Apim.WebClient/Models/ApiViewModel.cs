using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apim.WebClient.Models
{
    public class ApiViewModel
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Token { get; set; }
        public string Subscriptionkey { get; set; }
    }
}
