using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientMS.Models;

namespace PatientMS.Data;

public class PatientMSContext : IdentityDbContext
{
    public PatientMSContext(DbContextOptions<PatientMSContext> options)
        : base(options) { }

    public DbSet<Patient> Patient { get; set; }
    public DbSet<Doctor> Doctor { get; set; }
    public DbSet<DoctorAvailability> DoctorAvailability { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<PatientReport> PatientReport { get; set; }
}
