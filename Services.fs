namespace StudentManagement

// ========== BUSINESS LOGIC ==========
module StudentService =
    let private getAllStudents () =
        match JsonSerializer.loadStudents() with
        | Ok students -> students
        | Error _ -> []

    let createStudent (id: int) (name: string) (email: string) : OperationResult =
        let students = getAllStudents()

        let idErrors = Validation.validateId (students |> List.map (fun s -> s.Id)) id
        let nameErrors = Validation.validateName name
        let emailErrors = Validation.validateEmail email

        let allErrors = idErrors @ nameErrors @ emailErrors

        if List.isEmpty allErrors then
            let newStudent = { Id = id; Name = name; Email = email; Grades = Map.empty }
            match JsonSerializer.saveStudents (newStudent :: students) with
            | Ok () -> Success
            | Error msg -> Failure ["Save failed: " + msg]
        else
            Failure allErrors

    let listAllStudents () =
        getAllStudents() |> List.sortBy (fun s -> s.Id)

    let findStudentById (studentId: int) =
        getAllStudents() |> List.tryFind (fun s -> s.Id = studentId)

    let modifyStudent (studentId: int) (name: string) (email: string) : OperationResult =
        let students = getAllStudents()
        match students |> List.tryFind (fun s -> s.Id = studentId) with
        | Some existingStudent ->
            let nameErrors = Validation.validateName name
            let emailErrors = Validation.validateEmail email

            if List.isEmpty nameErrors && List.isEmpty emailErrors then
                let updatedStudent = { existingStudent with Name = name; Email = email }
                let otherStudents = students |> List.filter (fun s -> s.Id <> studentId)
                match JsonSerializer.saveStudents (updatedStudent :: otherStudents) with
                | Ok () -> Success
                | Error msg -> Failure ["Save failed: " + msg]
            else
                Failure (nameErrors @ emailErrors)
        | None -> Failure ["Student not found"]

    let removeStudent (studentId: int) : OperationResult =
        let students = getAllStudents() |> List.filter (fun s -> s.Id <> studentId)
        match JsonSerializer.saveStudents students with
        | Ok () -> Success
        | Error msg -> Failure ["Save failed: " + msg]

    let addStudentGrade (studentId: int) (subject: string) (grade: float) : OperationResult =
        let students = getAllStudents()
        match students |> List.tryFind (fun s -> s.Id = studentId) with
        | Some student ->
            let gradeErrors = Validation.validateGrade subject grade
            if List.isEmpty gradeErrors then
                let updatedGrades = student.Grades |> Map.add subject grade
                let updatedStudent = { student with Grades = updatedGrades }
                let otherStudents = students |> List.filter (fun s -> s.Id <> studentId)
                match JsonSerializer.saveStudents (updatedStudent :: otherStudents) with
                | Ok () -> Success
                | Error msg -> Failure ["Save failed: " + msg]
            else
                Failure gradeErrors
        | None -> Failure ["Student not found"]

    let updateStudentGrade (studentId: int) (subject: string) (newGrade: float) : OperationResult =
        let students = getAllStudents()
        match students |> List.tryFind (fun s -> s.Id = studentId) with
        | Some student ->
            if student.Grades.ContainsKey subject then
                let gradeErrors = Validation.validateGrade subject newGrade
                if List.isEmpty gradeErrors then
                    let updatedGrades = student.Grades |> Map.add subject newGrade
                    let updatedStudent = { student with Grades = updatedGrades }
                    let otherStudents = students |> List.filter (fun s -> s.Id <> studentId)
                    match JsonSerializer.saveStudents (updatedStudent :: otherStudents) with
                    | Ok () -> Success
                    | Error msg -> Failure ["Save failed: " + msg]
                else
                    Failure gradeErrors
            else
                Failure ["Subject not found"]
        | None -> Failure ["Student not found"]

    let removeStudentGrade (studentId: int) (subject: string) : OperationResult =
        let students = getAllStudents()
        match students |> List.tryFind (fun s -> s.Id = studentId) with
        | Some student ->
            if student.Grades.ContainsKey subject then
                let updatedGrades = student.Grades |> Map.remove subject
                let updatedStudent = { student with Grades = updatedGrades }
                let otherStudents = students |> List.filter (fun s -> s.Id <> studentId)
                match JsonSerializer.saveStudents (updatedStudent :: otherStudents) with
                | Ok () -> Success
                | Error msg -> Failure ["Save failed: " + msg]
            else
                Failure ["Subject not found"]
        | None -> Failure ["Student not found"]