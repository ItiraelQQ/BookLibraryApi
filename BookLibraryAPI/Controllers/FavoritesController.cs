using BookLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Controllers
{
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
        _context = context; 
        }

        // Добавление книги в избранное
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Favorite>> AddToFavorite(Favorite favorite)
        {
            var existingFavorite = await _context.Favorites
                .SingleOrDefaultAsync(f => f.UserId == favorite.UserId && f.BookId == favorite.BookId);
        
            if (existingFavorite != null)
            {
                return Conflict("Book is already in favorites.");
            } 

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFavoritesByUser), new {userId = favorite.UserId}, favorite);
        }

        // Получение списка избранных книг пользователя
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavoritesByUser(int userId)
        {
            var favorites = await _context.Favorites
                .Include(f => f.Book)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return Ok(favorites);
        }

        // Удаление книги из избранного
        [Authorize]
        [HttpDelete("{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFromFavorite(int userId, int bookId)
        {
            var favorite = await _context.Favorites
                .SingleOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);
        
            if (favorite == null)
                return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
