namespace StudentManagement

open StatusTypes

module AuthService =
    let mutable currentUser: User option = None

    let login (username: string) (password: string) : Result<User, StatusError> =
        match DatabaseAccess.getAllAdmins() with
        | Ok admins ->
            match admins |> List.tryFind (fun (name, pwd) -> name = username && pwd = password) with
            | Some (name, _) ->
                let user = { Id = 0; Name = name; Password = password; Role = Admin }
                currentUser <- Some user
                Ok user
            | None ->
                match DatabaseAccess.getAllViewers() with
                | Ok viewers ->
                    match viewers |> List.tryFind (fun (name, pwd) -> name = username && pwd = password) with
                    | Some (name, _) ->
                        let user = { Id = 1; Name = name; Password = password; Role = Viewer 1 }
                        currentUser <- Some user
                        Ok user
                    | None -> Error (ValidationError "Invalid username or password")
                | Error _ -> Error (ServerError "Database error")
        | Error _ -> Error (ServerError "Database error")

    let registerStudent (name: string) (email: string) : Result<int, StatusError> =
        match StudentService.findStudentByEmail email with
        | Some _ -> Error (StatusError.ConflictError "Email already exists")
        | None ->
            let students = StudentService.listAllStudents()
            let newId =
                if List.isEmpty students then 1
                else (students |> List.map (fun s -> s.Id) |> List.max) + 1

            match StudentService.createStudent newId name email with
            | Ok _ -> Ok newId
            | Error err -> Error err


    let registerViewer (name: string) (email: string) (password: string) : Result<int, StatusError> =
        match DatabaseAccess.getAllViewers() with
        | Ok viewers ->
            match viewers |> List.tryFind (fun (n, _) -> n = name) with
            | Some _ -> Error (ConflictError "Username already exists")
            | None ->
                match DatabaseAccess.addViewer name password email with
                | Ok () ->
                    let newUser = { Id = 1; Name = name; Password = password; Role = Viewer 1 }
                    currentUser <- Some newUser
                    Ok 1
                | Error _ -> Error (ServerError "Failed to add viewer to database")
        | Error _ -> Error (ServerError "Database error")

    let addNewAdmin (name: string) (password: string) (email: string) : Result<unit, StatusError> =
        match DatabaseAccess.addAdmin name password email with
        | Ok () -> Ok ()
        | Error _ -> Error (ServerError "Failed to add admin to database")

    let removeAdmin (name: string) : Result<unit, StatusError> =
        match DatabaseAccess.deleteAdmin name with
        | Ok () -> Ok ()
        | Error _ -> Error (ServerError "Failed to remove admin from database")

    let logout () =
        currentUser <- None

    let getCurrentUser () = currentUser

    let isAdmin () =
        match currentUser with
        | Some user -> user.Role = Admin
        | None -> false

    let isStudent () =
        match currentUser with
        | Some user ->
            match user.Role with
            | StudentUser _ -> true
            | _ -> false
        | None -> false

    let isViewer () =
        match currentUser with
        | Some user ->
            match user.Role with
            | Viewer _ -> true
            | _ -> false
        | None -> false

    let getStudentId () =
        match currentUser with
        | Some user ->
            match user.Role with
            | StudentUser id -> Some id
            | _ -> None
        | None -> None
