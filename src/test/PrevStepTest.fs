module PrevStepTest
open NUnit.Framework
open GUI
open CoreParser
open Stmt
open Expr
open VerticalPrinter

//[<TestCase ("x := 1; x := 2", Result = Some (Assign("x", Num 2)))>]
[<Test>]
let ``2 steps forward - 1 step back`` () =
  let s = "x := 1; x := 2"
  programInput.Text <- s
  interpretAction ()
  nextStepAction nextStepButton 
  nextStepAction nextStepButton 
  prevStepAction prevStepButton
  Assert.AreEqual (program, Some (Assign("x", Num 2)))
[<Test>]
let ``step forward - step back`` () =
  let s = "x := 1; x := 2"
  programInput.Text <- s
  interpretAction ()
  nextStepAction nextStepButton 
  prevStepAction prevStepButton
  Assert.AreEqual (program, Some (Seq (Assign("x", Num 1), Assign ("x", Num 2))))