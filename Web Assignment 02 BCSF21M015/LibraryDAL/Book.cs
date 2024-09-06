
namespace LibraryDAL
{
    public class Book
    {
        public int BookId { get; set; }
        // for empty string by default
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
