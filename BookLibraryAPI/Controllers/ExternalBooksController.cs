using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalBooksController : ControllerBase
    {
        private readonly GoogleBooksService _googleBooksService;

        public ExternalBooksController(GoogleBooksService googleBooksService)
        {
            _googleBooksService = googleBooksService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<GoogleBooksResponse>> SearchBooks(string query)
        {
            var books = await _googleBooksService.SearchBooksAsync(query);
            return Ok(books);
        }
    }
}
