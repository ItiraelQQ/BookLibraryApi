namespace BookLibraryAPI.Models
{
    public class GoogleBooksResponse
    {
        public List<Volume> Items { get; set; }
    }
    public class Volume
    {
        public VolumeInfo VolumeInfo { get; set; }
    }
    public class VolumeInfo 
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public ImageLinks ImageLinks { get; set; }
    }
    public class ImageLinks
    {
        public string Thumbnail { get; set; }
    }
}
