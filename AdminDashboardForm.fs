namespace StudentManagement.WinForms

open System
open System.Windows.Forms
open StudentManagement
open StudentManagement.StatusTypes

type AdminDashboardForm() as this =
    inherit Form(
        Text = "Admin Dashboard",
        Width = Screen.PrimaryScreen.WorkingArea.Width,
        Height = Screen.PrimaryScreen.WorkingArea.Height,
        StartPosition = FormStartPosition.Manual,  // مهم علشان نحدد الـ bounds
        Left = 0,
        Top = 0
    )
    
    let dgvStudents = new DataGridView(Dock=DockStyle.Fill, ReadOnly=true, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill)

    let btnViewAll = new Button(Text="View All Students", Top=10, Left=10, Width=150)
    let btnViewById = new Button(Text="View Student By ID", Top=50, Left=10, Width=150)
    let btnAddStudent = new Button(Text="Add Student", Top=90, Left=10, Width=150)
    let btnEditStudent = new Button(Text="Edit Student", Top=130, Left=10, Width=150)
    let btnDeleteStudent = new Button(Text="Delete Student", Top=170, Left=10, Width=150)
    let btnAddGrade = new Button(Text="Add Grade", Top=210, Left=10, Width=150)
    let btnUpdateGrade = new Button(Text="Update Grade", Top=250, Left=10, Width=150)
    let btnRemoveGrade = new Button(Text="Remove Grade", Top=290, Left=10, Width=150)
    let btnViewStudentStats = new Button(Text="View Student Stats", Top=410, Left=10, Width=150)
    let btnViewStats = new Button(Text="View Statistics", Top=330, Left=10, Width=150)
    let btnViewTables = new Button(Text="View Database Tables", Top=370, Left=10, Width=150)
    let btnLogout = new Button(Text="Logout", Top=510, Left=10, Width=150)

    let panel = new Panel(Dock=DockStyle.Left, Width=180)
    
    do
        panel.Controls.AddRange([|
            btnViewAll; btnViewById; btnAddStudent; btnEditStudent; btnDeleteStudent;
            btnAddGrade; btnUpdateGrade; btnRemoveGrade; btnViewStudentStats; btnViewStats; btnViewTables; btnLogout
        |])
        this.Controls.AddRange([| dgvStudents; panel |])


        // View all students
        btnViewAll.Click.Add(fun _ ->
            let students = StudentService.listAllStudents()
            dgvStudents.DataSource <- 
                students 
                |> Seq.map (fun s -> {| Id = s.Id; Name = s.Name; Email = s.Email |}) 
                |> Seq.toList   
                |> ResizeArray
        )

        // View student by ID
        btnViewById.Click.Add(fun _ ->
            let input = Microsoft.VisualBasic.Interaction.InputBox("Enter Student ID:", "View Student")
            let ok, id = System.Int32.TryParse input
            if not ok then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Please enter a valid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.getStudentDetailsById id with
                | Ok student ->
                    MessageBox.Show(
                        sprintf "ID: %d\nName: %s\nEmail: %s\nGrades: %s"
                            student.Id
                            student.Name
                            student.Email
                            (if student.Grades.IsEmpty then "None"
                             else student.Grades |> Map.toList |> List.map (fun (k,v) -> sprintf "%s: %.2f" k v) |> String.concat "; "),
                        "Student Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // Add Student
        btnAddStudent.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter ID:", "Add Student")
            let name = Microsoft.VisualBasic.Interaction.InputBox("Enter Name:", "Add Student")
            let email = Microsoft.VisualBasic.Interaction.InputBox("Enter Email:", "Add Student")
            let ok, id = System.Int32.TryParse idStr
            if not ok then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.createStudent id name email with
                | Ok msg ->
                    let code = StatusCode.Created
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )
                    |> ignore

                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore



        )

        // Edit Student
        btnEditStudent.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter ID to edit:", "Edit Student")
            let newName = Microsoft.VisualBasic.Interaction.InputBox("Enter New Name:", "Edit Student")
            let newEmail = Microsoft.VisualBasic.Interaction.InputBox("Enter New Email:", "Edit Student")
            let ok, id = System.Int32.TryParse idStr
            if not ok then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.modifyStudent id newName newEmail with
                | Ok msg ->
                    let code = StatusCode.OK
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // Delete Student
        btnDeleteStudent.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter ID to delete:", "Delete Student")
            let ok, id = System.Int32.TryParse idStr
            if not ok then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.removeStudent id with
                | Ok msg ->
                    let code = StatusCode.OK
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // Add Grade
        btnAddGrade.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Student ID:", "Add Grade")
            let subject = Microsoft.VisualBasic.Interaction.InputBox("Enter Subject:", "Add Grade")
            let gradeStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Grade:", "Add Grade")
            let okId, id = System.Int32.TryParse idStr
            let okGrade, grade = System.Double.TryParse gradeStr
            if not okId || not okGrade then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID or Grade." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.addStudentGrade id subject grade with
                | Ok msg ->
                    let code = StatusCode.Created
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // Update Grade
        btnUpdateGrade.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Student ID:", "Update Grade")
            let subject = Microsoft.VisualBasic.Interaction.InputBox("Enter Subject:", "Update Grade")
            let gradeStr = Microsoft.VisualBasic.Interaction.InputBox("Enter New Grade:", "Update Grade")
            let okId, id = System.Int32.TryParse idStr
            let okGrade, grade = System.Double.TryParse gradeStr
            if not okId || not okGrade then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID or Grade." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.updateStudentGrade id subject grade with
                | Ok msg ->
                    let code = StatusCode.OK
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // Remove Grade
        btnRemoveGrade.Click.Add(fun _ ->
            let idStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Student ID:", "Remove Grade")
            let subject = Microsoft.VisualBasic.Interaction.InputBox("Enter Subject:", "Remove Grade")
            let okId, id = System.Int32.TryParse idStr
            if not okId then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Invalid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.removeStudentGrade id subject with
                | Ok msg ->
                    let code = StatusCode.OK
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) msg,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    ) |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )

        // View Statistics for a Student
        btnViewStudentStats.Click.Add(fun _ ->
            let input = Microsoft.VisualBasic.Interaction.InputBox("Enter Student ID:", "View Student Stats")
            let ok, id = System.Int32.TryParse input
            if not ok then
                let code = StatusCode.BadRequest
                MessageBox.Show(
                    sprintf "[%d] Please enter a valid ID." (int code),
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ) |> ignore
            else
                match StudentService.getStudentDetailsById id with
                | Ok student ->
                    // تمرير الطالب + حد النجاح للفورم
                    let statsForm = new StudentStatsForm(student, 50.0)
                    statsForm.ShowDialog() |> ignore
                | Error e ->
                    let code = toCode e
                    MessageBox.Show(
                        sprintf "[%d] %s" (int code) (toMsg e),
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
        )


        // View Statistics
        btnViewStats.Click.Add(fun _ ->
            let panelMain = new Panel(Dock = DockStyle.Fill)
            this.Controls.Add(panelMain)
            panelMain.Controls.Clear()

            let stats = Statistics.classStats 50.0 (StudentService.listAllStudents())
            let statsForm = new StatisticsForm(stats)
            statsForm.ShowDialog() |> ignore
        )

        // View Database Tables
        btnViewTables.Click.Add(fun _ ->
            let viewerForm = new StudentManagement.WinForms.DatabaseViewerForm()
            viewerForm.ShowDialog() |> ignore
        )

        // Logout
        btnLogout.Click.Add(fun _ ->
            AuthService.logout()
            this.Close()
        )
