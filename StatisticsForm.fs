namespace StudentManagement

open System
open System.Windows.Forms
open System.Drawing

type StatisticsForm(stats: ClassStats) as this =
    inherit Form(Text = "Statistics", Width = 600, Height = 400)

    do
        // Panel علشان نضيف كل حاجة
        let panelMain = new FlowLayoutPanel(Dock = DockStyle.Fill, AutoScroll = true)
        this.Controls.Add(panelMain)

        // Labels
        let formatOptFloat (f: float option) =
            match f with
            | Some v -> sprintf "%.2f" v
            | None -> "N/A"

        let lblInfo = new Label(
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10.0f),
                        MaximumSize = new Size(this.ClientSize.Width - 40, 0)
                      )
        
        let highestAvgText = formatOptFloat stats.HighestAvg
        let lowestAvgText  = formatOptFloat stats.LowestAvg
        let passRateValue  = stats.PassRate |> Option.defaultValue 0.0

        lblInfo.Text <- sprintf 
            "Student count: %d\nHighest average: %s\nLowest average: %s\nPass rate: %.2f%%"
            stats.StudentCount
            highestAvgText
            lowestAvgText
            (passRateValue * 100.0)

        panelMain.Controls.Add(lblInfo)
