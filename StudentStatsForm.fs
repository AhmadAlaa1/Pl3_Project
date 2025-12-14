namespace StudentManagement.WinForms
open StudentManagement

open System
open System.Windows.Forms
open System.Drawing
open GradeCalc

type StudentStatsForm(student: Student, passThreshold: float) as this =
    inherit Form(Text = sprintf "Student: %s" student.Name, Width = 400, Height = 400)

    do
        let panelMain = new FlowLayoutPanel(Dock = DockStyle.Fill, AutoScroll = true, Padding = Padding(20))
        this.Controls.Add(panelMain)

        let avgText =
            match GradeCalc.averageScore student.Grades with
            | Some avg -> sprintf "%.2f" avg
            | None -> "N/A"

        let statusText = GradeCalc.studentStatus passThreshold student

        let gradesText =
            if student.Grades.IsEmpty then "No grades"
            else
                student.Grades
                |> Map.toList
                |> List.map (fun (subject, score) -> sprintf "%s: %.2f" subject score)
                |> String.concat "\n"

        let lblInfo =
            new Label(
                AutoSize = true,
                Font = new Font("Segoe UI", 10.0f),
                MaximumSize = Size(this.ClientSize.Width - 40, 0)
            )

        lblInfo.Text <- sprintf
            "ID: %d\nName: %s\n\nGrades:\n%s\n\nAverage: %s\nStatus: %s"
            student.Id
            student.Name
            gradesText
            avgText
            statusText

        panelMain.Controls.Add(lblInfo)
