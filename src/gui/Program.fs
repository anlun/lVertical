open Stmt.Parser
open CoreParser
open Interpreter
open System.Drawing
open System.Windows.Forms

let programInput =
  let textBox = new TextBox()
  textBox.Location   <- System.Drawing.Point(0, 0)
  textBox.Multiline  <- true
  textBox.ScrollBars <- ScrollBars.Vertical
  textBox.Height <- 300
  textBox.Width  <- 300
  textBox

let programLabel =
  let lbl = new Label()
  lbl.Location <- System.Drawing.Point(programInput.Width, 0)
  lbl.AutoSize <- true
  lbl

let mutable env     : string -> Option<int> = fun (s : string) -> None
let mutable program : Option<Stmt.t> = None 
let mutable progMemory : Stmt.t list = []
let mutable envMenory : List<string -> Option<int>> = []

let nextStepAction (but : Button) args =
  match program with 
  | None   -> but.Enabled <- false
  | Some p ->
    let (nenv, np) = ss env p
    env     <- nenv
    program <- np
    programLabel.Text <- sprintf "%A" program
    envMenory <- env :: envMenory
    if program <> None then 
      progMemory <- program.Value::progMemory
    
let nextStepButton =
  let but = new Button()
  but.Text     <- "Next Step"
  but.Location <- System.Drawing.Point(programInput.Width - but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (nextStepAction but)
  but

  
let prevStepAction (but : Button) args =
  match progMemory.IsEmpty with 
  | true   -> but.Enabled <- false
  | false ->
    program <- Some progMemory.Head
    env <- envMenory.Head
    programLabel.Text <- sprintf "%A" progMemory.Head 
    if not progMemory.Tail.IsEmpty then 
      progMemory <- progMemory.Tail
    nextStepButton.Enabled <- true

let prevStepButton =
  let but = new Button()
  but.Text     <- "Prev Step"
  but.Location <- System.Drawing.Point(programInput.Width - 2*but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (prevStepAction but)
  but

let interpretAction args =
  let parseResult = &programInput.Text |> parse ()
  try
    program <- parseResult |> List.head |> fst |> Some
    env <- (fun s -> None)
    nextStepButton.Enabled <- true
    prevStepButton.Enabled <- true
    programLabel.Text <- sprintf "%A" program.Value
    progMemory <- []
    envMenory <- []
    progMemory <- program.Value::progMemory
    envMenory <- env::envMenory
  with
  | _ -> failwith "test"

let interpretButton =
  let but = new Button()
  but.Text <- "Interpret"
  but.Location <- System.Drawing.Point(0, programInput.Height)
  but.MouseClick.Add interpretAction  
  but

let mainForm =
  let form = new Form(Visible = false, TopMost = true)
  form.Controls.Add(interpretButton)
  form.Controls.Add(nextStepButton)
  form.Controls.Add(prevStepButton)
  form.Controls.Add(programInput)
  form.Controls.Add(programLabel)
  form

[<EntryPoint>]
let main argv = 
  mainForm.Visible <- true
  Application.Run()
  0
