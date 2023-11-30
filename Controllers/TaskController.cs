using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebFrontEnd.Models;

namespace WebFrontEnd.Controllers
{
    public class TaskController : Controller
    {
        private readonly HttpClient _httpClient;
        public TaskController(IHttpClientFactory ihttpClientFactory)
        {

            _httpClient = ihttpClientFactory.CreateClient();
            //tolong ganti URL yang ada di URI
            _httpClient.BaseAddress = new Uri("https://localhost:7113/");
        }
        public ActionResult Create() 
        { 
            return View(); 
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Task/{id}");

                response.EnsureSuccessStatusCode(); // Ensure the response is successful (status code 2xx)

                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize the API response into a UserModel
                TaskModel task = JsonConvert.DeserializeObject<TaskModel>(responseData);

                return View(task);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network issues)
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Task/{id}");
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                TaskModel user = JsonConvert.DeserializeObject<TaskModel>(responseData);

                return View(user);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network issues)
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Task/{id}");
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                ViewBag.ShowSuccessModal = true;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Task/{id}");

                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                TaskModel task = JsonConvert.DeserializeObject<TaskModel>(responseData);
                return View(task);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TaskModel editedTask)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Task/{editedTask.Id}", editedTask);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                ViewBag.ShowSuccessModal = true;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            { 
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View(editedTask); 
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(editedTask);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskModel task)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Task", task);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                ViewBag.ShowSuccessModal = true;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to the API: {ex.Message}";
                return View(task);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(task);
            }
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                List<TaskModel> tasks = await GetTaskData();
                if(tasks != null) { return View(tasks); }
                
                else
                {
                    ViewBag.ErrorMessage = "No user data received from the API.";
                    return View(new List<TaskModel>());
                }
            }
            catch
            {
                ViewBag.ErrorMessage = "An error occurred while fetching user data from the API.";
                return View(new List<TaskModel>()); 
            }
        }
        private async Task<List<TaskModel>> GetTaskData()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Task");
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseData))
                {
                    List<TaskModel> task = JsonConvert.DeserializeObject<List<TaskModel>>(responseData);
                    return task;
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException)
            {
                // Rethrow the exception to be handled in the calling method
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception in GetUserDataFromApi: {ex}");
                return null;
            }
        }
    }
}
