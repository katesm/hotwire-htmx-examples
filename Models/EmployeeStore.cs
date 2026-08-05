namespace hotwire_turbo_stimulus_demo.Models;

public sealed class EmployeeStore
{
    private readonly object _sync = new();
    private readonly List<Employee> _employees =
    [
        new(1, "UOXF-00428", "Maya", "Singh", "maya.singh@university.ac.uk", "Student Registry", "Senior Registry Officer", "Permanent", "G6", "North Campus", "Dr Alan Hsu", new DateOnly(2021, 3, 15), "Active"),
        new(2, "UOXF-00461", "Liam", "Bennett", "liam.bennett@university.ac.uk", "Finance", "Payroll Analyst", "Permanent", "G5", "Central Campus", "Rachel Moore", new DateOnly(2022, 9, 5), "Active"),
        new(3, "UOXF-00512", "Fatima", "Khan", "fatima.khan@university.ac.uk", "HR", "Talent Partner", "Fixed Term", "G7", "South Campus", "Helen Ward", new DateOnly(2023, 1, 9), "Onboarding"),
        new(4, "UOXF-00397", "Ethan", "Cole", "ethan.cole@university.ac.uk", "IT Services", "Systems Administrator", "Permanent", "G6", "North Campus", "Priya Nair", new DateOnly(2020, 6, 22), "Active"),
        new(5, "UOXF-00544", "Noor", "Rahman", "noor.rahman@university.ac.uk", "Procurement", "Contracts Officer", "Part Time", "G5", "West Campus", "James Yates", new DateOnly(2024, 2, 1), "Probation")
    ];

    private int _nextId = 6;

    public IReadOnlyList<Employee> GetAll()
    {
        lock (_sync) return _employees.ToList();
    }

    public Employee? GetById(int id)
    {
        lock (_sync) return _employees.FirstOrDefault(e => e.Id == id);
    }

    public Employee Add(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string department,
        string jobTitle,
        string employmentType,
        string grade,
        string campus,
        string manager,
        DateOnly startDate,
        string status)
    {
        lock (_sync)
        {
            var employee = new Employee(
                _nextId++,
                employeeNumber.Trim(),
                firstName.Trim(),
                lastName.Trim(),
                email.Trim(),
                department.Trim(),
                jobTitle.Trim(),
                employmentType,
                grade.Trim().ToUpperInvariant(),
                campus.Trim(),
                manager.Trim(),
                startDate,
                status);

            _employees.Add(employee);
            return employee;
        }
    }

    public Employee? Update(
        int id,
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string department,
        string jobTitle,
        string employmentType,
        string grade,
        string campus,
        string manager,
        DateOnly startDate,
        string status)
    {
        lock (_sync)
        {
            var index = _employees.FindIndex(e => e.Id == id);
            if (index < 0) return null;

            var employee = new Employee(
                id,
                employeeNumber.Trim(),
                firstName.Trim(),
                lastName.Trim(),
                email.Trim(),
                department.Trim(),
                jobTitle.Trim(),
                employmentType,
                grade.Trim().ToUpperInvariant(),
                campus.Trim(),
                manager.Trim(),
                startDate,
                status);

            _employees[index] = employee;
            return employee;
        }
    }

    public bool Delete(int id)
    {
        lock (_sync)
        {
            var index = _employees.FindIndex(e => e.Id == id);
            if (index < 0) return false;

            _employees.RemoveAt(index);
            return true;
        }
    }
}
