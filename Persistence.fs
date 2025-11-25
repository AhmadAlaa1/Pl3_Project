// Nedaa Hany

// Example Code:
//  module Persistence =
//  openopenSystem.IO
//  System.Text.Json
//  let saveToFile (path : string) (state : AppState) =
//  let options = JsonSerializerOptions(WriteIndented = true)
//  let json = JsonSerializer.Serialize(state, options)
//  File.WriteAllText(path, json)
//  let loadFromFile (path : string) : AppState =
//  if File.Exists(path) then
//  let json = File.ReadAllText(path)
//  JsonSerializer.Deserialize<AppState>(json)
//  else
//  { Students = [] }