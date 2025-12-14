namespace StudentManagement.WinForms

open System
open System.Windows.Forms

module Program =
    [<STAThread>] 
    [<EntryPoint>]
    let main _ =
        StudentManagement.DatabaseAccess.initializeDatabase()

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)

        Application.Run(new LoginForm())

        0 
