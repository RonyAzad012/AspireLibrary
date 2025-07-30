using AspireLibrary.Data;
using AspireLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AspireLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FineSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FineSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fine = await _context.FineSettings.FirstOrDefaultAsync();
            return View(fine);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var setting = await _context.FineSettings.FindAsync(id);
            if (setting == null)
                return NotFound();
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FineSettings settings)
        {
            if (ModelState.IsValid)
            {
                _context.Update(settings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(settings);
        }

        public async Task<IActionResult> Create()
        {
            var existing = await _context.FineSettings.AnyAsync();
            if (existing) return RedirectToAction(nameof(Index));
            var model = new FineSettings { FinePerDay = 5, MaxFine = 100 };
            _context.FineSettings.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

