namespace StudentManagement

open System
open StudentManagement  

module GradeCalc =

    let totalScore (grades : Map<string,float>) : float =
        grades |> Map.toList |> List.sumBy snd

    let averageScore (grades : Map<string,float>) : float option =
        if Map.isEmpty grades then None
        else
            let total = totalScore grades
            let count = float (Map.count grades)
            Some (total / count)

    let isPassing (passThreshold : float) (grades : Map<string,float>) : bool =
        match averageScore grades with
        | None -> false
        | Some avg -> avg >= passThreshold

    let studentStatus (passThreshold : float) (student : Student) : string =
        if isPassing passThreshold student.Grades then "Pass" else "Fail"

    let averages (students : Student list) : (string * float option) list =
        students |> List.map (fun s -> s.Name, averageScore s.Grades)
