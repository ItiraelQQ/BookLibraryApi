namespace BookLibraryAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; }
        public string Comment { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
