using System.ComponentModel.DataAnnotations;

namespace hotwire_turbo_stimulus_demo.Models;

public sealed class StudentFormInput
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name must be 50 characters or fewer.")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name must be 50 characters or fewer.")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [MaxLength(100, ErrorMessage = "Email must be 100 characters or fewer.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please select a course.")]
    [Display(Name = "Course")]
    public string? Course { get; set; }

    [Required(ErrorMessage = "Please select a year.")]
    [Range(1, 4, ErrorMessage = "Year must be between 1 and 4.")]
    [Display(Name = "Year")]
    public int? Year { get; set; }
}
