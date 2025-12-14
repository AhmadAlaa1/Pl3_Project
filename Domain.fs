namespace StudentManagement

type Student = {
    Id: int
    Name: string
    Email: string
    Grades: Map<string, float>
}

type OperationResult =
    | Success
    | Failure of string list

type UserRole =
    | Admin
    | StudentUser of int
    | Viewer of int  

type User = {
    Id: int
    Name: string
    Password: string
    Role: UserRole
}

type Grade = { 
    Subject: string; 
    Score: float 
}

type AuthState = {
    CurrentUser: User option
    IsLoggedIn: bool
}

type ClassStats = {
    StudentCount : int
    HighestStudent : (int * string * float) option
    LowestStudent  : (int * string * float) option
    PassRate : float option
}
