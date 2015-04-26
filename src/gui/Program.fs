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

let mutable program : Option<Stmt.t> = None 

let mutable programL : Stmt.t list = []
let mutable envL     : (string -> Option<int>) list = []

let prevStepAction (but : Button)=
    program <- Some programL.Head
    programL <- programL.Tail
    envL <- envL.Tail
    if (programL.Length = 0) then 
      but.Enabled <- false
    programLabel.Text <- sprintf "%A" program

let prevStepButton =
  let but = new Button()
  but.Text     <- "Prev Step"
  but.Location <- System.Drawing.Point(programInput.Width - 2 * but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (fun e -> prevStepAction but)
  but.Name <- "Prev"
  but

let nextStepAction (but : Button) =
  match program with 
  | None   -> MessageBox.Show("The program has ended") |> ignore
  | Some p ->
    prevStepButton.Enabled <- true
    programL <- p :: programL

    let (nenv, np) = ss envL.Head p
    envL     <- nenv :: envL
    program <- np
    programLabel.Text <- sprintf "%A" program

let nextStepButton =
  let but = new Button()
  but.Text     <- "Next Step"
  but.Location <- System.Drawing.Point(programInput.Width - but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (fun e -> nextStepAction but)
  but

let interpretAction args =
  let parseResult = &programInput.Text |> parse ()
  nextStepButton.Enabled <- true
  program <- parseResult |> List.head |> fst |> Some
  envL <- [(fun s -> None)]

  programLabel.Text <- sprintf "%A" program

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