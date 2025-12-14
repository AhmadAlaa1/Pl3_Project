namespace StudentManagement

open System
open System.Collections.Generic
open Microsoft.Data.Sqlite

module DatabaseAccess =
    let private connectionString = "Data Source=students.db"

    let initializeDatabase () =
        use connection = new SqliteConnection(connectionString)
        connection.Open()

        let createStudentsTable = """
            CREATE TABLE IF NOT EXISTS Students (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE
            )
        """

        let createAdminsTable = """
            CREATE TABLE IF NOT EXISTS Admins (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Email TEXT,
                CreatedDate TEXT NOT NULL
            )
        """

        let createViewersTable = """
            CREATE TABLE IF NOT EXISTS Viewers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE,
                CreatedDate TEXT NOT NULL
            )
        """

        let createGradesTable = """
            CREATE TABLE IF NOT EXISTS Grades (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER NOT NULL,
                Subject TEXT NOT NULL,
                Grade REAL NOT NULL,
                FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
                UNIQUE(StudentId, Subject)
            )
        """

        use cmd = connection.CreateCommand()
        cmd.CommandText <- createStudentsTable
        cmd.ExecuteNonQuery() |> ignore

        cmd.CommandText <- createAdminsTable
        cmd.ExecuteNonQuery() |> ignore

        cmd.CommandText <- createViewersTable
        cmd.ExecuteNonQuery() |> ignore

        cmd.CommandText <- createGradesTable
        cmd.ExecuteNonQuery() |> ignore

        use checkCmd = connection.CreateCommand()
        checkCmd.CommandText <- "SELECT COUNT(*) FROM Admins WHERE Name = 'admin'"
        let count = Convert.ToInt32(checkCmd.ExecuteScalar())

        if count = 0 then
            use insertCmd = connection.CreateCommand()
            insertCmd.CommandText <- "INSERT INTO Admins (Name, Password, Email, CreatedDate) VALUES ('admin', 'admin', 'admin@system.com', @date)"
            insertCmd.Parameters.AddWithValue("@date", System.DateTime.Now.ToString()) |> ignore
            insertCmd.ExecuteNonQuery() |> ignore

    let getAllStudents () : Result<Student list, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Id, Name, Email FROM Students ORDER BY Id"
            use reader = cmd.ExecuteReader()

            let students = ResizeArray<Student>()
            while reader.Read() do
                let id = reader.GetInt32(0)
                let name = reader.GetString(1)
                let email = reader.GetString(2)

                use gradeCmd = connection.CreateCommand()
                gradeCmd.CommandText <- "SELECT Subject, Grade FROM Grades WHERE StudentId = @id"
                gradeCmd.Parameters.AddWithValue("@id", id) |> ignore
                use gradeReader = gradeCmd.ExecuteReader()

                let grades = Map.ofList (
                    [while gradeReader.Read() do
                        yield (gradeReader.GetString(0), gradeReader.GetDouble(1))]
                )

                students.Add({
                    Id = id
                    Name = name
                    Email = email
                    Grades = grades
                })

            Ok (List.ofSeq students)
        with
        | ex -> Error ex.Message

    let addStudent (id: int) (name: string) (email: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "INSERT INTO Students (Id, Name, Email) VALUES (@id, @name, @email)"
            cmd.Parameters.AddWithValue("@id", id) |> ignore
            cmd.Parameters.AddWithValue("@name", name) |> ignore
            cmd.Parameters.AddWithValue("@email", email) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let updateStudent (id: int) (name: string) (email: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "UPDATE Students SET Name = @name, Email = @email WHERE Id = @id"
            cmd.Parameters.AddWithValue("@id", id) |> ignore
            cmd.Parameters.AddWithValue("@name", name) |> ignore
            cmd.Parameters.AddWithValue("@email", email) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message


    let deleteStudent (id: int) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "DELETE FROM Students WHERE Id = @id"
            cmd.Parameters.AddWithValue("@id", id) |> ignore

            let rows = cmd.ExecuteNonQuery()
            if rows = 0 then
                Error "Student not found"
            else
                Ok ()
        with
        | ex ->
            Error "Failed to delete student"


    let addGrade (studentId: int) (subject: string) (grade: float) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "INSERT INTO Grades (StudentId, Subject, Grade) VALUES (@studentId, @subject, @grade)"
            cmd.Parameters.AddWithValue("@studentId", studentId) |> ignore
            cmd.Parameters.AddWithValue("@subject", subject) |> ignore
            cmd.Parameters.AddWithValue("@grade", grade) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let updateGrade (studentId: int) (subject: string) (grade: float) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "UPDATE Grades SET Grade = @grade WHERE StudentId = @studentId AND Subject = @subject"
            cmd.Parameters.AddWithValue("@studentId", studentId) |> ignore
            cmd.Parameters.AddWithValue("@subject", subject) |> ignore
            cmd.Parameters.AddWithValue("@grade", grade) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let deleteGrade (studentId: int) (subject: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "DELETE FROM Grades WHERE StudentId = @studentId AND Subject = @subject"
            cmd.Parameters.AddWithValue("@studentId", studentId) |> ignore
            cmd.Parameters.AddWithValue("@subject", subject) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let findStudentByEmail (email: string) : Result<Student option, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Id, Name, Email FROM Students WHERE Email = @email"
            cmd.Parameters.AddWithValue("@email", email) |> ignore
            use reader = cmd.ExecuteReader()

            if not reader.HasRows then
                Ok None
            else
                reader.Read() |> ignore
                let id = reader.GetInt32(0)
                let name = reader.GetString(1)
                let email = reader.GetString(2)

                use gradeCmd = connection.CreateCommand()
                gradeCmd.CommandText <- "SELECT Subject, Grade FROM Grades WHERE StudentId = @id"
                gradeCmd.Parameters.AddWithValue("@id", id) |> ignore
                use gradeReader = gradeCmd.ExecuteReader()

                let grades = Map.ofList (
                    [while gradeReader.Read() do
                        yield (gradeReader.GetString(0), gradeReader.GetDouble(1))]
                )

                Ok (Some {
                    Id = id
                    Name = name
                    Email = email
                    Grades = grades
                })
        with
        | ex -> Error ex.Message

    let addAdmin (name: string) (password: string) (email: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "INSERT INTO Admins (Name, Password, Email, CreatedDate) VALUES (@name, @password, @email, @date)"
            cmd.Parameters.AddWithValue("@name", name) |> ignore
            cmd.Parameters.AddWithValue("@password", password) |> ignore
            cmd.Parameters.AddWithValue("@email", email) |> ignore
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now.ToString()) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let getAllAdmins () : Result<(string * string) list, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Name, Password FROM Admins"
            use reader = cmd.ExecuteReader()

            let admins = ResizeArray<string * string>()
            while reader.Read() do
                let name = reader.GetString(0)
                let password = reader.GetString(1)
                admins.Add((name, password))

            Ok (List.ofSeq admins)
        with
        | ex -> Error ex.Message

    let deleteAdmin (name: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "DELETE FROM Admins WHERE Name = @name"
            cmd.Parameters.AddWithValue("@name", name) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let addViewer (name: string) (password: string) (email: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "INSERT INTO Viewers (Name, Password, Email, CreatedDate) VALUES (@name, @password, @email, @date)"
            cmd.Parameters.AddWithValue("@name", name) |> ignore
            cmd.Parameters.AddWithValue("@password", password) |> ignore
            cmd.Parameters.AddWithValue("@email", email) |> ignore
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now.ToString()) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let getAllViewers () : Result<(string * string) list, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Name, Password FROM Viewers"
            use reader = cmd.ExecuteReader()

            let viewers = ResizeArray<string * string>()
            while reader.Read() do
                let name = reader.GetString(0)
                let password = reader.GetString(1)
                viewers.Add((name, password))

            Ok (List.ofSeq viewers)
        with
        | ex -> Error ex.Message

    let deleteViewer (name: string) : Result<unit, string> =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "DELETE FROM Viewers WHERE Name = @name"
            cmd.Parameters.AddWithValue("@name", name) |> ignore

            cmd.ExecuteNonQuery() |> ignore
            Ok ()
        with
        | ex -> Error ex.Message

    let displayAllData () =
        try
            use connection = new SqliteConnection(connectionString)
            connection.Open()

            printfn "\n========== STUDENTS TABLE =========="
            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Id, Name, Email FROM Students ORDER BY Id"
            use reader = cmd.ExecuteReader()

            if not reader.HasRows then
                printfn "No students in database."
            else
                printfn "%-5s %-20s %-30s" "ID" "Name" "Email"
                printfn "%s" (String.replicate 60 "-")
                while reader.Read() do
                    let id = reader.GetInt32(0)
                    let name = reader.GetString(1)
                    let email = reader.GetString(2)
                    printfn "%-5d %-20s %-30s" id name email

            printfn "\n========== GRADES TABLE =========="
            use gradeCmd = connection.CreateCommand()
            gradeCmd.CommandText <- "SELECT StudentId, Subject, Grade FROM Grades ORDER BY StudentId, Subject"
            use gradeReader = gradeCmd.ExecuteReader()

            if not gradeReader.HasRows then
                printfn "No grades in database."
            else
                printfn "%-10s %-20s %-10s" "StudentID" "Subject" "Grade"
                printfn "%s" (String.replicate 45 "-")
                while gradeReader.Read() do
                    let studentId = gradeReader.GetInt32(0)
                    let subject = gradeReader.GetString(1)
                    let grade = gradeReader.GetDouble(2)
                    printfn "%-10d %-20s %-10.2f" studentId subject grade

            printfn "\n========== ADMINS TABLE =========="
            use adminCmd = connection.CreateCommand()
            adminCmd.CommandText <- "SELECT Name, Password, Email, CreatedDate FROM Admins ORDER BY Id"
            use adminReader = adminCmd.ExecuteReader()

            if not adminReader.HasRows then
                printfn "No admins in database."
            else
                printfn "%-20s %-15s %-30s %-20s" "Name" "Password" "Email" "Created"
                printfn "%s" (String.replicate 90 "-")
                while adminReader.Read() do
                    let name = adminReader.GetString(0)
                    let password = adminReader.GetString(1)
                    let email = adminReader.GetString(2)
                    let created = adminReader.GetString(3)
                    printfn "%-20s %-15s %-30s %-20s" name password email created

            printfn "\n========== VIEWERS TABLE =========="
            use viewerCmd = connection.CreateCommand()
            viewerCmd.CommandText <- "SELECT Name, Password, Email, CreatedDate FROM Viewers ORDER BY Id"
            use viewerReader = viewerCmd.ExecuteReader()

            if not viewerReader.HasRows then
                printfn "No viewers in database."
            else
                printfn "%-20s %-15s %-30s %-20s" "Name" "Password" "Email" "Created"
                printfn "%s" (String.replicate 90 "-")
                while viewerReader.Read() do
                    let name = viewerReader.GetString(0)
                    let password = viewerReader.GetString(1)
                    let email = viewerReader.GetString(2)
                    let created = viewerReader.GetString(3)
                    printfn "%-20s %-15s %-30s %-20s" name password email created

            printfn "\n%s\n" (String.replicate 60 "=")
        with
        | ex -> printfn "Error displaying data: %s" ex.Message