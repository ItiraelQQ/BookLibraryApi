using System.ComponentModel.DataAnnotations;

namespace BookLibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public string Genre { get; set; }
        [Range(0, 2025, ErrorMessage = "Year must be between 0 and 2025")]
        public int Year { get; set; }
        public double Rating { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
