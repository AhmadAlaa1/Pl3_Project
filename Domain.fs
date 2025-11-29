// =======================================
// Domain.fs
// Student Grades Management System (F#)
// Author: Student Model Designer
// Description:
// This file defines ALL core domain types
// used across the entire project.
// =======================================

// ------------------------------
// User Roles
// ------------------------------
type Role =
    | Admin
    | Viewer
// Admin: full access (CRUD + stats + save/load)
// Viewer: read-only (view students + view stats)


// ------------------------------
// Grade Record
// Each grade is a subject + score
// ------------------------------
type Grade = {
    Subject : string
    Score   : float
}
// Example:
// { Subject = "Math"; Score = 85.0 }


// ------------------------------
// Student Record
// A student contains:
// - ID
// - Name
// - List of grades
// - Passing status (updated by GradeCalc module)
// ------------------------------
type Student = {
    Id        : int
    Name      : string
    Grades    : Grade list
    IsPassing : bool
}
// NOTE:
// IsPassing will be recalculated using GradeCalc.isPassing


// ------------------------------
// Class Statistics (for reports)
// These values are automatically computed
// by the Statistician team member
// ------------------------------
type ClassStats = {
    StudentCount : int
    HighestAvg   : float option
    LowestAvg    : float option
    PassRate     : float option
}
// Example:
// { StudentCount = 10
//   HighestAvg = Some 92.0
//   LowestAvg = Some 55.0
//   PassRate = Some 0.70 }


// ------------------------------
// Global Application State
// This is what gets saved/loaded via JSON
// ------------------------------
type AppState = {
    Students : Student list
}
// Example:
// { Students = [ student1; student2 ] }
