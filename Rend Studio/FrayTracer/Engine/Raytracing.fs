namespace Engine

open FrayTracer
open Visual
open Mathematics
open Geometry
open System
open System.Diagnostics

[<AutoOpen>]
module Raytracing =

    let raytrace size (cam:Camera) rayMappings colorMappings scene =

        // Phong lighting
//        let adjustColor color pHit pLight intersection =
//            let specularPower = 20.0
//
//            let lightDirection = normalize (pLight.Position - intersection)
//            let pHitNormal = pHit.GetNormal intersection
//            let reflectedDirection = lightDirection - 2.0 * (lightDirection * pHitNormal) * pHitNormal
//            let viewRay = intersection - cam.Position
//            let phongTerm = max (normalize reflectedDirection * normalize viewRay) 0.0
//            let phongTermPow = (phongTerm ** specularPower)
//            color + (pLight.GetColor (vector 0.0 0.0 0.0)) * phongTermPow

        let sw = Stopwatch.StartNew()

        let findNearest ray =
            let intersect p =
                match p.Intersect ray Double.MaxValue with
                | Hit d -> Some (p,d)
                | HitFromInsidePrimitive d -> Some (p,d)
                | Miss -> None

            match scene.Items |> List.map (fun p -> intersect p) |> Seq.choose id |> Seq.toList with
            | [] -> None
            | pairs -> pairs |> List.minBy (fun x -> snd x) |> Some

        

        let traceRay r =
            let maxLevels = 5;

            let createRaySequence mapping p i r = seq {
                match mapping p i r with
                | None
                | Some (r, c) -> yield createRaySequence mapping 
            }
                

            let rays = rayMappings |> Seq.map (fun m -> Seq.initInfinite (fun i -> m )) |> Seq.

            let raySequence = seq {
                match 
            }

            let getColor p intersection =
                let traceLight p intersection light =
                    let r = ray intersection (normalize light.Position - intersection)
                    //findNearest r
                    (p, Miss)

                scene.Lights |> List.map (fun l -> traceLight p intersection l) |> List.fold (fun c s -> adjustColor c p (fst s) intersection) black

            match findNearest r with
            | Some (p, d) -> getColor p (r.Origin + r.Direction * d) 
            | None -> black

        let camXAxis = normal cam.Direction (vector 0.0 1.0 0.0)
        let camYAxis = normal cam.Direction camXAxis

        let createRay x y =
            let dX = camXAxis * (x + 0.5 - (float size.Width) / 2.0)
            let dY = camYAxis * (y + 0.5 - (float size.Height) / 2.0)
            ray cam.Position (normalize (cam.Direction + dX + dY))

        let renderColumn x =
            Array.init size.Height (fun y -> createRay (float x) (float y) |> traceRay)

        // Parallel rendering
        let pixels = 
            Async.Parallel [ for x in 0 .. size.Width - 1 -> async { return renderColumn x }]
            |> Async.RunSynchronously |> array2D

        // Serial rendering
//        let pixels = 
//            [ for x in 0 .. size.Width - 1 -> renderColumn x ]
//            |> array2D

        sw.Stop()

        { Pixels = pixels; RenderTime = (sw.ElapsedMilliseconds * 1L<ms>) }