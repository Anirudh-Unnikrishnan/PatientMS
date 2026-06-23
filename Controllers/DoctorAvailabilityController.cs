using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;
using ClosedXML.Excel;

namespace PatientMS.Controllers
{
    [Authorize]
    public class DoctorAvailabilityController : Controller
    {
        private readonly PatientMSContext _context;

        public DoctorAvailabilityController(PatientMSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var availability = _context.DoctorAvailability.Include(d => d.Doctor);
            return View(await availability.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var availability = await _context.DoctorAvailability
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (availability == null) return NotFound();
            return View(availability);
        }

        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName");
            return View();
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId,DayOfWeek,StartTime,EndTime,BreakStart,BreakEnd,IsActive")] DoctorAvailability doctorAvailability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName", doctorAvailability.DoctorId);
            return View(doctorAvailability);
        }

        // Excel Upload
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction(nameof(Index));
            }

            var ext = Path.GetExtension(excelFile.FileName).ToLower();
            if (ext != ".xlsx" && ext != ".xls")
            {
                TempData["Error"] = "Only .xlsx or .xls files are allowed.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // skip header

                foreach (var row in rows)
                {
                    var doctorName = row.Cell(1).GetString();
                    var doctor = await _context.Doctor
                        .FirstOrDefaultAsync(d => d.FullName == doctorName);

                    if (doctor == null) continue;

                    var availability = new DoctorAvailability
                    {
                        DoctorId = doctor.Id,
                        DayOfWeek = row.Cell(2).GetString(),
                        StartTime = TimeSpan.Parse(row.Cell(3).GetString()),
                        EndTime = TimeSpan.Parse(row.Cell(4).GetString()),
                        BreakStart = string.IsNullOrEmpty(row.Cell(5).GetString())
                            ? null : TimeSpan.Parse(row.Cell(5).GetString()),
                        BreakEnd = string.IsNullOrEmpty(row.Cell(6).GetString())
                            ? null : TimeSpan.Parse(row.Cell(6).GetString()),
                        IsActive = true
                    };
                    _context.Add(availability);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Availability uploaded successfully from Excel.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error reading file: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var doctorAvailability = await _context.DoctorAvailability.FindAsync(id);
            if (doctorAvailability == null) return NotFound();
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName", doctorAvailability.DoctorId);
            return View(doctorAvailability);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,DayOfWeek,StartTime,EndTime,BreakStart,BreakEnd,IsActive")] DoctorAvailability doctorAvailability)
        {
            if (id != doctorAvailability.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorAvailabilityExists(doctorAvailability.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName", doctorAvailability.DoctorId);
            return View(doctorAvailability);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var doctorAvailability = await _context.DoctorAvailability
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctorAvailability == null) return NotFound();
            return View(doctorAvailability);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorAvailability = await _context.DoctorAvailability.FindAsync(id);
            if (doctorAvailability != null)
                _context.DoctorAvailability.Remove(doctorAvailability);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorAvailabilityExists(int id)
        {
            return _context.DoctorAvailability.Any(e => e.Id == id);
        }
    }
}