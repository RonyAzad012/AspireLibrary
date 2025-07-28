using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using AspireLibrary.Data;
using AspireLibrary.Models;

[Authorize(Roles = "Admin,Librarian")]
public class BorrowRecordController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BorrowRecordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // View all borrowed books
    public async Task<IActionResult> Index()
    {
        var records = await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .ToListAsync();
        return View(records);
    }

    // Issue a book to a member
    public IActionResult Issue()
    {
        ViewBag.Members = _userManager.Users.ToList();
        ViewBag.Books = _context.Books.Where(b => b.AvailableCopies > 0).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(BorrowRecord record)
    {
        if (ModelState.IsValid)
        {
            record.BorrowDate = DateTime.Now;
            record.FineAmount = 0;

            var book = await _context.Books.FindAsync(record.BookId);
            if (book != null && book.AvailableCopies > 0)
            {
                book.AvailableCopies--;
                _context.BorrowRecords.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        ViewBag.Members = _userManager.Users.ToList();
        ViewBag.Books = _context.Books.Where(b => b.AvailableCopies > 0).ToList();
        return View(record);
    }

    // Return a borrowed book
    public async Task<IActionResult> Return(int id)
    {
        var record = await _context.BorrowRecords.Include(b => b.Book).Include(b => b.User).FirstOrDefaultAsync(b => b.BorrowRecordId == id);
        if (record == null || record.ReturnDate != null)
            return NotFound();

        record.ReturnDate = DateTime.Now;

        // Optional fine logic
        var daysLate = (record.ReturnDate.Value - record.BorrowDate).Days - 14; // 14-day borrow limit
        if (daysLate > 0)
        {
            var fineRate = _context.FineSettings.FirstOrDefault();
            if (fineRate != null)
            {
                record.FineAmount = Math.Min(daysLate * fineRate.FinePerDay, fineRate.MaxFine);

                // Send email notification for fine
                await SendLateReturnEmail(record.User.Email, record.User.FullName, record.Book.Title, record.FineAmount);
            }
        }

        record.Book.AvailableCopies++;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // View details of a borrow record
    public async Task<IActionResult> Details(int id)
    {
        var record = await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BorrowRecordId == id);

        if (record == null) return NotFound();
        return View(record);
    }

    private async Task SendLateReturnEmail(string toEmail, string fullName, string bookTitle, decimal fineAmount)
    {
        var fromAddress = new MailAddress("yourlibraryemail@example.com", "Library Management");
        var toAddress = new MailAddress(toEmail, fullName);
        const string subject = "Late Book Return Notice";
        string body = $"Dear {fullName},\n\nYou have returned the book '{bookTitle}' late. A fine of {fineAmount:C} has been added to your account.\n\nPlease pay it at your earliest convenience.\n\nThank you.";

        var smtp = new SmtpClient
        {
            Host = "smtp.your-email-provider.com",
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential("yourlibraryemail@example.com", "your-email-password")
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            await smtp.SendMailAsync(message);
        }
    }
}
