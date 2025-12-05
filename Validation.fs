namespace StudentManagement

open System

module Validation =
    let validateId (existingIds: int list) (id: int) =
        if id <= 0 then ["ID must be positive"]
        elif List.contains id existingIds then ["ID already exists"]
        else []

    let validateName (name: string) =
        if String.IsNullOrWhiteSpace(name) then ["Name cannot be empty"]
        elif name.Length < 2 then ["Name must be at least 2 characters"]
        else []

    let validateEmail (email: string) =
        if String.IsNullOrWhiteSpace(email) then ["Email cannot be empty"]
        elif not (email.Contains("@")) then ["Email must contain @"]
        else []

    let validateGrade (subject: string) (grade: float) =
        let subjectErrors =
            if String.IsNullOrWhiteSpace(subject) then ["Subject cannot be empty"] else []

        let gradeErrors =
            if grade < 0.0 || grade > 100.0 then ["Grade must be between 0 and 100"] else []

        subjectErrors @ gradeErrors