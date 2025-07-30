using AspireLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AspireLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminDashboardController(ApplicationDbContext context,
                                        UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalBooks = await _context.Books.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalBorrowed = await _context.BorrowRecords.CountAsync();
            var totalFinesCollected = await _context.BorrowRecords.SumAsync(b => b.FineAmount);

            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var librarianUsers = await _userManager.GetUsersInRoleAsync("Librarian");
            var memberUsers = await _userManager.GetUsersInRoleAsync("Member");

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalBorrowed = totalBorrowed;
            ViewBag.TotalFines = totalFinesCollected;

            ViewBag.AdminCount = adminUsers.Count;
            ViewBag.LibrarianCount = librarianUsers.Count;
            ViewBag.MemberCount = memberUsers.Count;

            return View();
        }
    }


}
