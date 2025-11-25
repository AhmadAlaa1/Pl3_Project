// Rawan Gamal

// Exmaple Code:
//  module StudentCrud =
//  let addStudent (newStudent : Student) (students : Student list) : Student list =
//  newStudent :: students
//  let updateStudent (updated : Student) (students : Student list) : Student list =
//  students
//  |> List.map (fun s -> if s.Id = updated.Id then updated else s)
//  let deleteStudent (id : int) (students : Student list) : Student list =
//  students
//  |> List.filter (fun s -> s.Id <> id)
