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

    public IActionResult Index()
    {
        return View(_store.GetAll());
    }

    [HttpGet]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(StudentFormInput input)
    {
        if (!ModelState.IsValid)
            return PartialView("_StudentForm", input);

        _store.Add(input.FirstName!, input.LastName!, input.Email!, input.Course!, input.Year!.Value);

        Response.Headers["HX-Retarget"] = "#student-table";
        Response.Headers["HX-Reswap"]   = "outerHTML";
        Response.Headers["HX-Trigger"]  = "closeModal";
        return PartialView("_StudentTable", _store.GetAll());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, StudentFormInput input)
    {
        // Ensure the Id in the model matches the route so re-render targets the right action
        input.Id = id;

        if (!ModelState.IsValid)
            return PartialView("_StudentForm", input);

        var updated = _store.Update(id, input.FirstName!, input.LastName!, input.Email!, input.Course!, input.Year!.Value);
        if (updated is null) return NotFound();

        Response.Headers["HX-Retarget"] = "#student-table";
        Response.Headers["HX-Reswap"]   = "outerHTML";
        Response.Headers["HX-Trigger"]  = "closeModal";
        return PartialView("_StudentTable", _store.GetAll());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _store.Delete(id);
        return Content("");
    }
}
