namespace StudentManagement.WinForms

open System
open System.Windows.Forms
open StudentManagement

type LoginForm() as this =
    inherit Form
        (
            Text = "Login",
            Width = 350,
            Height = 220,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            StartPosition = FormStartPosition.CenterScreen
        )

    // --- Controls ---
    let lblUsername = new Label(Text = "Username:", Top = 20, Left = 20, Width = 100)
    let txtUsername = new TextBox(Top = 40, Left = 20, Width = 280)

    let lblPassword = new Label(Text = "Password:", Top = 70, Left = 20, Width = 100)
    let txtPassword = new TextBox(Top = 90, Left = 20, Width = 280, PasswordChar = '*')

    let btnLogin = new Button(Text = "Login", Top = 130, Left = 20, Width = 120)
    let btnRegister = new Button(Text = "Register Viewer", Top = 130, Left = 160, Width = 140)

    do
        this.Controls.AddRange([| lblUsername; txtUsername; lblPassword; txtPassword; btnLogin; btnRegister |])

        // --- Event Handlers ---
        btnLogin.Click.Add(fun _ ->
            let username = txtUsername.Text.Trim()
            let password = txtPassword.Text.Trim()

            // التحقق من الحقول الفارغة
            if String.IsNullOrWhiteSpace username then
                let code = StatusTypes.StatusCode.BadRequest
                MessageBox.Show(sprintf "[%d] Username is required" (int code),
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning) |> ignore
            elif String.IsNullOrWhiteSpace password then
                let code = StatusTypes.StatusCode.BadRequest
                MessageBox.Show(sprintf "[%d] Password is required" (int code),
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning) |> ignore
            else
                // محاولة تسجيل الدخول
                match AuthService.login username password with
                | Ok user ->
                    // تسجيل دخول ناجح
                    MessageBox.Show(sprintf "[%d] Login successful!" (int StatusTypes.StatusCode.OK),
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information) |> ignore

                    if AuthService.isAdmin() then
                        let dashboard = new AdminDashboardForm()
                        this.Hide()
                        dashboard.ShowDialog() |> ignore
                        this.Show()
                    else
                        let viewerDashboard = new ViewerDashboardForm()
                        this.Hide()
                        viewerDashboard.ShowDialog() |> ignore
                        this.Show()

                | Error err ->
                    // تحويل StatusError للرسالة والكود
                    let code = StatusTypes.StatusCode.Unauthorized
                    let msg = StatusTypes.toMsg err
                    MessageBox.Show(sprintf "[%d] %s" (int code) msg,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error) |> ignore
        )

        btnRegister.Click.Add(fun _ ->
            use registerForm = new RegisterForm()
            registerForm.ShowDialog() |> ignore
        )
