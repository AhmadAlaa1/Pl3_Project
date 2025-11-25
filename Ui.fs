// Youssef Yasser

// Example Code (Console):
//  module Ui =
//  let rec mainLoop (role : Role) (state : AppState) =
//  printfn "1) View all students"
//  printfn "2) View class statistics"
//  if role = Admin then
//  printfn "3) Add student"
//  printfn "4) Edit student"
//  printfn "5) Delete student"
//  printfn "0) Save & Exit"
//  let choice = System.Console.ReadLine()
//  match choice, role with
//  | "1", _ ->
//  mainLoop role state
//  | "2", _ ->
//  mainLoop role state
//  | "3", Admin ->
//  mainLoop role state
//  | "4", Admin ->
//  mainLoop role state
//  | "5", Admin ->
//  mainLoop role state
//  | "0", _ ->
//  Persistence.saveToFile "students.json" state
//  | _ ->
//  printfn "Invalid choice."
//  mainLoop role state