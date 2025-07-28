using System.ComponentModel.DataAnnotations;

namespace AspireLibrary.Models
{
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string CoverImage { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

}
