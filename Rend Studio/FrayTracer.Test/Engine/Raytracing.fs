namespace Raytracing

open FrayTracer
open Xunit
open FsUnit.Xunit
open Mathematics
open Geometry
open Engine
open FsUnitFix
open System
open System.Drawing

[<Trait("Engine","Raytracing")>]
type ``: ``() =

    let emptyScene = { Lights = []; Items = [] }
    let redMaterial = { material with Color = red }
    let greenMaterial = { material with Color = green }
    let blueMaterial = { material with Color = blue }
    let whiteMaterial = { material with Color = white }

    [<Fact>]
    let ``When tracing a scene, the resulting frame has the correct size`` () =
        let result = raytrace { Width = 10; Height = 5 } { Position = (vector 0.0 0.0 0.0); Direction = (vector 0.0 0.0 0.0) } emptyScene
        result.Pixels.GetLength 0 |> should equal 10
        result.Pixels.GetLength 1 |> should equal 5

    [<Fact>]
    let ``When tracing an empty scene, the resulting frame has only black pixels`` () =
        let frame = raytrace { Width = 10; Height = 5 } { Position = (vector 0.0 0.0 0.0); Direction = (vector 0.0 0.0 0.0) } emptyScene
        let flatten (A:'a[,]) = A |> Seq.cast<'a>
        let hasNonBlackPixels = frame.Pixels |> flatten |> Seq.exists (fun v -> v <> black)
        hasNonBlackPixels |> should be False

    [<Fact>]
    let ``When tracing a scene, the resulting frame has the correct pixels`` () =
        let t1 = triangle (vector 0.0 0.0 0.0) (vector 100.0 0.0 0.0) (vector 0.0 100.0 0.0) redMaterial
        let t2 = triangle (vector 0.0 0.0 0.0) (vector 0.0 100.0 0.0) (vector -100.0 0.0 0.0) greenMaterial
        let t3 = triangle (vector 0.0 0.0 0.0) (vector -100.0 0.0 0.0) (vector 0.0 -100.0 0.0) blueMaterial
        let t4 = triangle (vector 0.0 0.0 0.0) (vector 0.0 -100.0 0.0) (vector 100.0 0.0 0.0) whiteMaterial
        let scene = { Lights = []; Items = [ t1; t2; t3; t4 ] }
        let result = raytrace { Width = 2; Height = 2 } { Position = (vector 0.0 0.0 5.0); Direction = (vector 0.0 0.0 -1.0) } scene
        result.Pixels.[0,0] |> should equal green
        result.Pixels.[0,1] |> should equal blue
        result.Pixels.[1,0] |> should equal red
        result.Pixels.[1,1] |> should equal white

    [<Fact>]
    let ``?`` () =
        let t = triangle (vector -10.0 0.0 10.0) (vector 10.0 5.0 -10.0) (vector 10.0 -5.0 -10.0) redMaterial
        let l = light (vector 20.0 0.0 0.0) 1.0 whiteMaterial
        let scene = { Lights = [ l ]; Items = [ t ] }
        let result = raytrace { Width = 1; Height = 1 } { Position = (vector 0.0 0.0 5.0); Direction = (vector 0.0 0.0 -1.0) } scene
        result.Pixels.[0,0] |> should equal white // ish
        //colorAcc += hitPrimitive.Material.Color * (((PositionedLight)hitPrimitive).Brightness / (distance * distance));
        // 1: add partial color from primitive
        // 2: limit the color to the color spectrum of the light source
        // 3: continue tracing recursively, possibly adding more lights... in future, translucency should be handled for non-blocking light sources

    // Test that HitLigght and HitLightFromInsidePrimitive are handled correctly
    // Test that the recursion depth is used as expected
    // Test that the light model is correctly applied
    // Test that colors are only applied if a light can be traced