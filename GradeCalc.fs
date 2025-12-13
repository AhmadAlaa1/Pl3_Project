namespace StudentManagement

open System
open StudentManagement  // لو Student معرف في Domain.fs

module GradeCalc =

    /// Total score of all grades (grades is Map<string,float>)
    let totalScore (grades : Map<string,float>) : float =
        grades |> Map.toList |> List.sumBy snd

    /// Average score of student's grades
    let averageScore (grades : Map<string,float>) : float option =
        if Map.isEmpty grades then None
        else
            let total = totalScore grades
            let count = float (Map.count grades)
            Some (total / count)

    /// Determine if student is passing based on threshold
    let isPassing (passThreshold : float) (grades : Map<string,float>) : bool =
        match averageScore grades with
        | None -> false
        | Some avg -> avg >= passThreshold

    /// Get student's pass/fail status dynamically
    let studentStatus (passThreshold : float) (student : Student) : string =
        if isPassing passThreshold student.Grades then "Pass" else "Fail"

    /// Calculate averages for all students (returns list of name * average)
    let averages (students : Student list) : (string * float option) list =
        students |> List.map (fun s -> s.Name, averageScore s.Grades)
