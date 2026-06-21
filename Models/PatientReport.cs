using System.ComponentModel.DataAnnotations;

namespace PatientMS.Models;

public class PatientReport
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    [Required]
    [Display(Name = "Report Title")]
    public string? Title { get; set; }

    [Display(Name = "Report Description")]
    public string? Description { get; set; }

    [Display(Name = "File Name")]
    public string? FileName { get; set; }

    [Display(Name = "File Path")]
    public string? FilePath { get; set; }

    [Display(Name = "File Type")]
    public string? FileType { get; set; }

    [Display(Name = "Upload Date")]
    public DateTime UploadDate { get; set; } = DateTime.Now;

    [Display(Name = "Uploaded By")]
    public string? UploadedBy { get; set; }
}
