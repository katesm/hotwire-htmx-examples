namespace hotwire_turbo_stimulus_demo.Models;

public sealed record Employee(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string JobTitle,
    string EmploymentType,
    string Grade,
    string Campus,
    string Manager,
    DateOnly StartDate,
    string Status);
