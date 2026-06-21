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
    public class DoctorAvailabilityController : Controller
    {
        private readonly PatientMSContext _context;

        public DoctorAvailabilityController(PatientMSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var patientMSContext = _context.DoctorAvailability.Include(d => d.Doctor);
            return View(await patientMSContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAvailability = await _context.DoctorAvailability
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctorAvailability == null)
            {
                return NotFound();
            }

            return View(doctorAvailability);
        }

        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId,DayOfWeek,StartTime,EndTime,IsActive")] DoctorAvailability doctorAvailability)
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAvailability = await _context.DoctorAvailability.FindAsync(id);
            if (doctorAvailability == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName", doctorAvailability.DoctorId);
            return View(doctorAvailability);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,DayOfWeek,StartTime,EndTime,IsActive")] DoctorAvailability doctorAvailability)
        {
            if (id != doctorAvailability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorAvailabilityExists(doctorAvailability.Id))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "FullName", doctorAvailability.DoctorId);
            return View(doctorAvailability);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAvailability = await _context.DoctorAvailability
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctorAvailability == null)
            {
                return NotFound();
            }

            return View(doctorAvailability);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorAvailability = await _context.DoctorAvailability.FindAsync(id);
            if (doctorAvailability != null)
            {
                _context.DoctorAvailability.Remove(doctorAvailability);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorAvailabilityExists(int id)
        {
            return _context.DoctorAvailability.Any(e => e.Id == id);
        }
    }
}
