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

        // If logged in as Doctor, find their Doctor record by email
        Doctor? currentDoctor = null;
        if (isDoctor && currentUserEmail != null)
        {
            currentDoctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email != null &&
                    d.Email.ToLower() == currentUserEmail.ToLower());
        }

        // Get bookings — filter by doctor if logged in as Doctor
        var bookingsQuery = _context.Booking
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .AsQueryable();

        if (isDoctor && currentDoctor != null)
        {
            bookingsQuery = bookingsQuery
                .Where(b => b.DoctorId == currentDoctor.Id);
        }

        var bookings = await bookingsQuery.ToListAsync();

        var events = bookings.Select(b =>
        {
            var status = (b.Status ?? "").Trim();

            string color;
            if (status == "Completed")
                color = "#38a169";
            else if (status == "Cancelled")
                color = "#e53e3e";
            else
                color = "#4fd1c5";

            return new
            {
                id = b.Id.ToString(),
                title = $"{b.Patient!.FullName} — Dr. {b.Doctor!.FullName}",
                start = b.AppointmentDate.Date.Add(b.AppointmentTime)
                    .ToString("yyyy-MM-ddTHH:mm:ss"),
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

        // Get break events — filter by doctor if logged in as Doctor
        var availabilityQuery = _context.DoctorAvailability
            .Include(a => a.Doctor)
            .Where(a => a.BreakStart != null && a.BreakEnd != null && a.IsActive)
            .AsQueryable();

        if (isDoctor && currentDoctor != null)
        {
            availabilityQuery = availabilityQuery
                .Where(a => a.DoctorId == currentDoctor.Id);
        }

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
                    // Validate break times are reasonable
                    // (not midnight/1am which means data was saved incorrectly)
                    var breakStart = a.BreakStart!.Value;
                    var breakEnd = a.BreakEnd!.Value;

                    breakEvents.Add(new
                    {
                        id = $"break-{a.Id}-{i}",
                        title = $"Break — Dr. {a.Doctor!.FullName}",
                        start = date.Add(breakStart)
                            .ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = date.Add(breakEnd)
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