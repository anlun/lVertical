module GUI
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

let mutable programAr : Option<Stmt.t>[] = [||]
let mutable envAr     : (string -> Option<int>)[] = [||]

let mutable nextStEnabled = true
//let mutable prevStEnabled = false
let prevStepAction (but : Button) args =
//  match program with 
//  | None   -> but.Enabled <- false
//  | Some p ->
    program <- programAr.[programAr.Length - 1]
    env     <- envAr.[envAr.Length - 1]
    programAr <- programAr.[0 .. programAr.Length - 2]
    envAr <- envAr.[0 .. envAr.Length - 2]
    nextStEnabled <- true
    if (programAr.Length = 0) then 
      but.Enabled <- false
    //  prevStVisible <- false
    programLabel.Text <- sprintf "%A" program

let prevStepButton =
  let but = new Button()
  but.Text     <- "Prev Step"
  but.Location <- System.Drawing.Point(programInput.Width - 2 * but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (prevStepAction but)
  but

let nextStepAction (but : Button) args =
  match program with 
  | None   -> 
    but.Enabled <- false
    nextStEnabled <- false
  | Some p ->

    prevStepButton.Enabled <- true
    programAr <- Array.append programAr [|program|]
    envAr <- Array.append envAr [|env|]

    let (nenv, np) = ss env p
    env     <- nenv
    program <- np
    programLabel.Text <- sprintf "%A" program

let nextStepButton =
  let but = new Button()
  but.Text     <- "Next Step"
  but.Location <- System.Drawing.Point(programInput.Width - but.Width, programInput.Height)
  but.Enabled  <- false
  nextStEnabled <- false
  but.Click.Add (nextStepAction but)
  but

let interpretAction args =
  let parseResult = &programInput.Text |> parse ()
  try
    program <- parseResult |> List.head |> fst |> Some
    env <- (fun s -> None)

    nextStepButton.Enabled <- true
    nextStEnabled <- true

    programLabel.Text <- sprintf "%A" program
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
  form.MouseMove.Add (fun e ->
 //   prevStepButton.Visible <- prevStVisible
    nextStepButton.Enabled <- nextStEnabled
  )
  form

[<EntryPoint>]
let main argv = 
  mainForm.Visible <- true
  Application.Run()
  0