namespace StudentManagement

module Statistics =

    /// Calculate average grade for a student
    let studentAverage (student: Student) : float option =
        if student.Grades.IsEmpty then None
        else
            student.Grades
            |> Map.values
            |> Seq.average
            |> Some

    /// Check if a student is passing based on threshold
    let isPassing (passThreshold: float) (grades: Map<string, float>) : bool =
        if grades.IsEmpty then false
        else
            let avg =
                grades
                |> Map.values
                |> Seq.average
            avg >= passThreshold

    /// Calculate class-wide statistics
    let classStats (passThreshold: float) (students: Student list) : ClassStats =

        // (Id, Name, Average)
        let studentAvgs =
            students
            |> List.choose (fun s ->
                match studentAverage s with
                | Some avg -> Some (s.Id, s.Name, avg)
                | None -> None
            )

        let highestStudent =
            if List.isEmpty studentAvgs then None
            else
                studentAvgs
                |> List.maxBy (fun (_, _, avg) -> avg)
                |> Some

        let lowestStudent =
            if List.isEmpty studentAvgs then None
            else
                studentAvgs
                |> List.minBy (fun (_, _, avg) -> avg)
                |> Some

        let total = students.Length

        let passCount =
            students
            |> List.filter (fun s -> isPassing passThreshold s.Grades)
            |> List.length

        let passRate =
            if total = 0 then None
            else Some (float passCount / float total)

        {
            StudentCount = total
            HighestStudent = highestStudent
            LowestStudent  = lowestStudent
            PassRate = passRate
        }