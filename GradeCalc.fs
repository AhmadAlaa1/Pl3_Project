// Seif AbdelAziz

// Example Code:
//  let averageScore (grades : Grade list) : float option =
//  match grades with
//  | [] -> None
//  | _ ->
//  let sum = grades |> List.sumBy (fun g -> g.Score)
//  let count = grades |> List.length |> float
//  Some (sum / count)
//  let isPassing (passThreshold : float) (grades : Grade list) : bool =
//  match averageScore grades with
//  | None -> false
//  | Some avg -> avg >= passThreshold

//  let recalcStudentPassStatus passThreshold (student : Student) : Student =
//  let passing = isPassing passThreshold student.Grades
//  { student with IsPassing = passing }