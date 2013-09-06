namespace Geometry

open FrayTracer
open Mathematics

[<AutoOpen>]
module Primitive =

    let primitive position boundingBox getNormal intersects intersect getColor material =
        { Position = position; BoundingBox = boundingBox; GetNormal = getNormal; Intersects = intersects; Intersect = intersect; GetColor = getColor; Material = material }

    let reflect (reflectedPrimitive:Primitive) originalRayDirection intersection =
        let primitiveNormal = reflectedPrimitive.GetNormal intersection
        let reflectedDirection = normalize ( originalRayDirection - 2.0 * (originalRayDirection * primitiveNormal) * primitiveNormal )
        ray (intersection + reflectedDirection * Vector.Epsilon) reflectedDirection


    // ========== Triangle Primitive ========== //

    let triangle a b c material =

        let boundingBox = boundingbox (vector (List.min [a.X; b.X; c.X ]) (List.min [a.Y; b.Y; c.Y ]) (List.min [a.Z; b.Z; c.Z ])) (vector (List.max [a.X; b.X; c.X ]) (List.max [a.Y; b.Y; c.Y ]) (List.max [a.Z; b.Z; c.Z ]))

        let rightVertex = c - a
        let leftVertex = b - a
        let normal = -(normal rightVertex leftVertex)
        let k = match normal with
                | n when (abs n.X) > (abs n.Y) && (abs n.X) > (abs n.Z) -> 0
                | n when (abs n.Y) >= (abs n.X) && (abs n.Y) > (abs n.Z) -> 1
                | _ -> 2
        let yAxis = (k + 1) % 3
        let zAxis = (k + 2) % 3

        let krec = 1.0 / normal.[k]
        let normalYAxis = normal.[yAxis] * krec
        let normalZAxis = normal.[zAxis] * krec
        let normalD = (normal * a) * krec

        let reci = 1.0 / (rightVertex.[yAxis] * leftVertex.[zAxis] - rightVertex.[zAxis] * leftVertex.[yAxis])
        let bNormalY = rightVertex.[yAxis] * reci
        let bNormalZ = -rightVertex.[zAxis] * reci
        let cNormalY = leftVertex.[zAxis] * reci
        let cNormalZ = -leftVertex.[yAxis] * reci

        
        let getNormal (intersection:Vector) =
            normal

        let intersects (box:BoundingBox) =
            (intersectVolume boundingBox box) > 0.0

        let intersect ray distance =
            let o = ray.Origin
            let d = ray.Direction
            let lnd = 1.0 / (d.[k] + normalYAxis * d.[yAxis] + normalZAxis * d.[zAxis])
            let distToPlane = (normalD - o.[k] - normalYAxis * o.[yAxis] - normalZAxis * o.[zAxis]) * lnd
            match distance <= distToPlane || distToPlane <= 0.0 with
            | true -> Miss
            | _ ->
                let distanceY = o.[yAxis] + distToPlane * d.[yAxis] - a.[yAxis]
                let distanceZ = o.[zAxis] + distToPlane * d.[zAxis] - a.[zAxis]
                let β = distanceZ * bNormalY + distanceY * bNormalZ
                match β < 0.0 || β > 1.0 with
                | true -> Miss
                | _ ->
                    let γ = distanceY * cNormalY + distanceZ * cNormalZ
                    match γ < 0.0 || (β + γ) > 1.0 with
                    | true -> Miss
                    | _ ->
                        match d * normal > 0.0 with
                        | true -> distToPlane |> HitFromInsidePrimitive
                        | false -> distToPlane |> Hit

        let getColor intersection =
            material.Color
        
        primitive a boundingBox getNormal intersects intersect getColor material

    // ========== Sphere Primitive ========== //

    let sphere p radius material =
        
        let halfSize = vector radius radius radius
        let boundingBox = boundingbox (p - halfSize) (p + halfSize)

        let getNormal (intersection:Vector) =
            normalize (intersection - p)

        let intersects (box:BoundingBox) =
            (intersectVolume boundingBox box) > 0.0

        let intersect ray distance =
            let v = ray.Origin - p
            let b = -(v * ray.Direction)
            let det = b * b - v * v + radius * radius
            match det <= 0.0 with
            | true -> Miss
            | _ -> 
                let detSq = sqrt det
                let i1 = b - detSq
                let i2 = b + detSq
                match i2 <= 0.0 with
                | true -> Miss
                | false when i1 >= 0.0 && i1 < distance -> i1 |> Hit
                | false when i1 < 0.0 && i2 < distance -> i2 |> HitFromInsidePrimitive
                | _ -> Miss

        let getColor intersection =
            material.Color

        primitive p boundingBox getNormal intersects intersect getColor material

    // ========== Plane Primitive ========== //

    let plane p normal material =
        
        let boundingBox = { Position = (vector 0.0 0.0 0.0); Size = (vector 0.0 0.0 0.0) }

        let getNormal (intersection:Vector) =
            normal

        let intersects (box:BoundingBox) =
            true

        let intersect (ray:Ray) distance =
            let d = p * normal
            let dot = normal * ray.Direction
            match dot with
            | 0.0 -> Miss
            | _ -> 
                let dist = (d - (normal * ray.Origin)) / dot
                match dist <= 0.0 || dist >= distance with
                | true -> Miss
                | false when dot < 0.0 -> dist |> Hit
                | _ -> dist |> HitFromInsidePrimitive

        let getColor intersection =
            material.Color

        primitive p boundingBox getNormal intersects intersect getColor material

    // ========== Light Primitive ========== //

    let light p radius material =
        
        sphere p radius material