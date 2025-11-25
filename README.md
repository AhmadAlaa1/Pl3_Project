# Student Grades Management System (F# – Functional Design)

Console-based, functional-first student records manager with immutable data, pure business logic, JSON persistence, and simple role-based access control (Admin vs Viewer).

## Features
- Manage students and grades with pure CRUD list operations.
- Compute per-student averages, pass/fail, and class statistics.
- Role-based access control (Admin: full CRUD; Viewer: read-only).
- JSON persistence to keep data between runs.
- Separation of concerns: domain + pure logic vs UI and I/O.

## Project Layout
- `Domain.fs` – Core types (`Role`, `Grade`, `Student`, `ClassStats`, `AppState`).
- `StudentCrud.fs` – Pure add/update/delete/find over immutable student lists.
- `GradeCalc.fs` – Grade averaging, pass/fail, and pass-status recompute.
- `Statistics.fs` – Class-level metrics (highest/lowest averages, pass rate).
- `AccessControl.fs` – Actions and permission checks per role.
- `Persistence.fs` – JSON save/load helpers for `AppState`.
- `Ui.fs` – Console loop that wires user input to pure functions.
- `Program.fs` – Entry point (currently a placeholder to be filled in).

> Note: Many modules currently hold example code/comments as scaffolding. Fill in the actual implementations following the signatures and examples provided in each file.

## Data Model (essential types)
```fsharp
type Role = Admin | Viewer

type Grade = { Subject: string; Score: float }

type Student = {
    Id: int
    Name: string
    Grades: Grade list
    IsPassing: bool
}

type ClassStats = {
    StudentCount: int
    HighestAvg: float option
    LowestAvg: float option
    PassRate: float option
}

type AppState = { Students: Student list }
```

## Build & Run
Requirements: [.NET SDK](https://dotnet.microsoft.com/download)

```bash
# restore/build
dotnet build

# run (fill in Program.fs / Ui.fs main loop first)
dotnet run
```

## Suggested Next Steps
1) Implement the pure functions in each module using the provided examples.
2) Wire up `Program.fs` and `Ui.fs` to:
   - load initial state via `Persistence.loadFromFile`
   - prompt for role (Admin/Viewer)
   - route menu choices to `StudentCrud`, `GradeCalc`, `Statistics`
   - save on exit via `Persistence.saveToFile`
3) Add basic tests for pure modules (`StudentCrud`, `GradeCalc`, `Statistics`, `AccessControl`).
4) Refine JSON schema and ensure backwards compatibility when fields change.
5) Polish the console UX (validation, nicer printing of tables, etc.).

## Design Principles
- Immutable data; no in-place mutation.
- Pure business logic; side effects only at the edges (console, file I/O).
- Small, composable functions with clear signatures.
- Role checks before performing mutating actions.
- Keep persistence isolated; treat other modules as a pure library.

