# Hotwire Cheatsheet for ASP.NET Core MVC

A practical quick reference for developers who are new to **Hotwire** and
are working with **ASP.NET Core MVC**, **Razor Views/Partial Views**, and
**Tailwind CSS**.

Hotwire is a collection of browser tools for building server-rendered,
HTML-over-the-wire applications:

- **Turbo Drive**: navigate between pages without full-page reloads.
- **Turbo Frames**: update one region of a page independently.
- **Turbo Streams**: apply one or more DOM operations from an HTML response.
- **Stimulus**: add small, organized JavaScript controllers where browser
  behavior is needed.

------------------------------------------------------------------------

## 1. The Hotwire Mental Model

Instead of returning JSON and asking JavaScript to build the UI, MVC returns
HTML and Hotwire updates the relevant part of the document.

``` text
Browser interaction
        |
        v
Turbo request
        |
        v
MVC controller
        |
        v
Razor View or Partial View
        |
        v
Turbo updates the document
```

The key idea is:

> **MVC renders the HTML. Turbo transports and swaps the HTML. Stimulus
> handles behavior that genuinely belongs in the browser.**

------------------------------------------------------------------------

## 2. The Hotwire Pieces

| Tool | Primary job | Typical MVC response |
| --- | --- | --- |
| Turbo Drive | Enhance normal links and forms | Full HTML page |
| Turbo Frames | Scope navigation to one frame | A matching `<turbo-frame>` |
| Turbo Streams | Make targeted DOM changes | `text/vnd.turbo-stream.html` |
| Stimulus | Add client-side behavior | JavaScript controller |

Use the smallest tool that fits the interaction. A modal loaded into a frame
usually does not need a stream; a create action that updates a table and stats
panel can return multiple streams in one response.

------------------------------------------------------------------------

## 3. Turbo Drive

Turbo Drive intercepts normal links and form submissions, fetches the next
page, and replaces the document body without a complete browser reload.

``` html
<a href="/Employees">Employees</a>
```

The server can return a normal full page:

``` csharp
[HttpGet("/Employees")]
public IActionResult Index()
{
    return View(_store.GetAll());
}
```

Turbo Drive is normally enabled when Turbo is loaded. Opt out of it for a
specific element when a browser-native navigation is required:

``` html
<a href="/reports/export"
   data-turbo="false">
    Download report
</a>
```

Use `turbo:load` instead of only `DOMContentLoaded` for page setup. Turbo
navigation can replace the body while the document itself remains alive.

------------------------------------------------------------------------

## 4. Turbo Frames

A Turbo Frame limits navigation to one named region.

``` html
<turbo-frame id="modal-content">
</turbo-frame>
```

A link can target that frame:

``` html
<a href="/Employees/Form"
   data-turbo-frame="modal-content">
    Add Employee
</a>
```

The MVC action returns markup containing the same frame:

``` csharp
[HttpGet("/Employees/Form")]
public IActionResult Form(int? id)
{
    var model = id.HasValue
        ? BuildInputFromEmployee(id.Value)
        : new EmployeeFormInput();

    return PartialView("_EmployeeModalFrame", model);
}
```

The partial should preserve the frame boundary:

``` html
<turbo-frame id="modal-content">
    <form method="post" action="/Employees/Create">
        <!-- fields -->
        <button type="submit">Save</button>
    </form>
</turbo-frame>
```

### Frame rules

- The response should contain a frame with the requested frame ID.
- Only the matching frame is replaced.
- Links and forms inside a frame stay scoped to that frame by default.
- Use `data-turbo-frame="_top"` to escape the frame and navigate the page.

``` html
<a href="/Employees"
   data-turbo-frame="_top">
    Return to employees
</a>
```

------------------------------------------------------------------------

## 5. Loading a Modal with a Frame

A common MVC pattern is an empty frame inside a dialog:

``` html
<dialog id="modal">
    <turbo-frame id="modal-content"></turbo-frame>
</dialog>
```

The trigger only needs to target the frame:

``` html
<a href="/Employees/Form"
   data-turbo-frame="modal-content">
    Add Employee
</a>
```

Turbo fetches the form partial and replaces the frame contents. JavaScript
can listen for the frame lifecycle event and open the dialog:

``` javascript
document.addEventListener("turbo:frame-load", (event) => {
    if (event.target.id !== "modal-content") return;

    const modal = document.getElementById("modal");
    if (modal && !modal.open) modal.showModal();
});
```

Keep modal cleanup in `turbo:before-cache` so an open dialog is not cached
and restored on a later Turbo Drive visit.

------------------------------------------------------------------------

## 6. Turbo Streams

A Turbo Stream is an HTML instruction that targets an element by ID.

``` html
<turbo-stream action="replace" target="employee-table-container">
    <template>
        <div id="employee-table-container">
            <!-- updated table -->
        </div>
    </template>
</turbo-stream>
```

A response may contain several stream elements. This is useful when one
server action changes multiple parts of the page.

``` html
<turbo-stream action="replace" target="employee-stats-container">
    <template><!-- updated stats --></template>
</turbo-stream>
<turbo-stream action="replace" target="employee-table-container">
    <template><!-- updated table --></template>
</turbo-stream>
```

### Common stream actions

| Action | Effect |
| --- | --- |
| `append` | Add content after the target's children |
| `prepend` | Add content before the target's children |
| `replace` | Replace the target element |
| `update` | Replace the target element's contents |
| `remove` | Remove the target element |
| `before` | Insert content immediately before the target |
| `after` | Insert content immediately after the target |

Example row deletion:

``` html
<turbo-stream action="remove" target="employee-42">
</turbo-stream>
```

------------------------------------------------------------------------

## 7. Returning Streams from ASP.NET Core MVC

ASP.NET Core MVC does not have a built-in `TurboStream()` helper. Return the
stream markup as a `ContentResult` with Turbo's media type:

``` csharp
private ContentResult TurboStreamResponse(params string[] streams)
{
    return Content(
        string.Concat(streams),
        "text/vnd.turbo-stream.html");
}
```

A small helper keeps individual actions readable:

``` csharp
private static string TurboStream(
    string action,
    string target,
    string html)
{
    return $"<turbo-stream action=\"{action}\" target=\"{target}\">" +
           $"<template>{html}</template></turbo-stream>";
}
```

A create action can update several targets:

``` csharp
[HttpPost("/Employees/Create")]
[ValidateAntiForgeryToken]
public IActionResult Create(EmployeeFormInput input)
{
    if (!ModelState.IsValid)
    {
        return PartialView("_EmployeeModalFrame", input);
    }

    _store.Add(/* validated values */);
    var employees = _store.GetAll();

    return TurboStreamResponse(
        TurboStream("replace", "employee-stats-container",
            RenderPartial("_EmployeeStats", employees)),
        TurboStream("replace", "employee-table-container",
            RenderPartial("_EmployeeTable", employees)),
        TurboStream("update", "modal-content", string.Empty));
}
```

The partial returned by a `replace` action should include the target element's
ID because the target itself is being replaced.

------------------------------------------------------------------------

## 8. Turbo Forms

Turbo enhances ordinary HTML forms. MVC model binding works normally:

``` html
<form method="post" action="/Employees/Create">
    @Html.AntiForgeryToken()

    <label for="firstName">First name</label>
    <input id="firstName" name="FirstName" />

    <label for="lastName">Last name</label>
    <input id="lastName" name="LastName" />

    <button type="submit">Create</button>
</form>
```

On validation failure, return the form partial with its validation state:

``` csharp
if (!ModelState.IsValid)
{
    return PartialView("_EmployeeModalFrame", input);
}
```

On success, return a Turbo Stream response when several page regions need to
change. Do not return JSON merely because the request was asynchronous.

------------------------------------------------------------------------

## 9. Anti-Forgery Protection

Turbo submits normal form fields, so include the ASP.NET Core anti-forgery
token in forms protected by `[ValidateAntiForgeryToken]`:

``` html
<form method="post" action="/Employees/Delete/42">
    @Html.AntiForgeryToken()
    <button type="submit">Delete</button>
</form>
```

Do not disable anti-forgery protection to make Turbo requests work. If a
non-form request needs a token, configure the client and server to use the
same header or form-field convention.

------------------------------------------------------------------------

## 10. Stimulus Controllers

Stimulus connects HTML elements to small JavaScript controllers. A controller
is identified by `data-controller`:

``` html
<div data-controller="counter">
    <input data-counter-target="count" value="0" readonly />

    <button type="button"
            data-action="click->counter#decrement">
        -
    </button>
    <button type="button"
            data-action="click->counter#increment">
        +
    </button>
    <button type="button"
            data-action="click->counter#reset">
        Reset
    </button>
</div>
```

The controller declares targets and actions:

``` javascript
import {
    Application,
    Controller
} from "https://cdn.jsdelivr.net/npm/@hotwired/stimulus@3.2.2/+esm";

class CounterController extends Controller {
    static targets = ["count"];

    increment() {
        this.countTarget.value = Number(this.countTarget.value) + 1;
    }

    decrement() {
        this.countTarget.value = Number(this.countTarget.value) - 1;
    }

    reset() {
        this.countTarget.value = 0;
    }
}

const application = Application.start();
application.register("counter", CounterController);
```

### Stimulus naming

| HTML attribute | Controller property |
| --- | --- |
| `data-controller="counter"` | `CounterController` |
| `data-counter-target="count"` | `this.countTarget` |
| `data-action="click->counter#increment"` | `increment()` |

Stimulus controllers are reusable behavior units. They should coordinate DOM
behavior, not become a replacement for server-side application state.

------------------------------------------------------------------------

## 11. Stimulus Lifecycle Methods

Use lifecycle methods when a controller needs setup or cleanup:

``` javascript
connect() {
    this.element.addEventListener("keydown", this.handleKeydown);
}

disconnect() {
    this.element.removeEventListener("keydown", this.handleKeydown);
}
```

Useful methods include:

- `initialize()`: runs when the controller instance is created.
- `connect()`: runs when its element enters the document.
- `disconnect()`: runs when its element leaves the document.

Turbo can replace portions of the DOM, so cleanup in `disconnect()` matters
for event listeners, observers, and timers.

------------------------------------------------------------------------

## 12. Turbo and Stimulus Events

Turbo emits browser events that are useful for coordinating behavior:

| Event | Use |
| --- | --- |
| `turbo:load` | Run setup after a Turbo page visit |
| `turbo:frame-load` | React after a frame finishes loading |
| `turbo:before-stream-render` | Inspect or customize a stream before render |
| `turbo:submit-end` | React after a form submission completes |
| `turbo:before-cache` | Clean transient UI before Turbo caches a page |

Example:

``` javascript
document.addEventListener("turbo:submit-end", (event) => {
    if (!event.detail.success) return;

    const form = event.target;
    if (form.closest("#modal-content")) {
        document.getElementById("modal")?.close();
    }
});
```

Prefer a Stimulus controller for behavior belonging to one component. Use a
document-level listener for application-wide Turbo lifecycle concerns.

------------------------------------------------------------------------

## 13. Choosing Frames or Streams

Use a **Turbo Frame** when the interaction has one natural update boundary:

- Load an edit form into a modal.
- Replace a details panel.
- Navigate pages inside a tab or section.

Use **Turbo Streams** when one request changes multiple targets:

- Update a table and its summary counts.
- Append a new row and clear a form.
- Remove a record and update pagination or totals.

It is fine for one workflow to use both: a frame can host the form, and a
successful submission can return streams that update the page outside the
frame.

------------------------------------------------------------------------

## 14. Partial View Organization

Organize partials around the UI fragments they render:

``` text
Views/
└── Employees/
    ├── Index.cshtml
    ├── _EmployeeModalFrame.cshtml
    ├── _EmployeeForm.cshtml
    ├── _EmployeeTable.cshtml
    ├── _EmployeeRow.cshtml
    └── _EmployeeStats.cshtml
```

A useful convention is:

- Full page navigation returns `View("Index", model)`.
- Frame navigation returns a partial containing the matching frame.
- Stream actions return one or more `<turbo-stream>` elements.
- Validation failures return the form partial with `ModelState` intact.

Keep each fragment responsible for the element IDs that other streams target.

------------------------------------------------------------------------

## 15. HTTP Status Codes

Hotwire uses normal HTTP responses. Use status codes that describe the
result:

| Status | Typical use |
| --- | --- |
| `200 OK` | HTML page, frame, or stream rendered successfully |
| `201 Created` | A resource was created, when a body or location is useful |
| `204 No Content` | Successful action with no HTML to render |
| `400 Bad Request` | Malformed or invalid request |
| `403 Forbidden` | Anti-forgery or authorization failure |
| `404 Not Found` | Requested record does not exist |
| `422 Unprocessable Content` | Validation failed, if used by the app |

For a validation error where the form should be shown again, return the form
partial with an appropriate error status if your client behavior expects it.

------------------------------------------------------------------------

## 16. Troubleshooting Checklist

When a Hotwire interaction is not working, check these in order:

1. Is Turbo loaded, and are there browser console errors?
2. Is the request reaching the expected MVC action?
3. Does the response contain the expected HTML?
4. For a frame request, does the response contain the matching frame ID?
5. For streams, is the response content type
   `text/vnd.turbo-stream.html`?
6. Does every stream target an element that exists in the current document?
7. Does a `replace` partial include the target element itself?
8. Is the anti-forgery token present for a protected form?
9. Is a stale modal, event listener, or observer being retained by Turbo cache?
10. Did a Turbo navigation replace the body without rerunning setup code?

The browser Network tab is usually the fastest way to distinguish a routing
problem from a rendering or target problem.

------------------------------------------------------------------------

## 17. Hotwire Translation Trick

Translate each piece of markup into a sentence.

``` html
<a href="/Employees/Form"
   data-turbo-frame="modal-content">
    Edit
</a>
```

Read it as:

> **GET** `/Employees/Form` **when I click this link**, then replace the
> contents of the `modal-content` frame with the matching frame returned by
> MVC.

For a stream:

``` html
<turbo-stream action="replace" target="employee-table-container">
    <template><!-- rendered table --></template>
</turbo-stream>
```

Read it as:

> Find `#employee-table-container` and replace it with the HTML inside the
> stream template.

For Stimulus:

``` html
<button data-action="click->counter#increment">
    +
</button>
```

Read it as:

> On click, call `increment()` on the `counter` controller attached to this
> element.

------------------------------------------------------------------------

## 18. Hotwire vs HTMX

| Concern | Hotwire | HTMX |
| --- | --- | --- |
| Navigation | Turbo Drive | Explicit attributes or `hx-boost` |
| One-region updates | Turbo Frames | `hx-target` and `hx-swap` |
| Multi-target updates | Turbo Streams | Multiple requests or custom response handling |
| Browser behavior | Stimulus controllers | JavaScript or another controller layer |
| Request style | Convention through HTML structure | Explicit `hx-*` attributes |

Choose HTMX when compact, explicit request attributes are the clearest fit.
Choose Hotwire when navigation, frames, streams, and structured browser
controllers form a coherent application-wide pattern. Mixing both can work,
but give each screen a clear ownership model.

------------------------------------------------------------------------

## 19. Recommended MVC Convention

For a Hotwire MVC screen, make the response type obvious from the action and
partial names:

``` text
EmployeesController
|
+- Index()          -> full page
+- Form()           -> matching Turbo Frame partial
+- Create()         -> Turbo Stream response or form partial on validation
+- Edit()           -> Turbo Stream response or form partial on validation
+- Delete()         -> Turbo Stream response
```

The golden rule is:

> **Return the smallest HTML fragment that expresses the state change.**

``` text
User action
    |
    v
Turbo Drive / Frame / Stream
    |
    v
MVC controller
    |
    v
Razor partial or stream response
    |
    v
Turbo updates the page
    |
    v
Stimulus handles local browser behavior
```
