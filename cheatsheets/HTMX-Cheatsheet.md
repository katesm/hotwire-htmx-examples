# HTMX Cheatsheet for ASP.NET Core MVC

A practical quick reference for developers who are new to HTMX and are
working with **ASP.NET Core MVC**, **Razor Views/Partial Views**, **EF
Core**, and **Tailwind CSS**.

------------------------------------------------------------------------

## 1. The HTMX Mental Model

HTMX lets HTML elements make HTTP requests and update part of the page
with the HTML returned by the server.

Instead of:

``` text
Browser
  ↓
JavaScript
  ↓
API → JSON
  ↓
JavaScript
  ↓
Update DOM
```

You can do:

``` text
Browser
  ↓
HTMX HTTP request
  ↓
MVC Controller
  ↓
Partial View → HTML
  ↓
HTMX swaps HTML into the page
```

### The key idea

> **HTMX does not replace ASP.NET Core MVC. It makes server-rendered MVC
> pages interactive without requiring a SPA framework.**

------------------------------------------------------------------------

# 2. The Five Attributes to Learn First

If you are new to HTMX, start with these:

  Attribute      What it does
  -------------- ---------------------------------------------
  `hx-get`       Makes a GET request
  `hx-post`      Makes a POST request
  `hx-target`    Specifies where the response goes
  `hx-swap`      Specifies how the response replaces content
  `hx-trigger`   Specifies when the request happens

Example:

``` html
<input
    name="search"
    hx-get="/students/search"
    hx-trigger="input changed delay:300ms"
    hx-target="#results"
    hx-swap="innerHTML">
```

Read this as:

> When the input changes, wait 300ms, GET `/students/search`, and put
> the returned HTML inside `#results`.

------------------------------------------------------------------------

# 3. HTTP Request Attributes

## `hx-get`

Makes a GET request.

``` html
<button hx-get="/students">
    Load Students
</button>
```

MVC:

``` csharp
[HttpGet]
public IActionResult List()
{
    var students = _db.Students.ToList();

    return PartialView("_StudentList", students);
}
```

------------------------------------------------------------------------

## `hx-post`

Makes a POST request.

``` html
<button hx-post="/students">
    Create Student
</button>
```

Usually used with forms:

``` html
<form
    hx-post="/students"
    hx-target="#student-list"
    hx-swap="beforeend">

    <input name="FirstName" />
    <input name="LastName" />

    <button type="submit">
        Add Student
    </button>
</form>
```

MVC:

``` csharp
[HttpPost]
public IActionResult Create(Student model)
{
    if (!ModelState.IsValid)
    {
        return PartialView("_StudentForm", model);
    }

    _db.Students.Add(model);
    _db.SaveChanges();

    return PartialView("_StudentRow", model);
}
```

------------------------------------------------------------------------

## `hx-put`

Makes a PUT request.

``` html
<button hx-put="/students/123">
    Update
</button>
```

Use when your application follows a REST-style update convention.

------------------------------------------------------------------------

## `hx-patch`

Makes a PATCH request.

``` html
<button hx-patch="/students/123">
    Update
</button>
```

Useful for partial updates.

------------------------------------------------------------------------

## `hx-delete`

Makes a DELETE request.

``` html
<button hx-delete="/students/123">
    Delete
</button>
```

MVC:

``` csharp
[HttpDelete]
public IActionResult Delete(int id)
{
    var student = _db.Students.Find(id);

    if (student is not null)
    {
        _db.Students.Remove(student);
        _db.SaveChanges();
    }

    return new EmptyResult();
}
```

------------------------------------------------------------------------

# 4. `hx-target`

`hx-target` tells HTMX which element should receive the response.

``` html
<button
    hx-get="/students/123"
    hx-target="#student-details">

    View Student
</button>

<div id="student-details"></div>
```

The server returns HTML, and HTMX places it in `#student-details`.

## Common target selectors

### By ID

``` html
hx-target="#results"
```

### By class

``` html
hx-target=".results"
```

### The element itself

``` html
hx-target="this"
```

### Closest matching element

``` html
hx-target="closest tr"
```

This is especially useful for tables.

``` html
<button
    hx-delete="/students/123"
    hx-target="closest tr"
    hx-swap="delete">

    Delete
</button>
```

------------------------------------------------------------------------

# 5. `hx-swap`

`hx-swap` determines how HTMX inserts the response.

## `innerHTML`

Replace the contents of the target.

``` html
hx-swap="innerHTML"
```

Given:

``` html
<div id="results">
    Old content
</div>
```

And the server returns:

``` html
<p>New content</p>
```

The result is:

``` html
<div id="results">
    <p>New content</p>
</div>
```

`innerHTML` is the default.

------------------------------------------------------------------------

## `outerHTML`

Replace the entire target element.

``` html
hx-swap="outerHTML"
```

Useful when editing or replacing a table row:

``` html
<tr id="student-123">
    <td>Michael</td>
    <td>
        <button
            hx-get="/students/123/edit"
            hx-target="closest tr"
            hx-swap="outerHTML">
            Edit
        </button>
    </td>
</tr>
```

The server can return a completely new `<tr>`.

------------------------------------------------------------------------

## Other swap modes

  Swap            Meaning
  --------------- -----------------------------------------
  `innerHTML`     Replace target contents
  `outerHTML`     Replace target itself
  `beforebegin`   Insert before target
  `afterbegin`    Insert inside target before first child
  `beforeend`     Insert inside target after last child
  `afterend`      Insert after target
  `delete`        Remove target
  `none`          Do not insert the response

### Common pattern: append

``` html
hx-target="#student-list"
hx-swap="beforeend"
```

### Common pattern: remove

``` html
hx-target="closest tr"
hx-swap="delete"
```

------------------------------------------------------------------------

# 6. `hx-trigger`

Controls when the request occurs.

## Click

``` html
hx-trigger="click"
```

For buttons, `click` is normally the default.

------------------------------------------------------------------------

## Change

Great for dropdowns:

``` html
<select
    name="academicYear"
    hx-get="/courses"
    hx-trigger="change"
    hx-target="#courses">
</select>
```

------------------------------------------------------------------------

## Input

Run when the user types:

``` html
<input
    hx-get="/students/search"
    hx-trigger="input"
    hx-target="#results">
```

------------------------------------------------------------------------

## Debounced search

A very common pattern:

``` html
<input
    name="search"
    hx-get="/students/search"
    hx-trigger="input changed delay:300ms"
    hx-target="#results">
```

This prevents a request from being sent for every keystroke.

------------------------------------------------------------------------

## Load

Request content when the element loads:

``` html
<div
    hx-get="/notifications"
    hx-trigger="load">
</div>
```

------------------------------------------------------------------------

## Polling

Request repeatedly:

``` html
<div
    hx-get="/notifications"
    hx-trigger="every 10s">
</div>
```

Use polling carefully. Only poll when there is a real need.

------------------------------------------------------------------------

# 7. Forms

HTMX works naturally with ASP.NET Core MVC forms.

``` html
<form
    hx-post="/students"
    hx-target="#student-list"
    hx-swap="beforeend">

    <div>
        <label for="firstName">First Name</label>
        <input id="firstName" name="FirstName" />
    </div>

    <div>
        <label for="lastName">Last Name</label>
        <input id="lastName" name="LastName" />
    </div>

    <button type="submit">
        Add Student
    </button>
</form>
```

MVC model binding works normally because HTMX is sending a normal HTTP
request.

``` csharp
[HttpPost]
public IActionResult Create(Student model)
{
    // Model binding works as usual.
}
```

### Important

HTMX does **not** require you to create a JSON API for every
interaction.

The MVC action can return a Razor Partial View containing HTML.

------------------------------------------------------------------------

# 8. `hx-include`

Use `hx-include` when you want to include values from another element.

Example:

``` html
<div id="filters">
    <input name="search" />

    <select name="academicYear">
        <option value="2026">2026</option>
        <option value="2027">2027</option>
    </select>
</div>

<button
    hx-get="/students"
    hx-include="#filters"
    hx-target="#student-list">

    Apply Filters
</button>
```

The request will include the named values from `#filters`.

This is useful for:

-   Search filters
-   Academic year filters
-   Multiple dropdowns
-   Pagination controls
-   Filter panels

------------------------------------------------------------------------

# 9. `hx-vals`

Add additional values to a request.

``` html
<button
    hx-post="/students/123/status"
    hx-vals='{"status":"approved"}'>

    Approve
</button>
```

Think of `hx-vals` as:

> "Send these additional values with the request."

------------------------------------------------------------------------

# 10. `hx-confirm`

Add a confirmation prompt without writing JavaScript.

``` html
<button
    hx-delete="/students/123"
    hx-confirm="Are you sure you want to delete this student?">

    Delete
</button>
```

------------------------------------------------------------------------

# 11. `hx-indicator`

Show a loading indicator while the request is running.

``` html
<button
    hx-get="/students"
    hx-target="#results"
    hx-indicator="#spinner">

    Search
</button>

<span id="spinner" class="htmx-indicator">
    Loading...
</span>
```

With Tailwind, you can style the indicator to match your design system.

------------------------------------------------------------------------

# 12. `hx-push-url`

Update the browser URL after an HTMX request.

``` html
<button
    hx-get="/students?page=2"
    hx-target="#student-list"
    hx-push-url="true">

    Next
</button>
```

The browser URL becomes:

``` text
/students?page=2
```

Useful for:

-   Pagination
-   Search
-   Filtering
-   Tabs
-   Detail pages

------------------------------------------------------------------------

# 13. `hx-select`

Use `hx-select` when the server response contains more HTML than you
need.

``` html
<div
    hx-get="/students"
    hx-select="#student-list"
    hx-target="#student-list">
</div>
```

If the server returns:

``` html
<html>
    <body>
        <header>...</header>

        <div id="student-list">
            ...
        </div>

        <footer>...</footer>
    </body>
</html>
```

HTMX extracts only:

``` html
<div id="student-list">
    ...
</div>
```

------------------------------------------------------------------------

# 14. `hx-boost`

`hx-boost` progressively enhances normal links and forms.

``` html
<body hx-boost="true">
```

Now:

``` html
<a href="/students">
    Students
</a>
```

can be handled through HTMX instead of causing a traditional full-page
navigation.

Use this carefully. For a new project, start with explicit HTMX
interactions and add `hx-boost` when you understand how your pages and
navigation work.

------------------------------------------------------------------------

# 15. Partial Views + HTMX

This is one of the most important ASP.NET Core MVC patterns.

A typical structure might look like:

``` text
Views/
└── Students/
    ├── Index.cshtml
    ├── _StudentList.cshtml
    ├── _StudentRow.cshtml
    ├── _StudentForm.cshtml
    └── _StudentEdit.cshtml
```

The full page:

``` csharp
public IActionResult Index()
{
    return View();
}
```

An HTMX endpoint:

``` csharp
[HttpGet]
public IActionResult List()
{
    var students = _db.Students.ToList();

    return PartialView("_StudentList", students);
}
```

The HTMX request:

``` html
<div
    hx-get="/students/list"
    hx-target="#student-list">
</div>
```

The server returns HTML instead of JSON.

------------------------------------------------------------------------

# 16. CRUD Pattern

HTMX works particularly well for CRUD screens.

## List

``` html
<button
    hx-get="/students"
    hx-target="#student-list">
    Refresh
</button>
```

## Create

``` html
<form
    hx-post="/students"
    hx-target="#student-list"
    hx-swap="beforeend">
    ...
</form>
```

## Edit

``` html
<button
    hx-get="/students/123/edit"
    hx-target="closest tr"
    hx-swap="outerHTML">
    Edit
</button>
```

## Delete

``` html
<button
    hx-delete="/students/123"
    hx-target="closest tr"
    hx-swap="delete"
    hx-confirm="Delete this student?">
    Delete
</button>
```

------------------------------------------------------------------------

# 17. The Inline Edit Pattern

This is a great example of how HTMX changes the way you think about MVC
UI.

Normal row:

``` html
<tr id="student-123">
    <td>Michael</td>
    <td>Kates</td>
    <td>
        <button
            hx-get="/students/123/edit"
            hx-target="closest tr"
            hx-swap="outerHTML">
            Edit
        </button>
    </td>
</tr>
```

Clicking **Edit** causes MVC to return:

``` html
<tr id="student-123">
    <td colspan="3">
        <form
            hx-post="/students/123"
            hx-target="closest tr"
            hx-swap="outerHTML">

            <input
                name="FirstName"
                value="Michael" />

            <input
                name="LastName"
                value="Kates" />

            <button type="submit">
                Save
            </button>
        </form>
    </td>
</tr>
```

Save causes MVC to return the normal row again.

No SPA state management is required.

------------------------------------------------------------------------

# 18. ASP.NET Core MVC Anti-Forgery Tokens

If your application uses ASP.NET Core's normal antiforgery protection,
remember that HTMX POST/PUT/PATCH/DELETE requests may need the
antiforgery token.

For forms, the normal MVC pattern can be used:

``` html
<form
    hx-post="/students">

    @Html.AntiForgeryToken()

    ...
</form>
```

For non-form requests, configure your application so the antiforgery
token is sent in the way expected by your application's antiforgery
configuration.

**Do not disable antiforgery protection just to make HTMX requests
work.**

------------------------------------------------------------------------

# 19. HTTP Status Codes Matter

HTMX works with normal HTTP responses.

Use appropriate status codes.

Common examples:

  Status                        Typical use
  ----------------------------- ------------------------------------------
  `200 OK`                      Successful request with HTML
  `201 Created`                 Resource created
  `204 No Content`              Successful request with no response body
  `400 Bad Request`             Invalid request
  `401 Unauthorized`            Authentication required
  `403 Forbidden`               Authenticated but not allowed
  `404 Not Found`               Resource doesn't exist
  `422 Unprocessable Content`   Validation failure

For example, a successful delete can return:

``` csharp
return NoContent();
```

while the client removes the row using:

``` html
hx-swap="delete"
```

------------------------------------------------------------------------

# 20. HTMX + Tailwind

HTMX handles **behavior**.

Tailwind handles **presentation**.

Example:

``` html
<button
    hx-delete="/students/123"
    hx-target="closest tr"
    hx-swap="delete"
    hx-confirm="Delete this student?"
    class="rounded-md bg-red-600 px-4 py-2 text-sm font-medium text-white">

    Delete
</button>
```

This separation is useful:

``` text
HTMX
  ↓
Interaction / HTTP / DOM replacement

Tailwind
  ↓
Styling / layout / visual states
```

------------------------------------------------------------------------

# 21. Common HTMX Patterns

## Search

``` html
<input
    name="search"
    hx-get="/students/search"
    hx-trigger="input changed delay:300ms"
    hx-target="#results">
```

------------------------------------------------------------------------

## Filtering

``` html
<select
    name="academicYear"
    hx-get="/students"
    hx-trigger="change"
    hx-target="#student-list">
</select>
```

------------------------------------------------------------------------

## Pagination

``` html
<a
    href="/students?page=2"
    hx-get="/students?page=2"
    hx-target="#student-list"
    hx-push-url="true">

    Next
</a>
```

------------------------------------------------------------------------

## Delete a table row

``` html
<button
    hx-delete="/students/123"
    hx-target="closest tr"
    hx-swap="delete"
    hx-confirm="Delete this student?">

    Delete
</button>
```

------------------------------------------------------------------------

## Load a modal

``` html
<button
    hx-get="/students/123"
    hx-target="#modal-content">

    View
</button>

<div id="modal-content"></div>
```

------------------------------------------------------------------------

## Load content when a section appears

``` html
<div
    hx-get="/notifications"
    hx-trigger="load"
    hx-target="this">
</div>
```

------------------------------------------------------------------------

# 22. What Should the Controller Return?

This is a common question for MVC developers.

### Full page navigation

Return a View:

``` csharp
return View("Index", model);
```

### HTMX interaction

Usually return a Partial View:

``` csharp
return PartialView("_StudentRow", model);
```

### Successful action with nothing to render

Return no content:

``` csharp
return NoContent();
```

A useful rule:

> **If HTMX is updating part of the page, return the HTML fragment
> needed to update that part.**

------------------------------------------------------------------------

# 23. Don't Return JSON Just Because You Can

With a traditional SPA you might do:

``` json
{
    "id": 123,
    "firstName": "Michael"
}
```

and then JavaScript turns that into HTML.

With MVC + HTMX, prefer:

``` html
<tr>
    <td>Michael</td>
    ...
</tr>
```

The server already knows how the UI should be rendered.

This is one of the biggest conceptual differences between:

``` text
SPA architecture
```

and:

``` text
Server-rendered MVC + HTMX
```

------------------------------------------------------------------------

# 24. When Should I Use JavaScript?

HTMX does not mean:

> "Never write JavaScript."

Use JavaScript when the browser needs behavior that is genuinely
client-side.

Examples:

-   Complex client-side calculations
-   Rich drag-and-drop
-   Browser APIs
-   Canvas
-   Highly interactive widgets
-   Third-party JavaScript libraries

A good rule:

> **Use HTML + HTMX for server-driven interactions. Use JavaScript when
> the behavior genuinely belongs in the browser.**

------------------------------------------------------------------------

# 25. HTMX vs Traditional JavaScript

### Traditional approach

``` text
Click
 ↓
JavaScript event handler
 ↓
fetch()
 ↓
JSON
 ↓
Deserialize
 ↓
Build DOM
 ↓
Update DOM
```

### HTMX

``` text
Click
 ↓
hx-get
 ↓
MVC Controller
 ↓
Partial View
 ↓
HTML
 ↓
HTMX swaps HTML
```

The second approach can eliminate a lot of application-specific
JavaScript.

------------------------------------------------------------------------

# 26. Troubleshooting Checklist

When an HTMX interaction isn't working, check these in order.

### 1. Is HTMX loaded?

Open the browser console and verify there are no script errors.

### 2. Is the URL correct?

Look at the browser Network tab.

``` text
GET /students/123
```

### 3. Is the MVC action being hit?

Put a breakpoint in the controller.

### 4. What did the server return?

Inspect the Network response.

You should usually see HTML.

### 5. Is `hx-target` correct?

``` html
hx-target="#student-list"
```

Make sure that element exists.

### 6. Is `hx-swap` correct?

Try:

``` html
hx-swap="innerHTML"
```

or:

``` html
hx-swap="outerHTML"
```

### 7. Did validation fail?

Check the response returned by the MVC action.

### 8. Is antiforgery protection blocking the request?

Look for a `400` response.

### 9. Check the browser Network tab

The Network tab is one of your best HTMX debugging tools.

------------------------------------------------------------------------

# 27. Quick Reference Table

  Attribute        Example                     Purpose
  ---------------- --------------------------- -------------------------
  `hx-get`         `hx-get="/students"`        GET request
  `hx-post`        `hx-post="/students"`       POST request
  `hx-put`         `hx-put="/students/1"`      PUT request
  `hx-patch`       `hx-patch="/students/1"`    PATCH request
  `hx-delete`      `hx-delete="/students/1"`   DELETE request
  `hx-target`      `hx-target="#results"`      Response destination
  `hx-swap`        `hx-swap="outerHTML"`       Replacement behavior
  `hx-trigger`     `hx-trigger="change"`       Request trigger
  `hx-include`     `hx-include="#filters"`     Include other values
  `hx-vals`        `hx-vals='{"id":1}'`        Add request values
  `hx-confirm`     `hx-confirm="Delete?"`      Confirmation
  `hx-indicator`   `hx-indicator="#spinner"`   Loading indicator
  `hx-push-url`    `hx-push-url="true"`        Update browser URL
  `hx-select`      `hx-select="#results"`      Select response content
  `hx-boost`       `hx-boost="true"`           Enhance links/forms
  `hx-sync`        `hx-sync="this:replace"`    Coordinate requests

------------------------------------------------------------------------

# 28. The HTMX "Translation" Trick

When reading HTMX markup, translate it into a sentence.

``` html
<button
    hx-get="/students/123/edit"
    hx-target="closest tr"
    hx-swap="outerHTML">
    Edit
</button>
```

Read it as:

> **GET** `/students/123/edit`\
> **when** I click this button\
> **take the response**\
> **and replace the closest `<tr>` with it.**

Another example:

``` html
<input
    hx-get="/students/search"
    hx-trigger="input changed delay:300ms"
    hx-target="#results">
```

Read it as:

> **GET** `/students/search`\
> **when** the user stops changing this input for 300ms\
> **and replace** `#results` with the response.

Once you can read HTMX this way, most HTMX markup becomes much easier to
understand.

------------------------------------------------------------------------

# 29. Recommended Project Convention

For an ASP.NET Core MVC application, consider organizing HTMX endpoints
around the UI they serve.

Example:

``` text
StudentsController
│
├── Index()
├── List()
├── Create()
├── Edit()
├── Update()
└── Delete()
```

And views:

``` text
Views/Students/
│
├── Index.cshtml
├── _StudentList.cshtml
├── _StudentRow.cshtml
├── _StudentForm.cshtml
└── _StudentEdit.cshtml
```

Keep the distinction clear:

``` text
View
  ↓
Full page

Partial View
  ↓
HTMX fragment
```

This makes it easier for new developers to understand what the server is
expected to return.

------------------------------------------------------------------------

# 30. The Golden Rule

When building an MVC page, ask:

> **"Can the server return the HTML I need?"**

If yes, HTMX may be all you need.

``` text
User interaction
      ↓
    HTMX
      ↓
 HTTP request
      ↓
MVC Controller
      ↓
Application / EF Core
      ↓
 Partial View
      ↓
    HTML
      ↓
    HTMX
      ↓
Update part of page
```

### Remember

**MVC renders the HTML.**

**HTMX requests and swaps the HTML.**

**Tailwind styles the HTML.**

**EF Core persists the data.**

That is the core of an ASP.NET Core MVC + HTMX application.
