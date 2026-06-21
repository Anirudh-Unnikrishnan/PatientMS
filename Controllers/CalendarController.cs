using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;

namespace PatientMS.Controllers;

public class CalendarController : Controller
{
    private readonly PatientMSContext _context;

    public CalendarController(PatientMSContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetEvents()
    {
        var bookings = await _context.Booking
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .ToListAsync();

        var events = bookings.Select(b => new
        {
            id = b.Id,
            title = $"{b.Patient!.FullName} - Dr. {b.Doctor!.FullName}",
            start = b.AppointmentDate.Date.Add(b.AppointmentTime)
                .ToString("yyyy-MM-ddTHH:mm:ss"),
            color = b.Status == "Scheduled" ? "#1B6CA8" :
                    b.Status == "Completed" ? "#28a745" : "#dc3545"
        });

        return Json(events);
    }
}