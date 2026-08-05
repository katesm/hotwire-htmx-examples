using Microsoft.AspNetCore.Mvc;
using hotwire_turbo_stimulus_demo.Models;

namespace hotwire_turbo_stimulus_demo.Controllers;

public class StudentsController : Controller
{
    private readonly StudentStore _store;

    public StudentsController(StudentStore store)
    {
        _store = store;
    }

    [HttpGet("/Students")]
    public IActionResult Index()
    {
        return View(_store.GetAll());
    }

    [HttpGet("/Students/Form")]
    public IActionResult Form(int? id)
    {
        if (id.HasValue)
        {
            var student = _store.GetById(id.Value);
            if (student is null) return NotFound();
            return PartialView("_StudentForm", new StudentFormInput
            {
                Id        = student.Id,
                FirstName = student.FirstName,
                LastName  = student.LastName,
                Email     = student.Email,
                Course    = student.Course,
                Year      = student.Year,
            });
        }
        return PartialView("_StudentForm", new StudentFormInput());
    }

    [HttpPost("/Students/Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(StudentFormInput input)
    {
        if (!ModelState.IsValid)
            return PartialView("_StudentForm", input);

        _store.Add(input.FirstName!, input.LastName!, input.Email!, input.Course!, input.Year!.Value);

        Response.Headers["HX-Trigger-After-Settle"] = "closeModal";
        return PartialView("_StudentDashboardUpdate", _store.GetAll());
    }

    [HttpPost("/Students/Edit")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(StudentFormInput input)
    {
        if (!input.Id.HasValue)
            return NotFound();

        if (!ModelState.IsValid)
            return PartialView("_StudentForm", input);

        var updated = _store.Update(input.Id.Value, input.FirstName!, input.LastName!, input.Email!, input.Course!, input.Year!.Value);
        if (updated is null) return NotFound();

        Response.Headers["HX-Trigger-After-Settle"] = "closeModal";
        return PartialView("_StudentDashboardUpdate", _store.GetAll());
    }

    [HttpPost("/Students/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _store.Delete(id);
        return PartialView("_StudentDashboardUpdate", _store.GetAll());
    }
}
