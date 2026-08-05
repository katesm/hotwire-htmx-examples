# Hotwire + HTMX MVC Lab

This repository is a deliberately small ASP.NET Core MVC app for learning server-rendered UI patterns.
It contains two CRUD-style experiences so you can compare approaches side by side:

- Students: HTMX-based CRUD with partial views.
- Employees: Hotwire-based CRUD using Turbo Frames, Turbo Streams, and a small Stimulus layer.

## What’s in the demo

- **Turbo Frames**: load or replace modal content without a full page refresh.
- **Turbo Streams**: update the employee table after create, edit, and delete actions.
- **Stimulus**: lightweight browser behavior for the modal lifecycle and a sample counter.
- **Tailwind CSS**: built using Tailwind’s standalone executable; Node.js is not required.

## How to run it

```bash
./scripts/install-tailwind.sh # first time only; downloads the macOS/Linux executable
dotnet run
```

On Windows PowerShell:

```powershell
.\scripts\install-tailwind.ps1 # first time only
dotnet run
```

After modifying Razor views or JavaScript, regenerate the stylesheet:

```bash
./scripts/tailwind-build.sh
```

```powershell
.\scripts\tailwind-build.ps1
```

The Tailwind CLI binary is intentionally ignored by Git. The generated `wwwroot/css/site.css` is committed so the demo runs immediately after cloning.

## Hotwire vs HTMX

### Hotwire strengths

- Better fit for apps that already lean on server-rendered navigation.
- Turbo Frames and Turbo Streams give a clean path for partial updates across larger screens.
- Stimulus keeps browser behavior organized when interaction logic starts to grow.

### Hotwire tradeoffs

- More concepts to understand at once: Turbo Drive, Frames, Streams, and Stimulus.
- Dialogs, frame targets, and stream updates need a little more lifecycle handling.
- Small CRUD screens can feel more verbose than a direct attribute-based approach.

### HTMX strengths

- Very compact markup for basic CRUD interactions.
- Requests and swap targets stay close to the triggering element.
- Easy to read for simple server-rendered forms and tables.

### HTMX tradeoffs

- Less structured when the app grows into richer page transitions.
- Can become attribute-heavy when a screen has many moving parts.
- Browser behavior still tends to need separate JavaScript once interactions get more complex.

## When to use which

- Use **HTMX** when you want the shortest path for simple server-driven CRUD.
- Use **Hotwire** when you want a stronger pattern for navigation, frames, and stream updates.
- For real apps, mixing both is reasonable if each screen benefits from a different approach.
