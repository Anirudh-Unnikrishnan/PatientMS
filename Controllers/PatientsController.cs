using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;
using PatientMS.Models;

namespace PatientMS.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly PatientMSContext _context;

        public PatientsController(PatientMSContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patient.ToListAsync());
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var patient = await _context.Patient.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,DateOfBirth,Gender,PhoneNumber,Email,MedicalHistory,DateRegistered")] Patient patient)
        {
            // Server-side duplicate checks
            if (!string.IsNullOrEmpty(patient.Email) &&
                await _context.Patient.AnyAsync(p => p.Email == patient.Email))
                ModelState.AddModelError("Email", "This email is already registered.");

            if (!string.IsNullOrEmpty(patient.PhoneNumber) &&
                await _context.Patient.AnyAsync(p => p.PhoneNumber == patient.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");

            if (ModelState.IsValid)
            {
                patient.DateRegistered = DateTime.Now;
                _context.Add(patient);
                await _context.SaveChangesAsync();

                // Redirect based on role
                if (User.IsInRole("Admin") || User.IsInRole("Receptionist") || User.IsInRole("Doctor"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Confirmation");
            }
            return View(patient);
        }

        // Shown to regular users after registering
        public IActionResult Confirmation()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth,Gender,PhoneNumber,Email,MedicalHistory,DateRegistered")] Patient patient)
        {
            if (id != patient.Id) return NotFound();

            if (!string.IsNullOrEmpty(patient.Email) &&
                await _context.Patient.AnyAsync(p => p.Email == patient.Email && p.Id != patient.Id))
                ModelState.AddModelError("Email", "This email is already registered.");

            if (!string.IsNullOrEmpty(patient.PhoneNumber) &&
                await _context.Patient.AnyAsync(p => p.PhoneNumber == patient.PhoneNumber && p.Id != patient.Id))
                ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var patient = await _context.Patient.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient != null) _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.Id == id);
        }
    }
}