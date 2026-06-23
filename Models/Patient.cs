using System.ComponentModel.DataAnnotations;

namespace PatientMS.Models;

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^\+?[0-9]{8,15}$",
        ErrorMessage = "Phone must be 8-15 digits, optionally starting with +.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address (e.g. name@example.com).")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage = "Enter a valid email address (e.g. name@example.com).")]
    public string? Email { get; set; }

    [Display(Name = "Medical History")]
    public string? MedicalHistory { get; set; }

    [Display(Name = "Date Registered")]
    public DateTime DateRegistered { get; set; } = DateTime.Now;

    public ICollection<Booking>? Bookings { get; set; }
    public ICollection<PatientReport>? Reports { get; set; }
}