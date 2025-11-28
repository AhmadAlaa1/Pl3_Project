module GradeCalc

open Domain

/// Sum of all grades for a student
let totalScore (grades : Grade list) : float =
    grades |> List.sumBy (fun g -> g.Score)


/// Calculate the average score of a student's grades.
/// Returns Some avg if grades exist, otherwise None.
let averageScore (grades : Grade list) : float option =
    match grades with
    | [] -> None
    | _ ->
        let total = totalScore grades
        let count = grades |> List.length |> float
        Some (total / count)


/// Calculate averages for a list of students
let averages (students : Student list) : (string * float option) list =
    students
    |> List.map (fun s -> s.Name, averageScore s.Grades)


/// Determines if the student is passing based on a threshold.
/// Returns true if avg >= threshold, otherwise false.
let isPassing (passThreshold : float) (grades : Grade list) : bool =
    match averageScore grades with
    | None -> false
    | Some avg -> avg >= passThreshold


/// Recalculate the student's passing status (returns a NEW record).
/// Functional design → no mutations.
let recalcStudentPassStatus (passThreshold : float) (student : Student) : Student =
    let passing = isPassing passThreshold student.Grades
    { student with IsPassing = passing }
