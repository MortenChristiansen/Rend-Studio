namespace Geometry

open FrayTracer
open Mathematics

[<AutoOpen>]
module BoundingBox =

    let boundingbox position size =
        let adjustVector vector size adjustment =
            match size with
            | { X = 0.0 } -> { vector with X = vector.X + adjustment }
            | { Y = 0.0 } -> { vector with Y = vector.Y + adjustment }
            | { Z = 0.0 } -> { vector with Z = vector.Z + adjustment }
            | _ -> vector

        { Position = adjustVector position size -0.01; Size = adjustVector size size 0.01 }

    let intersectVolume (box1:BoundingBox) (box2:BoundingBox) =
        let farEdge box axis =
            box.Size.[axis] + box.Position.[axis]

        let length axis =
            let len1 = box1.Position.[axis] - box2.Position.[axis]
            let len2 = (farEdge box2 axis) - (farEdge box1 axis)
            let len3 = box2.Size.[axis] - (max 0.0 len1) - (max 0.0 len2)
            max 0.0 len3

        (length 0) * (length 1) * (length 2)