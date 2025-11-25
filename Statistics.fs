// Omnia Waleed

// Example Code:
//  module Statistics =
//  let studentAverage (s : Student) : float option =
//  GradeCalc.averageScore s.Grades
//  let classStats (passThreshold : float) (students : Student list) : ClassStats =
//  let avgs =
//  students
//  |> List.choose (fun s -> studentAverage s)
//  let highest =
//  match avgs with
//  | [] -> None
//  | _ -> Some (List.max avgs)
//  let lowest =
//  match avgs with
//  | [] -> None
//  | _ -> Some (List.min avgs)
//  let passCount =
//  students
//  |> List.filter (fun s -> GradeCalc.isPassing passThreshold s.Grades)
//  |> List.length
//  let total = students |> List.length
//  let passRate =
//  if total = 0 then None
//  else Some (float passCount / float total)
//  {
//      StudentCount = total
//      HighestAvg= highest
//      LowestAvg= lowest
//      PassRate= passRate
//  }
