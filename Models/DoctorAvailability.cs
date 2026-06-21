using System.ComponentModel.DataAnnotations;
namespace PatientMS.Models;

public class DoctorAvailability
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    [Required]
    [Display(Name = "Day of Week")]
    public string? DayOfWeek { get; set; }

    [Required]
    [Display(Name = "Start Time")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Display(Name = "End Time")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; } = true;
}
