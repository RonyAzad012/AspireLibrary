using AspireLibrary.Data;

namespace AspireLibrary.Models
{
    public class BorrowRecord
    {
        public int BorrowRecordId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal FineAmount { get; set; }
    }
}
