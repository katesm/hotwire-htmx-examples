using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using hotwire_turbo_stimulus_demo.Models;

namespace hotwire_turbo_stimulus_demo.Controllers;

public class HomeController : Controller
{
    private readonly TaskStore _taskStore;

    public HomeController(TaskStore taskStore)
    {
        _taskStore = taskStore;
    }

    public IActionResult Index()
    {
        return View(_taskStore.GetAll());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Tasks()
    {
        return PartialView("_TaskList", _taskStore.GetAll());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTask(string title)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            _taskStore.Add(title);
        }

        return PartialView("_TaskList", _taskStore.GetAll());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleTask(int id)
    {
        
        var task = _taskStore.Toggle(id);
        if (task is null)
        {
            return NotFound();
        }

        return TurboStream("_TaskRow", task, $"task_{task.Id}");
    }

    private ContentResult TurboStream(string partialViewName, object model, string target)
    {
        ViewData.Model = model;
        var html = this.RenderViewToString(partialViewName, ViewData);
        var stream = $"<turbo-stream action=\"replace\" target=\"{target}\"><template>{html}</template></turbo-stream>";
        return Content(stream, "text/vnd.turbo-stream.html");
    }

    [HttpGet]
    public IActionResult HtmxTip()
    {
        string[] tips =
        [
            "Turbo Frames scope navigation so only the frame content reloads.",
            "HTMX lets any element trigger AJAX requests using plain HTML attributes.",
            "Stimulus connects lightweight JS controllers to server-rendered HTML.",
            "Turbo Streams let the server push targeted DOM mutations over HTTP or WebSockets.",
            "hx-boost progressively enhances links and forms, just like Turbo Drive.",
            "hx-swap-oob lets a single server response update multiple elements at once.",
        ];
        return PartialView("_HtmxTip", tips[Random.Shared.Next(tips.Length)]);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
