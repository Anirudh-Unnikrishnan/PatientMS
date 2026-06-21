using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;

namespace PatientMS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PatientMSContext _context;

    public HomeController(ILogger<HomeController> logger, PatientMSContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["PatientCount"] = await _context.Patient.CountAsync();
        ViewData["DoctorCount"] = await _context.Doctor.CountAsync();
        ViewData["TodayBookings"] = await _context.Booking
            .Where(b => b.AppointmentDate.Date == DateTime.Today)
            .CountAsync();
        ViewData["PendingReports"] = await _context.PatientReport.CountAsync();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}