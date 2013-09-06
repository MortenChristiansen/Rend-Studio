namespace DomainTypes

open FrayTracer
open Visual
open Xunit
open FsUnit.Xunit
open Mathematics
open FsUnitFix
open System

[<Trait("DomainTypes","Ray")>]
type ``1: ``() =

    [<Fact>]
    let ``When creating a ray, it has the correct properties`` () =
        let result = ray (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0)
        result |> should equal { Origin = { X=1.0; Y=2.0; Z=3.0 }; Direction = { X=4.0; Y=5.0; Z=6.0 } }

    [<Fact>]
    let ``The string representation of the Ray is properly formatted`` () =
        let result = (ray (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0)).ToString()
        result |> should equal "{ 1.000; 2.000; 3.000 } -> { 4.000; 5.000; 6.000 }"

[<Trait("DomainTypes","Color")>]
type ``2: ``() =

    [<Fact>]
    let ``The string representation of a color is properly formatted`` () =
        let result = { R= 1.0; G = 2.0; B = 3.0 }.ToString()
        result |> should equal "{ 1; 2; 3 }"

    [<Fact>]
    let ``Adding two colors together results in a color with the components added`` () =
        let c1 = rgb 1.0 2.0 3.0
        let c2 = rgb 4.0 5.0 6.0
        let result = c1 + c2
        result |> should equal (rgb 5.0 7.0 9.0)

    [<Fact>]
    let ``Subtracting two colors results in a color with the components subtracted`` () =
        let c1 = rgb 1.0 2.0 3.0
        let c2 = rgb 4.0 5.0 6.0
        let result = c1 - c2
        result |> should equal (rgb -3.0 -3.0 -3.0)

    [<Fact>]
    let ``Multiplying a color with a coefficient results in a color with the components multiplied`` () =
        let c = rgb 1.0 2.0 3.0
        let result = c * 2.0
        result |> should equal (rgb 2.0 4.0 6.0)

    [<Fact>]
    let ``Multiplying a coefficient with a color results in a color with the components multiplied`` () =
        let c = rgb 1.0 2.0 3.0
        let result = 2.0 * c
        result |> should equal (rgb 2.0 4.0 6.0)