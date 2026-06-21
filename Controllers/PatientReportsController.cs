using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var patientMSContext = _context.PatientReport.Include(p => p.Patient);
            return View(await patientMSContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReport
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patientReport == null)
            {
                return NotFound();
            }

            return View(patientReport);
        }
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientReport report, IFormFile? uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var allowedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(uploadedFile.FileName).ToLower();

                if (!allowedTypes.Contains(extension))
                {
                    ModelState.AddModelError("", "Only PDF, JPG, and PNG files are allowed.");
                    return View(report);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "reports", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await uploadedFile.CopyToAsync(stream);

                report.FileName = uploadedFile.FileName;
                report.FilePath = "/uploads/reports/" + fileName;
                report.FileType = extension.Replace(".", "").ToUpper();
            }

            report.UploadDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReport.FindAsync(id);
            if (patientReport == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName", patientReport.PatientId);
            return View(patientReport);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,Title,Description,FileName,FilePath,FileType,UploadDate,UploadedBy")] PatientReport patientReport)
        {
            if (id != patientReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientReportExists(patientReport.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName", patientReport.PatientId);
            return View(patientReport);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReport
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patientReport == null)
            {
                return NotFound();
            }

            return View(patientReport);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientReport = await _context.PatientReport.FindAsync(id);
            if (patientReport != null)
            {
                _context.PatientReport.Remove(patientReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientReportExists(int id)
        {
            return _context.PatientReport.Any(e => e.Id == id);
        }
    }
}
