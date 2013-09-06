open FrayTracer
open Mathematics
open Geometry
open Engine
open Visual
open PostProcessing

[<EntryPoint>]
let main argv = 
    let t1 = triangle (vector 0.0 10.0 0.0) (vector 0.0 -10.0 0.0) (vector 10.0 0.0 0.0) { material with Color = red }
    let t2 = triangle (vector -10.0 10.0 0.0) (vector -10.0 -10.0 0.0) (vector 0.0 0.0 0.0) { material with Color = green }
    let t3 = triangle (vector 0.0 -10.0 0.0) (vector 0.0 -20.0 0.0) (vector 10.0 -5.0 0.0) { material with Color = blue }
    let s1 = sphere (vector 0.0 -30.0 0.0) 15.0 { material with Color = green }
    let p1 = plane (vector 0.0 0.0 -200.0) (vector 0.0 0.0 1.0) { material with Color = blue }
    let p2 = plane (vector -400.0 0.0 0.0) (vector 1.0 0.0 0.0) { material with Color = white }
    let p3 = plane (vector 400.0 0.0 0.0) (vector -1.0 0.0 0.0) { material with Color = red }
    let frame = raytrace { Width = 300; Height = 120 } { Position = (vector 0.0 0.0 150.0); Direction = (vector 0.0 0.0 -150.0) } { Lights = []; Items = [ t1; t2; t3; s1; p2 ] }
    frame |> saveToPng "result.png"

    0
