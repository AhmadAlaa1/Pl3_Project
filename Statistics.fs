namespace StudentManagement

module Statistics =
    let studentAverage (student: Student) : float option =
        if student.Grades.IsEmpty then None
        else
            let grades = student.Grades |> Map.toList |> List.map snd
            Some (List.average grades)

    let isPassing (passThreshold: float) (grades: Map<string, float>) =
        if grades.IsEmpty then false
        else
            let avg = grades |> Map.toList |> List.map snd |> List.average
            avg >= passThreshold

    let classStats (passThreshold: float) (students: Student list) : ClassStats =
        let avgs =
            students
            |> List.choose studentAverage

        let highest = if List.isEmpty avgs then None else Some (List.max avgs)
        let lowest  = if List.isEmpty avgs then None else Some (List.min avgs)

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
            HighestAvg = highest
            LowestAvg = lowest
            PassRate = passRate
        }