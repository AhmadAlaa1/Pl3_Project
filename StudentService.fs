namespace StudentManagement

open StatusTypes

module StudentService =

    let private getAllStudents () =
        match DatabaseAccess.getAllStudents() with
        | Ok students -> students
        | Error msg -> []  

    let createStudent (id: int) (name: string) (email: string) : Result<string, StatusError> =
        let students = getAllStudents()

        let idErrors = Validation.validateId (students |> List.map (fun s -> s.Id)) id
        let nameErrors = Validation.validateName name
        let emailErrors = Validation.validateEmail email

        let allErrors = idErrors @ nameErrors @ emailErrors

        if List.isEmpty allErrors then
            match DatabaseAccess.addStudent id name email with
            | Ok () -> Ok "Student created successfully"
            | Error msg -> Error (StatusError.ServerError("Save failed: Email may already exist. "))
        else
            Error (StatusError.ValidationError(String.concat "; " allErrors))

    let listAllStudents () : Student list =
        getAllStudents() |> List.sortBy (fun s -> s.Id)

    let findStudentById (studentId: int) : Student option =
        getAllStudents() |> List.tryFind (fun s -> s.Id = studentId)

    let getStudentDetailsById (studentId: int) : Result<Student, StatusError> =
        match getAllStudents() |> List.tryFind (fun s -> s.Id = studentId) with
        | Some s -> Ok s
        | None -> Error (NotFoundError "Student not found")


    let findStudentByEmail (email: string) : Student option =
        getAllStudents() |> List.tryFind (fun s -> s.Email = email)

    let modifyStudent (studentId: int) (name: string) (email: string) : Result<string, StatusError> =
        match findStudentById studentId with
        | Some _ ->
            let nameErrors = Validation.validateName name
            let emailErrors = Validation.validateEmail email

            if List.isEmpty nameErrors && List.isEmpty emailErrors then
                match DatabaseAccess.updateStudent studentId name email with
                | Ok () -> Ok "Student updated successfully"
                | Error msg -> Error (StatusError.ServerError("Save failed: Email may already exist. "))
            else
                Error (StatusError.ValidationError(String.concat "; " (nameErrors @ emailErrors)))
        | None -> Error (StatusError.NotFoundError $"Student with ID {studentId} not found")

    let removeStudent (studentId: int) : Result<string, StatusError> =
        match DatabaseAccess.deleteStudent studentId with
        | Ok () -> Ok "Student removed successfully"
        | Error msg -> Error (StatusError.ServerError(msg))

    let addStudentGrade (studentId: int) (subject: string) (grade: float) : Result<string, StatusError> =
        match findStudentById studentId with
        | Some student ->
            let gradeErrors = Validation.validateGrade subject grade
            if List.isEmpty gradeErrors then
                if student.Grades.ContainsKey subject then
                    Error (StatusError.ConflictError "Subject already exists for this student")
                else
                    match DatabaseAccess.addGrade studentId subject grade with
                    | Ok () -> Ok "Grade added successfully"
                    | Error msg -> Error (StatusError.ServerError("Save failed: " + msg))
            else
                Error (StatusError.ValidationError(String.concat "; " gradeErrors))
        | None -> Error (StatusError.NotFoundError $"Student with ID {studentId} not found")

    let updateStudentGrade (studentId: int) (subject: string) (newGrade: float) : Result<string, StatusError> =
        match findStudentById studentId with
        | Some student ->
            if student.Grades.ContainsKey subject then
                let gradeErrors = Validation.validateGrade subject newGrade
                if List.isEmpty gradeErrors then
                    match DatabaseAccess.updateGrade studentId subject newGrade with
                    | Ok () -> Ok "Grade updated successfully"
                    | Error msg -> Error (StatusError.ServerError("Save failed: " + msg))
                else
                    Error (StatusError.ValidationError(String.concat "; " gradeErrors))
            else
                Error (StatusError.NotFoundError $"Subject '{subject}' not found for this student")
        | None -> Error (StatusError.NotFoundError $"Student with ID {studentId} not found")

    let removeStudentGrade (studentId: int) (subject: string) : Result<string, StatusError> =
        match findStudentById studentId with
        | Some student ->
            if student.Grades.ContainsKey subject then
                match DatabaseAccess.deleteGrade studentId subject with
                | Ok () -> Ok "Grade removed successfully"
                | Error msg -> Error (StatusError.ServerError("Save failed: " + msg))
            else
                Error (StatusError.NotFoundError $"Subject '{subject}' not found for this student")
        | None -> Error (StatusError.NotFoundError $"Student with ID {studentId} not found")
