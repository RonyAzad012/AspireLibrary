using AspireLibrary.Data;

namespace AspireLibrary.Models
{
    public class BookRequest
    {
        public int BookRequestId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public DateTime RequestDate { get; set; }
    }
}
