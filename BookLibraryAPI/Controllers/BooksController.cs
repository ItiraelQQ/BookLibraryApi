﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookLibraryAPI.Models;
using BookLibraryAPI.Mappings;
using Microsoft.AspNetCore.Authorization;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GoogleBooksService _googleBooksService;

        public BooksController(AppDbContext context, GoogleBooksService googleBooksService)
        {
            _context = context;
            _googleBooksService = googleBooksService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(string author = null, string genre = null, int? yearFrom = null, int? YearTo = null, string sortBy = null)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.Contains(genre.ToLower()));
            }

            if (yearFrom.HasValue)
            {
                query = query.Where(b => b.Year >= yearFrom.Value);
            }

            if (YearTo.HasValue)
            {
                query = query.Where(b => b.Year <= YearTo.Value);
            }

            query = sortBy switch
            {
                "title" => query.OrderBy(b => b.Title),
                "year" => query.OrderBy(b => b.Year),
                "author" => query.OrderBy(b => b.Author),
                _ => query.OrderBy(b => b.Id),
            };
            return await query.ToListAsync();
        }

        

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        // POST: api/Books
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Genre = bookDto.Genre,
                Year = bookDto.Year
            };

            var googleBook = await _googleBooksService.SearchBooksAsync(bookDto.Title);

            if (googleBook.Items != null && googleBook.Items.Count > 0)
            {
                var volumeInfo = googleBook.Items[0].VolumeInfo;
                book.PosterUrl = volumeInfo.ImageLinks?.Thumbnail;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else throw;
            }
            return NoContent();
        }

        // DELETE: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/rating")]
        public async Task<IActionResult> UpdateRating(int id, double rating)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Rating = rating;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

