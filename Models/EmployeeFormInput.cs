using System.ComponentModel.DataAnnotations;

namespace hotwire_turbo_stimulus_demo.Models;

public sealed class EmployeeFormInput
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Employee number is required.")]
    [MaxLength(20, ErrorMessage = "Employee number must be 20 characters or fewer.")]
    [Display(Name = "Employee Number")]
    public string? EmployeeNumber { get; set; }

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

    [Required(ErrorMessage = "Department is required.")]
    [MaxLength(80, ErrorMessage = "Department must be 80 characters or fewer.")]
    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    [MaxLength(80, ErrorMessage = "Job title must be 80 characters or fewer.")]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Required(ErrorMessage = "Employment type is required.")]
    [Display(Name = "Employment Type")]
    public string? EmploymentType { get; set; }

    [Required(ErrorMessage = "Grade is required.")]
    [MaxLength(10, ErrorMessage = "Grade must be 10 characters or fewer.")]
    [Display(Name = "Grade")]
    public string? Grade { get; set; }

    [Required(ErrorMessage = "Campus is required.")]
    [MaxLength(80, ErrorMessage = "Campus must be 80 characters or fewer.")]
    [Display(Name = "Campus")]
    public string? Campus { get; set; }

    [Required(ErrorMessage = "Manager name is required.")]
    [MaxLength(100, ErrorMessage = "Manager name must be 100 characters or fewer.")]
    [Display(Name = "Manager")]
    public string? Manager { get; set; }

    [Required(ErrorMessage = "Please choose a start date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Status")]
    public string? Status { get; set; }
}
