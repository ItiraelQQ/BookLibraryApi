using BookLibraryAPI;
using BookLibraryAPI.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;


namespace BookLibraryAPI.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleBooksService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleBooks:ApiKey"];
        }

        public async Task<GoogleBooksResponse> SearchBooksAsync(string query)
        {
            {
                var response = await _httpClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?q={query}&key={_apiKey}");
                
                response.EnsureSuccessStatusCode();
            
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GoogleBooksResponse>(content);
            }
        }
    }
}
