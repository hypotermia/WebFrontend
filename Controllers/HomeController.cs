using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebFrontEnd.Models;
using System.Text.Json;
using System.Net;
using System.Reflection;

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
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/login", login);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Task");
                }
                else
                {
                    return View(login);
                }
                
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
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel login)
        {
            try
            {
                var antiForgeryToken = HttpContext.Request.Form["__RequestVerificationToken"];
                _httpClient.DefaultRequestHeaders.Add("RequestVerificationToken", (string?)antiForgeryToken);
                var response = await _httpClient.PostAsJsonAsync("api/Users", login);

                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle registration failure
                    ModelState.AddModelError(string.Empty, "Registration failed. Please try again."); // You can customize the error message
                    return View(login);
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle connection issues
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View(login);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
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