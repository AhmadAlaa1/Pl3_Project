namespace StudentManagement

// ========== DOMAIN TYPES ==========
type Student = {
    Id: int
    Name: string
    Email: string
    Grades: Map<string, float>
}

type OperationResult =
    | Success
    | Failure of string list