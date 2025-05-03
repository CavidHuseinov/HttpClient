using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using System.Text.Json;

namespace Client.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpClientController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _data;

        public HttpClientController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _data = config["Data:Resource"];
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var allData = await _httpClient.GetAsync(_data);
                allData.EnsureSuccessStatusCode();
                var content = await allData.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Api elimiz catmir. Sistem xetasi: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(string userId, string body, string title)
        {
            var request = new
            {
                userId = userId,
                body = body,
                title = title
            };
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_data, content);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"xarici api yeni melumat elave etmek mumkun olmadi. Server xetasi: {ex.Message}");
            }

        }
        [HttpDelete("delete-foreign-data/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var content = await _httpClient.DeleteAsync($"{_data}/{id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Silmek mumkun olmadi. Server xetasi {ex.Message}");
            }
        }
        [HttpPut("update-foreign-data")]
        public async Task<IActionResult> UpdateAsync(string userId, string body, string title, int id)
        {
            var request = new
            {
                userId = userId,
                body = body,
                title = title
            };
            try
            {

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_data}/{id}", content);
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Api yenilemek mumkun olmadi. Server xetasi: {ex.Message}");
            }
        }

    }
}
