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

type Queue<'A> = Null | Node of 'A * Queue<'A>
let mutable progQueue = Null
let mutable envQueue  = Null

let prevStepAction (but : Button) args =
  match progQueue with
  | Null                  -> ()
  | Node(value, nextProg) ->
    program   <- Some value
    progQueue <- nextProg
    match envQueue with
    | Null                  -> ()
    | Node(value', nextEnv) -> 
      env      <- value'
      envQueue <- nextEnv
    programLabel.Text <- sprintf "%A" program
    if progQueue = Null then but.Enabled <- false

let prevStepButton =
  let but = new Button()
  but.Text     <- "Previous Step"
  but.Size     <- Size(85, 23)
  but.Location <- System.Drawing.Point(programInput.Width - 2 * but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (prevStepAction but)
  but

let nextStepAction (but : Button) args =
  match program with 
  | None   -> MessageBox.Show("You reached end of program") |> ignore
  | Some p ->
    progQueue <- Node(p, progQueue)
    envQueue  <- Node(env, envQueue)
    let (nenv, np) = ss env p
    env     <- nenv
    program <- np
    programLabel.Text <- sprintf "%A" program
    prevStepButton.Enabled <- true

let nextStepButton =
  let but = new Button()
  but.Text     <- "Next Step"
  but.Location <- System.Drawing.Point(programInput.Width - but.Width, programInput.Height)
  but.Enabled  <- false
  but.Click.Add (nextStepAction but)
  but

let interpretAction args =
  let parseResult = &programInput.Text |> parse ()
  try
    program <- parseResult |> List.head |> fst |> Some
    env <- (fun s -> None)
    nextStepButton.Enabled <- true
    programLabel.Text <- sprintf "%A" program
  with
  | _ -> MessageBox.Show("Nothing to interpret") |> ignore

let interpretButton =
  let but = new Button()
  but.Text     <- "Interpret"
  but.Location <- System.Drawing.Point(0, programInput.Height)
  but.MouseClick.Add interpretAction 
  but

let mainForm =
  let form = new Form(Visible = false, TopMost = true)
  form.Size <- Size(700, 370)
  form.Controls.Add(interpretButton)
  form.Controls.Add(prevStepButton)
  form.Controls.Add(nextStepButton)
  form.Controls.Add(programInput)
  form.Controls.Add(programLabel)
  form

[<EntryPoint>]
let main argv = 
  mainForm.Visible <- true
  Application.Run()
  0