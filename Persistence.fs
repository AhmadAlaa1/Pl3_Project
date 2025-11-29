// Nedaa Hany

module Persistence

open System.IO
open System.Text.Json

type AppState =
    { Students : string list }

let saveToFile (path : string) (state : AppState) =
    let options = JsonSerializerOptions(WriteIndented = true)
    let json = JsonSerializer.Serialize(state, options)
    File.WriteAllText(path, json)

let loadFromFile (path : string) : AppState =
    if File.Exists(path) then
        let json = File.ReadAllText(path)
        let result = JsonSerializer.Deserialize<AppState>(json)
        match result with
        | null -> { Students = [] }  
        | state -> state
    else
        { Students = [] }

let initIfNotExists (path : string) =
    if not (File.Exists(path)) then
        saveToFile path { Students = [] }

let appendStudent (path : string) (student : string) =
    let state = loadFromFile path
    let newState =
        { state with Students = state.Students @ [ student ] }
    saveToFile path newState

let updateStudent (path : string) (oldName:string) (newName:string) =
    let state = loadFromFile path
    let updated =
        state.Students
        |> List.map (fun s -> if s = oldName then newName else s)

    saveToFile path { state with Students = updated }

let deleteStudent (path : string) (name:string) =
    let state = loadFromFile path
    let updated =
        state.Students
        |> List.filter (fun s -> s <> name)

    saveToFile path { state with Students = updated }

let clearFile (path : string) =
    saveToFile path { Students = [] }

