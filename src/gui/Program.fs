open CoreParser
open Interpreter
open System.Drawing
open System.Windows.Forms

let programInput =
  let textBox = new TextBox()
  textBox.Location   <- System.Drawing.Point(0, 20)
  textBox.Multiline  <- true
  textBox.ScrollBars <- ScrollBars.Vertical
  textBox.Height <- 280
  textBox.Width  <- 300
  textBox

let programLabel =
  let lbl = new Label()
  lbl.Location <- System.Drawing.Point(programInput.Width, programInput.Location.Y)
  lbl.AutoSize <- true
  lbl.MinimumSize <- programInput.Size
  lbl


let mutable env     : string -> Option<int> = fun (s : string) -> None
let mutable program : Option<Stmt.t> = None 

let outputLabel : Label =
  let lbl = new Label()
  lbl.Location <- System.Drawing.Point(0, 350)
  lbl.AutoSize <- true
  lbl

let nextStepAction (but : Button) (outputLabel : Label) args =
  let writeOp e =
    let eText = (Expr.calc env e).ToString ()
    outputLabel.Text <- outputLabel.Text + "\n" + eText
    env
  match program with 
  | Some p ->
    let (nenv, np) = ss_prime writeOp env p
    env     <- nenv
    program <- np
    if (Option.isNone program) then but.Enabled <- false
    programLabel.Text <- sprintf "%A" program
  | None   ->
    MessageBox.Show("Nothing to interpret") |> ignore

let nextStepButton =
  let but = new Button()
  but.Text     <- "Next Step"
  but.Location <- System.Drawing.Point(programInput.Width - but.Width,
                                       programInput.Location.Y + programInput.Height)
  but.Enabled  <- false
  but.Click.Add (nextStepAction but outputLabel)
  but

let interpretAction args =
  let parseResult = parseValue Stmt.Parser.parser programInput.Text
  match parseResult with
  | Some _ ->
    program <- parseResult
    env <- (fun s -> None)
    nextStepButton.Enabled <- true
    programLabel.Text <- sprintf "%A" program
  | None ->
    MessageBox.Show("Incorrect input program text!") |> ignore

let interpretButton =
  let but = new Button()
  but.Text <- "Interpret"
  but.Location <- System.Drawing.Point(0, programInput.Location.Y + programInput.Height)
  but.MouseClick.Add interpretAction 
  but

let setupMenu (form : Form) =
  let menu = new MenuStrip()
  let fileMenuItem = new ToolStripMenuItem("&File")
  let openMenuItem = new ToolStripMenuItem("&Open L file")
  let settMenuItem = new ToolStripMenuItem("&Settings")
  let exitMenuItem = new ToolStripMenuItem("&Exit")
  menu.Items.Add(fileMenuItem) |> ignore
  menu.Items.Add(settMenuItem) |> ignore
  fileMenuItem.DropDownItems.Add(exitMenuItem) |> ignore
  fileMenuItem.DropDownItems.Add(openMenuItem) |> ignore
  exitMenuItem.Click.Add(fun _ -> form.Close ())
  openMenuItem.Click.Add(fun _ ->
    let dialog = new OpenFileDialog ()
    dialog.FileOk.Add (fun e -> 
      let filename = dialog.FileName
      use stream = new System.IO.StreamReader(filename)
      let programText = stream.ReadToEnd ()
      programInput.Text <- programText
    )
    let result = dialog.ShowDialog()
    printfn "%A" result
  )
  menu

let mainForm =
  let form = new Form(Visible = false, TopMost = true)
  form.Controls.Add(interpretButton)
  form.Controls.Add(nextStepButton)
  form.Controls.Add(programInput)
  form.Controls.Add(programLabel)
  form.Controls.Add(outputLabel)
  form.MainMenuStrip <- setupMenu form
  form.Controls.Add(form.MainMenuStrip)
  form.AutoSize <- true
  form


[<EntryPoint>]
let main argv = 
  mainForm.Visible <- true
  Application.Run()
  0