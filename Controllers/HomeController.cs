using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebFrontEnd.Models;
using System.Text.Json;
using System.Net;

namespace WebFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController(IHttpClientFactory ihttpClientFactory)
        {
            _httpClient = ihttpClientFactory.CreateClient();
            //tolong ganti URL yang ada di URI
            _httpClient.BaseAddress = new Uri("https://localhost:7113/");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel login)
        {
            //give up for register and login !!! 
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/login", login);
                response.EnsureSuccessStatusCode();
                    return RedirectToAction("Index","Task");
                
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View(login);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(login);
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(LoginViewModel login)
        {
            try
            {
                //tolong ganti URL sesuai dengan localhost sendiri
                string apiUrl = "https://localhost:7113/api/Users";

                var requestBody = new
                {
                    Username = login.username,
                    Password = login.password
                };

                var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody));
                jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                ViewBag.ShowSuccessModal = true;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View(login);
            }
            catch (JsonException ex)
            {
                ViewBag.ErrorMessage = $"Error serializing JSON: {ex.Message}";
                return View(login);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(login);
            }
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
        public bool UserIsAuthenticated()
        {
            return true;
        }
    }
}