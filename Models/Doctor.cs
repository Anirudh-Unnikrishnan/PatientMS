using System.ComponentModel.DataAnnotations;

namespace PatientMS.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [Required]
    [StringLength(100)]
    public string? Specialisation { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Years of Experience")]
    public int YearsOfExperience { get; set; }

    [Display(Name = "Profile Bio")]
    public string? Bio { get; set; }

    public bool IsAvailable { get; set; } = true;

    public ICollection<DoctorAvailability>? Availabilities { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
