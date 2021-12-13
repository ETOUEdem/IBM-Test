
using Apim.WebClient.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Apim.WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
    
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string ReturnUrl)
        {
            var response = await GetResponse(username,password);
            var token = await GetToken(response);
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Append(CookieAuthenticationDefaults.AuthenticationScheme, token, cookieOptions);

            ViewBag.response = response.StatusCode;
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("TryApi");
            }
            else
            {
                return View();
            }

        }

        public async Task<HttpResponseMessage> GetResponse(string username, string password)
            {

            var AuthUrl = configuration["AuthUrl"];
              var pocoObject = new
            {
                Username = username,
                Password = password
            };

            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.
            string json = JsonConvert.SerializeObject(pocoObject);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            //The url to post to.

            var client = new HttpClient();

            //Pass in the full URL and the json string content
         return await client.PostAsync(AuthUrl, data);
         

        }

        public async Task<string> GetToken(HttpResponseMessage response)
        {
            string token = "";
            ////Pass in the full URL and the json string content
            //var response = await GetResponse(username, password);

            //It would be better to make sure this request actually made it through

            string responseBody = await response.Content.ReadAsStringAsync();


            if ((response.IsSuccessStatusCode))
            {
                token = await response.Content.ReadAsStringAsync();

            }
            return token;
        }

       
        public IActionResult TryApi()
        {
            var cookieValue = Request.Cookies[CookieAuthenticationDefaults.AuthenticationScheme];

   
            ViewBag.token = cookieValue;
            return View();
        }

        public IActionResult TryApiSend(Models.ApiViewModel api)
        {
            var result = "";
            var url = api.Url;

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Accept = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer "+api.Token;
            httpRequest.Headers["Ocp-Apim-Subscription-Key"] =  api.Subscriptionkey;


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
               result = streamReader.ReadToEnd();
            }
            return Json(result);

        }
    }
}
