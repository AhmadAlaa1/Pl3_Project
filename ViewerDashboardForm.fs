namespace StudentManagement.WinForms

open System
open System.Windows.Forms
open System.Drawing
open StudentManagement
open StudentManagement.StatusTypes

type ViewerDashboardForm() as this =
    inherit Form(
        Text = "Viewer Dashboard",
        Width = Screen.PrimaryScreen.WorkingArea.Width,
        Height = Screen.PrimaryScreen.WorkingArea.Height,
        StartPosition = FormStartPosition.Manual,  // مهم علشان نحدد الـ bounds
        Left = 0,
        Top = 0
    )
    // تعريف الـ controls فقط
    let panel = new Panel(Dock = DockStyle.Left, Width = 200)
    let panelMain = new Panel(Dock = DockStyle.Fill)

    let btnViewAll = new Button(Text="View All Students", Top=10, Left=10, Width=180, Height=40)
    let btnViewById = new Button(Text="View Student by ID", Top=60, Left=10, Width=180, Height=40)
    let btnViewStudentStats = new Button(Text="View Student Stats", Top=110, Left=10, Width=180, Height=40)
    let btnStats = new Button(Text="View Statistics", Top=160, Left=10, Width=180, Height=40)
    let btnLogout = new Button(Text="Logout", Top=210, Left=10, Width=180, Height=40)

    // كل الكود اللي ينفذ فور إنشاء الفورم يحطه داخل do block
    do
        panel.Controls.AddRange([| btnViewAll; btnViewById; btnViewStudentStats; btnStats; btnLogout |])
        this.Controls.AddRange([| panelMain; panel |])

        // ================================
        // View all students
        btnViewAll.Click.Add(fun _ ->
            panelMain.Controls.Clear()
            let dgv = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
            panelMain.Controls.Add(dgv)

            let students = StudentService.listAllStudents()
            let dt = new System.Data.DataTable()
            dt.Columns.Add("Id") |> ignore
            dt.Columns.Add("Name") |> ignore
            dt.Columns.Add("Email") |> ignore

            for s in students do
                dt.Rows.Add(s.Id, s.Name, s.Email) |> ignore

            dgv.DataSource <- dt
        )

        // ================================
        // View student by ID
        btnViewById.Click.Add(fun _ ->
            use idForm = new Form(Text = "Enter Student ID", Width = 250, Height = 120, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent)
            let lbl = new Label(Text = "Student ID:", Top = 10, Left = 10, AutoSize = true)
            let txt = new TextBox(Top = 30, Left = 10, Width = 200)
            let btnOk = new Button(Text = "View", Top = 60, Left = 10, Width = 80, DialogResult = DialogResult.OK)
            idForm.Controls.AddRange [| lbl; txt; btnOk |]
            idForm.AcceptButton <- btnOk

            if idForm.ShowDialog() = DialogResult.OK then
                let ok, id = Int32.TryParse(txt.Text.Trim())
                if not ok then
                    let code = StatusTypes.StatusCode.BadRequest
                    MessageBox.Show(
                        sprintf "[%d] Please enter a valid ID." (int code),
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    ) |> ignore
                else
                    match StudentService.getStudentDetailsById id with
                    | Ok s ->
                        panelMain.Controls.Clear()

                        let lblInfo = new Label(
                            Text = sprintf "Name: %s   Email: %s" s.Name s.Email,
                            Dock = DockStyle.Top,
                            Height = 30,
                            Font = new Font("Segoe UI", 10.0f, FontStyle.Bold)
                        )
                        panelMain.Controls.Add(lblInfo)

                        let dgvPanel = new Panel(Dock = DockStyle.Fill, Padding = new Padding(0, 40, 0, 0))
                        let dgv = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
                        dgvPanel.Controls.Add(dgv)
                        panelMain.Controls.Add(dgvPanel)

                        let dt = new System.Data.DataTable()
                        dt.Columns.Add("Subject") |> ignore
                        dt.Columns.Add("Grade") |> ignore

                        if s.Grades.IsEmpty then dt.Rows.Add("No grades", "") |> ignore
                        else for kv in s.Grades do dt.Rows.Add(kv.Key, kv.Value) |> ignore

                        dgv.DataSource <- dt

                    | Error e ->
                        let code = StatusTypes.toCode e
                        MessageBox.Show(
                            sprintf "[%d] %s" (int code) (StatusTypes.toMsg e),
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


        // ================================
        // View Statistics
        btnStats.Click.Add(fun _ ->
            panelMain.Controls.Clear()
            let stats = Statistics.classStats 75.0 (StudentService.listAllStudents())
            let statsForm = new StatisticsForm(stats)
            statsForm.ShowDialog() |> ignore
        )

        // ================================
        // Logout
        btnLogout.Click.Add(fun _ ->
            AuthService.logout()
            this.Close()
        )
