namespace StudentManagement

open System.IO
open System.Text.Json

module JsonSerializer =
    let private dataFile = "students.json"
    let private options = JsonSerializerOptions(WriteIndented = true)

    let saveStudents (students: Student list) =
        try
            let json = JsonSerializer.Serialize(students, options)
            File.WriteAllText(dataFile, json)
            Ok ()
        with
        | ex -> Error ex.Message

    let loadStudents () =
        try
            if File.Exists dataFile then
                let json = File.ReadAllText(dataFile)
                let students = JsonSerializer.Deserialize<Student list>(json)
                Ok students
            else
                Ok []
        with
        | ex -> Error ex.Message