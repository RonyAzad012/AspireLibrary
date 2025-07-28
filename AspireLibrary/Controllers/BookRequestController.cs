using AspireLibrary.Data;
using AspireLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class BookRequestController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // For Members — List their own requests
    public async Task<IActionResult> MyRequests()
    {
        var user = await _userManager.GetUserAsync(User);
        var requests = await _context.BookRequests
            .Include(r => r.Book)
            .Where(r => r.UserId == user.Id)
            .ToListAsync();
        return View(requests);
    }

    // Request Book
    public async Task<IActionResult> RequestBook(int bookId)
    {
        var user = await _userManager.GetUserAsync(User);

        var request = new BookRequest
        {
            BookId = bookId,
            UserId = user.Id,
            RequestDate = DateTime.Now,
            Status = "Pending"
        };

        _context.BookRequests.Add(request);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyRequests");
    }

    // Admin view — all requests
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Index()
    {
        var requests = await _context.BookRequests
            .Include(r => r.Book)
            .Include(r => r.User)
            .ToListAsync();
        return View(requests);
    }

    // Approve Request
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _context.BookRequests.FindAsync(id);
        if (request != null && request.Status == "Pending")
        {
            request.Status = "Approved";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // Reject Request
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _context.BookRequests.FindAsync(id);
        if (request != null && request.Status == "Pending")
        {
            request.Status = "Rejected";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // Details view
    public async Task<IActionResult> Details(int id)
    {
        var request = await _context.BookRequests
            .Include(r => r.Book)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.BookRequestId == id);

        if (request == null) return NotFound();
        return View(request);
    }
}
