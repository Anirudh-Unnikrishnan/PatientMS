using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;

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
        var currentUserEmail = User.Identity?.Name;
        var isDoctor = User.IsInRole("Doctor");

        // Find current doctor record if logged in as Doctor
        Doctor? currentDoctor = null;
        if (isDoctor && currentUserEmail != null)
        {
            currentDoctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email != null &&
                    d.Email.ToLower() == currentUserEmail.ToLower());
        }

        // Get bookings filtered by doctor if needed
        var bookingsQuery = _context.Booking
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .AsQueryable();

        if (isDoctor && currentDoctor != null)
            bookingsQuery = bookingsQuery.Where(b => b.DoctorId == currentDoctor.Id);

        var bookings = await bookingsQuery.ToListAsync();
        var now = DateTime.Now;

        var events = bookings.Select(b =>
        {
            var status = (b.Status ?? "").Trim();

            // Combine appointment date and time into one DateTime
            var appointmentDateTime = b.AppointmentDate.Date.Add(b.AppointmentTime);

            // Auto mark as Completed if appointment time has passed
            // and it wasn't manually cancelled
            if (appointmentDateTime < now && status != "Cancelled")
                status = "Completed";

            string color;
            if (status == "Completed")
                color = "#38a169"; // green
            else if (status == "Cancelled")
                color = "#e53e3e"; // red
            else
                color = "#4fd1c5"; // teal for Scheduled

            return new
            {
                id = b.Id.ToString(),
                title = $"{b.Patient!.FullName} — Dr. {b.Doctor!.FullName}",
                start = appointmentDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = color,
                extendedProps = new
                {
                    status = status,
                    reason = b.ReasonForVisit ?? "Not specified",
                    doctor = b.Doctor.FullName,
                    patient = b.Patient.FullName,
                    isBreak = false
                }
            };
        }).ToList();

        // Get break events filtered by doctor if needed
        var availabilityQuery = _context.DoctorAvailability
            .Include(a => a.Doctor)
            .Where(a => a.BreakStart != null && a.BreakEnd != null && a.IsActive)
            .AsQueryable();

        if (isDoctor && currentDoctor != null)
            availabilityQuery = availabilityQuery
                .Where(a => a.DoctorId == currentDoctor.Id);

        var availabilities = await availabilityQuery.ToListAsync();
        var today = DateTime.Today;
        var breakEvents = new List<object>();

        foreach (var a in availabilities)
        {
            for (int i = 0; i < 28; i++)
            {
                var date = today.AddDays(i);
                var calendarDay = date.DayOfWeek.ToString().Trim().ToLower();
                var availDay = (a.DayOfWeek ?? "").Trim().ToLower();

                if (calendarDay == availDay)
                {
                    breakEvents.Add(new
                    {
                        id = $"break-{a.Id}-{i}",
                        title = $"Break — Dr. {a.Doctor!.FullName}",
                        start = date.Add(a.BreakStart!.Value)
                            .ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = date.Add(a.BreakEnd!.Value)
                            .ToString("yyyy-MM-ddTHH:mm:ss"),
                        color = "#d69e2e",
                        extendedProps = new
                        {
                            status = "Break",
                            reason = "Doctor is on break",
                            doctor = a.Doctor.FullName,
                            patient = "",
                            isBreak = true
                        }
                    });
                }
            }
        }

        return Json(events.Cast<object>().Concat(breakEvents));
    }
}