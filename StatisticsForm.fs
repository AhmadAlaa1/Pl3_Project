namespace StudentManagement

open System
open System.Windows.Forms
open System.Drawing

type StatisticsForm(stats: ClassStats) as this =
    inherit Form(Text = "Class Statistics", Width = 600, Height = 400)

    do
        // Main Panel
        let panelMain =
            new FlowLayoutPanel(
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20)
            )

        this.Controls.Add(panelMain)

        // Helper to format student option
        let formatStudentOpt (studentOpt: (int * string * float) option) =
            match studentOpt with
            | Some (id, name, avg) ->
                sprintf "ID: %d\nName: %s\nAverage: %.2f" id name avg
            | None ->
                "N/A"

        let passRateText =
            match stats.PassRate with
            | Some v -> sprintf "%.2f%%" (v * 100.0)
            | None -> "N/A"

        // Main Label
        let lblInfo =
            new Label(
                AutoSize = true,
                Font = new Font("Segoe UI", 10.0f),
                MaximumSize = new Size(this.ClientSize.Width - 40, 0)
            )

        lblInfo.Text <-
            sprintf
                "Student count: %d\n\nHighest Average Student:\n%s\n\nLowest Average Student:\n%s\n\nPass rate: %s"
                stats.StudentCount
                (formatStudentOpt stats.HighestStudent)
                (formatStudentOpt stats.LowestStudent)
                passRateText

        panelMain.Controls.Add(lblInfo)
