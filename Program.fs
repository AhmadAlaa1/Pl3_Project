namespace StudentManagement.WinForms

open System
open System.Windows.Forms

module Program =
    [<STAThread>] // مهم جداً للـ WinForms
    [<EntryPoint>]
    let main _ =
        // تهيئة قاعدة البيانات
        StudentManagement.DatabaseAccess.initializeDatabase()

        // إعدادات WinForms
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)

        // تشغيل LoginForm كبداية
        Application.Run(new LoginForm())

        0 // قيمة الخروج
