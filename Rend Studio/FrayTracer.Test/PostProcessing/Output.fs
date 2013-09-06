namespace Output

open FrayTracer
open Xunit
open FsUnit.Xunit
open Visual
open PostProcessing
open FsUnitFix
open System.Drawing

[<Trait("PostProcessing","Output")>]
type ``: ``() =

    [<Fact>]
    let ``When creating a bitmap from a frame it contains the image information from the frame`` () =
        let frame = { Pixels = Array2D.init 2 2 (fun x y -> rgb (float x) (float y) 0.0); RenderTime = 1L<ms> }
        let result = createBitmap frame
        result.GetPixel(0, 0) |> should equal (Color.FromArgb(0, 0, 0))
        result.GetPixel(0, 1) |> should equal (Color.FromArgb(0, 1, 0))
        result.GetPixel(1, 0) |> should equal (Color.FromArgb(1, 0, 0))
        result.GetPixel(1, 1) |> should equal (Color.FromArgb(1, 1, 0))