namespace Color

open FrayTracer
open Visual
open Xunit
open FsUnit.Xunit
open FsUnitFix

[<Trait("Visual","Color")>]
type ``: ``() =

    [<Fact>]
    let ``When creating a color, it has the correct properties`` () =
        let result = rgb 1.0 2.0 3.0
        result |> should equal { R = 1.0; G = 2.0; B = 3.0 }

    [<Fact>]
    let ``When filtering a color, the resulting color has each channel limited by the range of the filter color`` () =
        let original = rgb 255.0 50.0 10.0
        let filterColor = rgb 30.0 127.7 0.0
        let result = filter original filterColor
        result |> should (equalWithin 0.1) { R = 30.0; G = 25.0; B = 0.0 }