using hotwire_turbo_stimulus_demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace hotwire_turbo_stimulus_demo.Controllers;

public sealed class EmployeesController : Controller
{
    private readonly EmployeeStore _store;

    public EmployeesController(EmployeeStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_store.GetAll());
    }

    [HttpGet]
    public IActionResult Form(int? id)
    {
        if (id.HasValue)
        {
            var employee = _store.GetById(id.Value);
            if (employee is null) return NotFound();

            return PartialView("_EmployeeModalFrame", new EmployeeFormInput
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Department = employee.Department,
                JobTitle = employee.JobTitle,
                EmploymentType = employee.EmploymentType,
                Grade = employee.Grade,
                Campus = employee.Campus,
                Manager = employee.Manager,
                StartDate = employee.StartDate,
                Status = employee.Status
            });
        }

        return PartialView("_EmployeeModalFrame", new EmployeeFormInput());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EmployeeFormInput input)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EmployeeModalFrame", input);
        }

        _store.Add(
            input.EmployeeNumber!,
            input.FirstName!,
            input.LastName!,
            input.Email!,
            input.Department!,
            input.JobTitle!,
            input.EmploymentType!,
            input.Grade!,
            input.Campus!,
            input.Manager!,
            input.StartDate!.Value,
            input.Status!);

        return EmployeeDashboardTurboStream(closeModal: true);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, EmployeeFormInput input)
    {
        input.Id = id;

        if (!ModelState.IsValid)
        {
            return PartialView("_EmployeeModalFrame", input);
        }

        var updated = _store.Update(
            id,
            input.EmployeeNumber!,
            input.FirstName!,
            input.LastName!,
            input.Email!,
            input.Department!,
            input.JobTitle!,
            input.EmploymentType!,
            input.Grade!,
            input.Campus!,
            input.Manager!,
            input.StartDate!.Value,
            input.Status!);

        if (updated is null) return NotFound();

        return EmployeeDashboardTurboStream(closeModal: true);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _store.Delete(id);
        return EmployeeDashboardTurboStream(closeModal: false);
    }


    private ContentResult EmployeeDashboardTurboStream(bool closeModal)
    {
        return TurboStreamResponse(
            TurboStream("replace", "employee-table-container", RenderPartial("_EmployeeTable", _store.GetAll())),
            closeModal ? TurboStream("update", "modal-content", string.Empty) : null
        );
    }

    private ContentResult TurboStreamResponse(params string?[] streams)
    {
        var payload = string.Concat(streams.Where(static stream => !string.IsNullOrWhiteSpace(stream)));
        return Content(payload, "text/vnd.turbo-stream.html");
    }

    private static string TurboStream(string action, string target, string html)
    {
        return $"<turbo-stream action=\"{action}\" target=\"{target}\"><template>{html}</template></turbo-stream>";
    }


    private string RenderPartial(string viewName, object model)
    {
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        return this.RenderViewToString(viewName, viewData);
    }
}
