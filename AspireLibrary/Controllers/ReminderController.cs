using AspireLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspireLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReminderController : Controller
    {
        private readonly EmailReminderService _emailReminderService;

        public ReminderController(EmailReminderService emailReminderService)
        {
            _emailReminderService = emailReminderService;
        }

        [HttpPost]
        public async Task<IActionResult> SendReminders()
        {
            await _emailReminderService.SendLateReturnRemindersAsync();
            return Json(new { success = true, message = "Reminders sent successfully." });
        }
    }

}
