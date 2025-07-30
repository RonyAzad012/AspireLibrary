using AspireLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AspireLibrary.Controllers
{
    [Authorize(Roles = "Admin, Librarian, User")]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var fineSettings = await _context.FineSettings.FirstOrDefaultAsync();

            var events = await _context.BorrowRecords
                .Where(b => b.ReturnDate == null)
                .Include(b => b.Book)
                .Include(b => b.User)
                .Select(b => new
                {
                    title = b.Book.Title + " (" + b.User.FullName + ")",
                    start = b.BorrowDate.AddDays(14).ToString("yyyy-MM-dd"),
                    allDay = true,
                    isOverdue = fineSettings != null && DateTime.Now > b.BorrowDate.AddDays(14)
                })
                .ToListAsync();

            return Json(events);
        }

    }
}
