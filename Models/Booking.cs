using System.ComponentModel.DataAnnotations;

namespace PatientMS.Models;

public class Booking
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    [Required]
    [Display(Name = "Appointment Date")]
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Display(Name = "Appointment Time")]
    [DataType(DataType.Time)]
    public TimeSpan AppointmentTime { get; set; }

    public string Status { get; set; } = "Scheduled";

    [Display(Name = "Reason for Visit")]
    public string? ReasonForVisit { get; set; }

    [Display(Name = "Doctor Notes")]
    public string? DoctorNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
