namespace StudentManagement.WinForms

open System
open System.Windows.Forms
open StudentManagement

type RegisterForm() as this =
    inherit Form(Text = "Register Viewer",
                 Width = 350,
                 Height = 250,
                 FormBorderStyle = FormBorderStyle.FixedDialog,
                 MaximizeBox = false,
                 StartPosition = FormStartPosition.CenterScreen)

    let lblName = new Label(Text="Name:", Top=20, Left=20, Width=100)
    let txtName = new TextBox(Top=40, Left=20, Width=280)

    let lblEmail = new Label(Text="Email:", Top=70, Left=20, Width=100)
    let txtEmail = new TextBox(Top=90, Left=20, Width=280)

    let lblPassword = new Label(Text="Password:", Top=120, Left=20, Width=100)
    let txtPassword = new TextBox(Top=140, Left=20, Width=280, PasswordChar='*')

    let btnRegister = new Button(Text="Register", Top=180, Left=20, Width=120)
    let btnCancel = new Button(Text="Cancel", Top=180, Left=160, Width=120)

    do
        this.Controls.AddRange([| lblName; txtName; lblEmail; txtEmail; lblPassword; txtPassword; btnRegister; btnCancel |])

        btnRegister.Click.Add(fun _ ->
            let name = txtName.Text.Trim()
            let email = txtEmail.Text.Trim()
            let password = txtPassword.Text.Trim()

            if String.IsNullOrWhiteSpace name then
                let code = StatusTypes.StatusCode.BadRequest
                MessageBox.Show(sprintf "[%d] Name is required" (int code),
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning) |> ignore
            elif String.IsNullOrWhiteSpace email then
                let code = StatusTypes.StatusCode.BadRequest
                MessageBox.Show(sprintf "[%d] Email is required" (int code),
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
                match AuthService.registerViewer name email password with
                | Ok _ ->
                    MessageBox.Show(sprintf "[%d] Registration successful!" (int StatusTypes.StatusCode.Created),
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information) |> ignore
                    this.Close()
                | Error err ->
                    let code = StatusTypes.toCode err
                    let msg = StatusTypes.toMsg err
                    MessageBox.Show(sprintf "[%d] %s" (int code) msg,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error) |> ignore
        )

        btnCancel.Click.Add(fun _ ->
            this.Close()
        )
