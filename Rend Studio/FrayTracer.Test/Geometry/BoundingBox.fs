namespace BoundingBox

open FrayTracer
open Xunit
open FsUnit.Xunit
open Mathematics
open Geometry
open FsUnitFix
open System

[<Trait("Geometry","BoundingBox")>]
type ``: ``() =

    [<Fact>]
    let ``When creating a bounding box, it has the correct properties`` () =
        let result = boundingbox (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0)
        result |> should equal { Position = { X=1.0; Y=2.0; Z=3.0 }; Size = { X=4.0; Y=5.0; Z=6.0 } }

    [<Fact>]
    let ``The string representation of the BoundingBox is properly formatted`` () =
        let result = (boundingbox (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0)).ToString()
        result |> should equal "[{ 1.000; 2.000; 3.000 } -> { 4.000; 5.000; 6.000 }]"

    [<Fact>]
    let ``The intersection volume of two overlapping bounding boxes is correctly calculated`` () =
        let box1 = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let box2 = boundingbox (vector 1.5 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = intersectVolume box1 box2
        result |> should equal 0.5

    [<Fact>]
    let ``The intersection volume of two non-overlapping bounding boxes is 0`` () =
        let box1 = boundingbox (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0)
        let box2 = boundingbox (vector 20.0 2.0 3.0) (vector 4.0 5.0 6.0)
        let result = intersectVolume box1 box2
        result |> should equal 0.0

    [<Fact>]
    let ``A bounding box which has a width of 0 in a dimension gets a small width`` () =
        let result = boundingbox (vector 0.0 0.0 0.0) (vector 1.0 1.0 0.0)
        result |> should equal { Position = { X=0.0; Y=0.0; Z=(-0.01) }; Size = { X=1.0; Y=1.0; Z=0.01 } }