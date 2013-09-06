namespace PixelFilters

open FrayTracer
open Xunit
open FsUnit.Xunit
open PostProcessing
open Visual
open FsUnitFix

[<Trait("PostProcessing","PixelFilters")>]
type ``: ``() =

    [<Fact>]
    let ``Greyscaling a frame produces pixels with the proper grey value`` () =
        let frame = { Pixels = Array2D.create 2 2 red; RenderTime = 1L<ms> }
        let result = greyScale frame
        result.Pixels.[0, 0] |> should (equalWithin 0.1) (rgb 76.2 76.2 76.2)
        result.Pixels.[1, 0] |> should (equalWithin 0.1) (rgb 76.2 76.2 76.2)
        result.Pixels.[1, 1] |> should (equalWithin 0.1) (rgb 76.2 76.2 76.2)
        result.Pixels.[0, 1] |> should (equalWithin 0.1) (rgb 76.2 76.2 76.2)

    [<Fact>]
    let ``WGreyscaling a frame retains the original render time`` () =
        let frame = { Pixels = Array2D.create 2 2 red; RenderTime = 1L<ms> }
        let result = greyScale frame
        result.RenderTime |> should equal 1L<ms>