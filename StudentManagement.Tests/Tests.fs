namespace StudentManagement.Tests

open System
open Xunit
open StudentManagement
open StatusTypes

// Fake database access module 
module DatabaseAccess =
    let mutable students : StudentManagement.Student list = []
    let mutable grades : Map<(int * string), float> = Map.empty

    let reset () =
        students <- []
        grades <- Map.empty

    let getAllStudents () = students

    let addStudent id name email =
        if String.IsNullOrWhiteSpace(name) then
            Error "Name cannot be empty"
        elif String.IsNullOrWhiteSpace(email) then
            Error "Email cannot be empty"
        elif students |> List.exists (fun s -> s.Id = id || s.Email = email) then
            Error "Duplicate"
        else
            let student = { Id = id; Name = name; Email = email; Grades = Map.empty }
            students <- student :: students
            Ok "Student created successfully"

    let updateStudent id name email =
        match students |> List.tryFindIndex (fun s -> s.Id = id) with
        | Some idx ->
            students <- students |> List.mapi (fun i s -> 
                if i = idx then { s with Name = name; Email = email } else s)
            Ok "Student updated successfully"
        | None -> Error "Student not found"

    let deleteStudent id =
        match students |> List.tryFind (fun s -> s.Id = id) with
        | Some _ ->
            students <- students |> List.filter (fun s -> s.Id <> id)
            Ok "Student removed successfully"
        | None -> Error "Student not found"

    let addGrade studentId subject score =
        if grades.ContainsKey((studentId, subject)) then
            Error "Grade already exists"
        else
            grades <- grades.Add((studentId, subject), score)
            Ok "Grade added successfully"

    let updateGrade studentId subject score =
        if grades.ContainsKey((studentId, subject)) then
            grades <- grades.Add((studentId, subject), score)
            Ok "Grade updated successfully"
        else
            Error "Grade not found"

    let deleteGrade studentId subject =
        if grades.ContainsKey((studentId, subject)) then
            grades <- grades.Remove((studentId, subject))
            Ok "Grade removed successfully"
        else
            Error "Grade not found"


// Unit tests
module StudentServiceTests =

    open StudentService
    open DatabaseAccess

    let setup () = DatabaseAccess.reset()

    [<Fact>]
    let ``Create student successfully`` () =
        setup()
        let result = addStudent 1 "Mona" "mona@mail.com"
        match result with
        | Ok msg ->
            Assert.Equal("Student created successfully", msg)
            Assert.Equal(1, students.Length)
        | _ -> Assert.True(false, "Expected Ok")

    [<Fact>]
    let ``Fail when creating student with duplicate email`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        let result = addStudent 2 "Sara" "ali@mail.com"
        match result with
        | Error _ -> Assert.True(true)
        | _ -> Assert.True(false, "Expected error for duplicate email")

    [<Fact>]
    let ``Fail when name is empty`` () =
        setup()
        let result = addStudent 1 "" "test@mail.com"
        match result with
        | Error _ -> Assert.True(true)
        | _ -> Assert.True(false, "Expected error for empty name")

    [<Fact>]
    let ``Modify student successfully`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        let result = updateStudent 1 "Alaa" "new@mail.com"
        match result with
        | Ok msg -> Assert.Equal("Student updated successfully", msg)
        | _ -> Assert.True(false, "Expected Ok")

    [<Fact>]
    let ``Fail when modifying missing student`` () =
        setup()
        let result = updateStudent 99 "Test" "test@mail.com"
        match result with
        | Error _ -> Assert.True(true)
        | _ -> Assert.True(false, "Expected error for missing student")

    [<Fact>]
    let ``Delete student successfully`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        let result = deleteStudent 1
        match result with
        | Ok msg ->
            Assert.Equal("Student removed successfully", msg)
            Assert.Empty(students)
        | _ -> Assert.True(false, "Expected Ok")

    [<Fact>]
    let ``Add grade successfully`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        let result = addGrade 1 "Math" 90.0
        match result with
        | Ok msg -> Assert.Equal("Grade added successfully", msg)
        | _ -> Assert.True(false, "Expected Ok")

    [<Fact>]
    let ``Fail when adding duplicate grade`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        addGrade 1 "Math" 90.0 |> ignore
        let result = addGrade 1 "Math" 80.0
        match result with
        | Error _ -> Assert.True(true)
        | _ -> Assert.True(false, "Expected error for duplicate grade")

    [<Fact>]
    let ``Update grade successfully`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        addGrade 1 "Math" 90.0 |> ignore
        let result = updateGrade 1 "Math" 95.0
        match result with
        | Ok msg -> Assert.Equal("Grade updated successfully", msg)
        | _ -> Assert.True(false, "Expected Ok")

    [<Fact>]
    let ``Remove grade successfully`` () =
        setup()
        addStudent 1 "Ali" "ali@mail.com" |> ignore
        addGrade 1 "Math" 90.0 |> ignore
        let result = deleteGrade 1 "Math"
        match result with
        | Ok msg -> Assert.Equal("Grade removed successfully", msg)
        | _ -> Assert.True(false, "Expected Ok")
