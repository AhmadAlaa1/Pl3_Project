namespace StudentManagement.WinForms

open System
open System.Windows.Forms
open Microsoft.Data.Sqlite
open System.Data

type DatabaseViewerForm() as this =
    inherit Form(Text = "Database Viewer", Width = 900, Height = 600, StartPosition = FormStartPosition.CenterScreen)

    let tabControl = new TabControl(Dock = DockStyle.Fill)

    let dgvStudents = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
    let dgvGrades = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
    let dgvAdmins = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)
    let dgvViewers = new DataGridView(Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill)

    do
        let studentsTab = new TabPage("Students")
        studentsTab.Controls.Add(dgvStudents)
        tabControl.TabPages.Add(studentsTab)

        let gradesTab = new TabPage("Grades")
        gradesTab.Controls.Add(dgvGrades)
        tabControl.TabPages.Add(gradesTab)

        let adminsTab = new TabPage("Admins")
        adminsTab.Controls.Add(dgvAdmins)
        tabControl.TabPages.Add(adminsTab)

        let viewersTab = new TabPage("Viewers")
        viewersTab.Controls.Add(dgvViewers)
        tabControl.TabPages.Add(viewersTab)

        this.Controls.Add(tabControl)

        let readerToDataTable (reader: SqliteDataReader) =
            let dt = new DataTable()
            for i in 0 .. reader.FieldCount - 1 do
                dt.Columns.Add(reader.GetName(i)) |> ignore
            while reader.Read() do
                let values = Array.init reader.FieldCount (fun i -> reader.GetValue(i))
                dt.Rows.Add(values) |> ignore
            dt

        let loadData() =
            use connection = new SqliteConnection("Data Source=students.db")
            connection.Open()

            use cmd = connection.CreateCommand()
            cmd.CommandText <- "SELECT Id, Name, Email FROM Students ORDER BY Id"
            use reader = cmd.ExecuteReader()
            dgvStudents.DataSource <- readerToDataTable reader

            use gradeCmd = connection.CreateCommand()
            gradeCmd.CommandText <- "SELECT StudentId, Subject, Grade FROM Grades ORDER BY StudentId, Subject"
            use gradeReader = gradeCmd.ExecuteReader()
            dgvGrades.DataSource <- readerToDataTable gradeReader

            use adminCmd = connection.CreateCommand()
            adminCmd.CommandText <- "SELECT Name, Password, Email, CreatedDate FROM Admins ORDER BY Id"
            use adminReader = adminCmd.ExecuteReader()
            dgvAdmins.DataSource <- readerToDataTable adminReader

            use viewerCmd = connection.CreateCommand()
            viewerCmd.CommandText <- "SELECT Name, Password, Email, CreatedDate FROM Viewers ORDER BY Id"
            use viewerReader = viewerCmd.ExecuteReader()
            dgvViewers.DataSource <- readerToDataTable viewerReader

        loadData()
