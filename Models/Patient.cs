using System.ComponentModel.DataAnnotations;

namespace PatientMS.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Medical History")]
    public string? MedicalHistory { get; set; }

    [Display(Name = "Date Registered")]
    public DateTime DateRegistered { get; set; } = DateTime.Now;

    public ICollection<Booking>? Bookings { get; set; }
    public ICollection<PatientReport>? Reports { get; set; }
}
