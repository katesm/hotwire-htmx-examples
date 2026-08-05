namespace hotwire_turbo_stimulus_demo.Models;

public sealed class StudentStore
{
    private readonly object _sync = new();
    private readonly List<Student> _students =
    [
        new(1, "Alice",  "Johnson", "alice.johnson@uni.ac.uk",  "Computer Science", 2),
        new(2, "Ben",    "Patel",   "ben.patel@uni.ac.uk",      "Engineering",       1),
        new(3, "Clara",  "Osei",    "clara.osei@uni.ac.uk",     "Medicine",          3),
        new(4, "David",  "Kim",     "david.kim@uni.ac.uk",      "Mathematics",       4),
        new(5, "Elena",  "Vasquez", "elena.vasquez@uni.ac.uk",  "Law",               2),
    ];
    private int _nextId = 6;

    public IReadOnlyList<Student> GetAll()
    {
        lock (_sync) return _students.ToList();
    }

    public Student? GetById(int id)
    {
        lock (_sync) return _students.FirstOrDefault(s => s.Id == id);
    }

    public Student Add(string firstName, string lastName, string email, string course, int year)
    {
        lock (_sync)
        {
            var student = new Student(_nextId++, firstName.Trim(), lastName.Trim(), email.Trim(), course, year);
            _students.Add(student);
            return student;
        }
    }

    public Student? Update(int id, string firstName, string lastName, string email, string course, int year)
    {
        lock (_sync)
        {
            var idx = _students.FindIndex(s => s.Id == id);
            if (idx < 0) return null;
            var updated = new Student(id, firstName.Trim(), lastName.Trim(), email.Trim(), course, year);
            _students[idx] = updated;
            return updated;
        }
    }

    public bool Delete(int id)
    {
        lock (_sync)
        {
            var idx = _students.FindIndex(s => s.Id == id);
            if (idx < 0) return false;
            _students.RemoveAt(idx);
            return true;
        }
    }
}
