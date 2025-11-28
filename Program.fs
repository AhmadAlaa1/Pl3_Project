// Just auto generated File


//!GradeCalc Test
open Domain
open GradeCalc

[<EntryPoint>]
let main argv =
    
    let s1 = {
        Name = "Seif"
        Grades = [ {Score = 80.0}; {Score = 90.0}; {Score = 70.0} ]
        IsPassing = false
    }

    let s2 = {
        Name = "Mona"
        Grades = [ {Score = 60.0}; {Score = 50.0} ]
        IsPassing = false
    }

    printfn "Total of Seif = %f" (totalScore s1.Grades)

    printfn "Average of Seif = %A" (averageScore s1.Grades)

    printfn "Is Seif passing 75? %b" (isPassing 75.0 s1.Grades)

    let s1Updated = recalcStudentPassStatus 90.0 s1
    printfn "Seif updated passing status = %b" s1Updated.IsPassing

    printfn "\nAverages of all students:" 
    averages [s1; s2] |> List.iter (printfn "%A")

    0
