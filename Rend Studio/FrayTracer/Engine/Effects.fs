namespace Engine

open FrayTracer
open Mathematics
open Visual
open Geometry

[<AutoOpen>]
module Effects =

    // Color mapping parameters:
    // p: Primitive hit
    // i: Vector to intersection
    // c: camera
    // l: light 

    // Default flat coloring - should not be used with other color mappings
    let flatColorMapping p i c l =
        p.Material.Color

    // Specular coloring
    let phongColorMapping p i c l =
        match p.Material.Specular > 0.0 with
        | false -> black
        | _ ->
            let specularPower = 20.0

            let lightDirection = normalize (l.Position - i)
            let pHitNormal = p.GetNormal i
            let reflectedDirection = lightDirection - 2.0 * (lightDirection * pHitNormal) * pHitNormal
            let viewRay = i - c.Position
            let phongTerm = max (normalize reflectedDirection * normalize viewRay) 0.0
            let phongTermPow = (phongTerm ** specularPower) * p.Material.Specular
            (l.GetColor (vector 0.0 0.0 0.0)) * phongTermPow // Only returns extra color, not accumulated color

    // Diffuse coloring
    let lambertColorMapping p i c l =
        match p.Material.Diffuse > 0.0 with
        | false -> black
        | _ ->
            let hitPrimitiveColor = p.GetColor i
            let lightDirection = normalize (l.Position - i)
            let hitPrimitiveNormal = p.GetNormal i
            let appliedColor = filter l.Material.Color hitPrimitiveColor
            let angle = hitPrimitiveNormal * lightDirection
            match angle > 0.0 with
            | false -> black
            | _ ->
                let coefficient = angle * p.Material.Diffuse
                appliedColor * coefficient


    // Ray mapping parameters:
    // p: Primitive hit
    // i: Vector to intersection
    // r: Ray being traced

    let maxDepth = 5

    // Reflection
    let reflectionRayMapping p i r =
        let getReflectedColor (c:Color) p =
            c * p.Material.Reflection

        match p.Material.Reflection > 0.0 with
        | false -> None
        | true -> Some ((reflect p r.Direction i), getReflectedColor)

    // Refraction
    let refractionRayMapping p i r rayRefraction collisionResult =
        let getRefractedColor (c:Color) p =
            c

        match p.Material.RefractionIndex > 0.0 && p.Material.Transparency > 0.0 with
        | false -> None
        | _ ->
            let primitiveRefraction = p.Material.RefractionIndex
            let n = rayRefraction / primitiveRefraction;
            let nn = p.GetNormal(i) * (float collisionResult)
            let cosI = -(nn * r.Direction)
            let cosT2 = 1.0 - n * n * (1.0 - cosI * cosI)
            match cosT2 > 0.0 with
            | false -> None
            | true ->
                let t = (n * r.Direction) + (n * cosI - (sqrt cosT2)) * nn
                Some (ray (i + t * Vector.Epsilon) t)


    // Color mapping
    // Intersection -> Color
    // There seems to be a pattern we might be able to extract. Each mapping relies on a property on the materiel which
    // affects the control flow and how the color is applied. Is this general enough that we can extract it. It also
    // happens to apply for the reflection mapping (the color function)

    // Ray mapping (maps ray to a new ray as well as a function for applying the resulting color)
    // A mapped ray will be traced as normal, and the color will be transformed with the function before being added
    // Intersection -> Ray
    // This should be thought of as a function that for example generates a Refraction Ray or a Reflection Ray


    // I can create an operator (or several) for composing mappings:
    // let raytracer = directMapping >|> refractionRayMapping >|> reflectionRayMapping >|# phongColorMapping >|# lambertColorMapping
    
    // This seems a bit more correct somehow:
    // let mappings = directRayMapping >|> [ refractionRayMapping; reflectionRayMapping ] >|# [ phongColorMapping, lambertColorMapping ]