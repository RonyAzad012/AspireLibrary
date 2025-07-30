using AspireLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace AspireLibrary.Services
{
    public class EmailReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailReminderService> _logger;
        private readonly IConfiguration _config;

        public EmailReminderService(ApplicationDbContext context, ILogger<EmailReminderService> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        public async Task SendLateReturnRemindersAsync()
        {
            var fineSettings = await _context.FineSettings.FirstOrDefaultAsync();
            if (fineSettings == null)
            {
                _logger.LogWarning("FineSettings not configured.");
                return;
            }

            var overdueRecords = await _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .Where(b => b.ReturnDate == null && b.BorrowDate.AddDays(14) < DateTime.Now)
                .ToListAsync();

            foreach (var record in overdueRecords)
            {
                try
                {
                    int daysLate = (DateTime.Now - record.BorrowDate.AddDays(14)).Days;
                    decimal fine = Math.Min(daysLate * fineSettings.FinePerDay, fineSettings.MaxFine);

                    // Optionally update fine in DB
                    record.FineAmount = fine;
                    _context.BorrowRecords.Update(record);
                    await _context.SaveChangesAsync();

                    // Send email
                    await SendEmailAsync(record.User.Email, "Library Book Return Reminder", $@"
                    Dear {record.User.FullName},<br/>
                    You have an overdue book: <strong>{record.Book.Title}</strong>.<br/>
                    It is {daysLate} day(s) late. Your current fine is ৳{fine}.<br/>
                    Please return the book as soon as possible.<br/><br/>
                    Thank you,<br/>
                    Library Management System
                ");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send reminder for BorrowRecord {record.Id}");
                }
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:FromEmail"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            using var message = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
