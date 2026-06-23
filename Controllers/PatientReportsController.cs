using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;

namespace PatientMS.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PatientReportsController : Controller
    {
        private readonly PatientMSContext _context;

        public PatientReportsController(PatientMSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reports = _context.PatientReport.Include(p => p.Patient);
            return View(await reports.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var report = await _context.PatientReport
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientReport report, List<IFormFile> uploadedFiles)
        {
            var allowedTypes = new[]
            {
                ".pdf", ".jpg", ".jpeg", ".png",
                ".doc", ".docx", ".xls", ".xlsx",
                ".txt", ".csv", ".ppt", ".pptx"
            };

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "uploads", "reports");
            Directory.CreateDirectory(uploadPath);

            var fileNames = new List<string>();
            var filePaths = new List<string>();

            foreach (var file in uploadedFiles)
            {
                if (file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedTypes.Contains(ext))
                    {
                        ModelState.AddModelError("",
                            $"{file.FileName} is not an allowed file type.");
                        ViewData["PatientId"] = new SelectList(
                            _context.Patient, "Id", "FullName");
                        return View(report);
                    }

                    var uniqueName = Guid.NewGuid().ToString() + ext;
                    var fullPath = Path.Combine(uploadPath, uniqueName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    fileNames.Add(file.FileName);
                    filePaths.Add("/uploads/reports/" + uniqueName);
                }
            }

            report.FileName = string.Join(";", fileNames);
            report.FilePath = string.Join(";", filePaths);
            report.FileType = "MULTI";
            report.UploadDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName");
            return View(report);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var report = await _context.PatientReport.FindAsync(id);
            if (report == null) return NotFound();
            ViewData["PatientId"] = new SelectList(
                _context.Patient, "Id", "FullName", report.PatientId);
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,PatientId,Title,Description,FileName,FilePath,FileType,UploadDate,UploadedBy")]
            PatientReport patientReport)
        {
            if (id != patientReport.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientReportExists(patientReport.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(
                _context.Patient, "Id", "FullName", patientReport.PatientId);
            return View(patientReport);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var report = await _context.PatientReport
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.PatientReport.FindAsync(id);
            if (report != null) _context.PatientReport.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientReportExists(int id)
        {
            return _context.PatientReport.Any(e => e.Id == id);
        }
    }
}