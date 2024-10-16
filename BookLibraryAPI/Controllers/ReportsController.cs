using Microsoft.AspNetCore.Mvc;


namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Отчет по кол-ву добавленных книг
        [HttpGet("booksCount")]
        public ActionResult<int> GetBooksCount()
        {
            var count = _context.Books.Count();
            return Ok(count);
        }

        // Отчет по популярным жанрам
        [HttpGet("popularGenres")]
        public ActionResult<IEnumerable<string>> GetPopularGenres()
        {
            var popularGenres = _context.Books
                .GroupBy(b => b.Genre)
                .OrderByDescending(g => g.Count())
                .Select(g => new {Genre = g.Key, Count = g.Count()})
                .ToList();

            return Ok(popularGenres);   
        }

        // Отчет по популярным авторам
        [HttpGet("popularAuthors")]
        public ActionResult<IEnumerable<string>> GetPopularAuthors()
        {
            var popularAuthors = _context.Books
                .GroupBy(b => b.Author)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Author = g.Key, Count = g.Count() })
                .ToList();

            return Ok(popularAuthors);
        }
    }
}
