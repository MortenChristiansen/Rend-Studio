namespace Primitive

open FrayTracer
open Xunit
open FsUnit.Xunit
open Mathematics
open Geometry
open FsUnitFix
open System

[<Trait("Geometry","Primitive")>]
type ``: ``() =

    let blackMaterial = { material with Color = black }
    let redMaterial = { material with Color = red }

    [<Fact>]
    let ``When creating a primitive, it has the correct properties`` () =
        let boundingBox = boundingbox (vector 1.0 2.0 3.0) (vector 1.0 2.0 3.0)
        let getNormal (intersection:Vector) = vector 1.0 2.0 3.0
        let intersects boundingBox = true
        let position = vector 1.0 2.0 3.0
        let intersect ray d = 0.0 |> Hit
        let getColor intersection = black
        let material = { Diffuse = 1.0; Specular = 2.0; Color = red; Transparency = 3.0; RefractionIndex = 4.0; Reflection = 5.0 }
        let result = primitive position boundingBox getNormal intersects intersect getColor material
        result.Position |> should equal { X=1.0; Y=2.0; Z=3.0 }
        result.Material |> should equal material

    [<Fact>]
    let ``When reflecting a ray off a primitive, the resulting ray has the correct properties`` () =
        let originalDirection = vector 1.0 4.0 9.0
        let intersection = vector 4.0 5.0 6.0
        let getNormal intersection = intersection + (vector 4.0 1.0 2.0)
        let p = primitive (vector 1.0 2.0 3.0) (boundingbox (vector 1.0 4.0 6.0) (vector 8.0 4.0 2.0)) getNormal (fun bb -> true) (fun x y -> 0.0 |> Hit) (fun x -> black) blackMaterial
        let result = reflect p originalDirection intersection
        result |> should (equalWithin 0.001) { Origin = { X=4.0; Y=5.0; Z=6.0 }; Direction = { X=(-0.626); Y=(-0.468); Z=(-0.623) } }

    // ========== Triangle Primitive ========== //

    [<Fact>]
    let ``When creating a triangle, the first vertex is used for the position of the primitive`` () =
        let a = vector 1.0 2.0 3.0
        let b = vector 4.0 5.0 6.0
        let c = vector 7.0 8.0 9.0
        let result = triangle a b c blackMaterial
        result.Position |> should equal (vector 1.0 2.0 3.0) 

    [<Fact>]
    let ``When creating a 2-dimensional triangle, the bounding box is adjusted`` () =
        let a = vector 0.0 2.0 3.0
        let b = vector 0.0 5.0 6.0
        let c = vector 0.0 8.0 9.0
        let result = (triangle a b c blackMaterial).BoundingBox
        result.Position.X |> should equal -0.01
        result.Size.X |> should equal 0.01

    [<Fact>]
    let ``When creating a triangle, it has the correct bounding box`` () =
        let a = vector 1.0 2.0 3.0
        let b = vector 4.0 5.0 6.0
        let c = vector 7.0 8.0 9.0
        let result = triangle a b c blackMaterial
        result.BoundingBox |> should equal (boundingbox (vector 1.0 2.0 3.0) (vector 7.0 8.0 9.0))

    [<Fact>]
    let ``The normal for a triangle is correctly calculated`` () =
        let triangle = triangle (vector 1.0 2.0 3.0) (vector 4.0 2.0 3.0) (vector 4.0 2.0 9.0) blackMaterial
        let result = triangle.GetNormal (vector 0.0 0.0 0.0)
        result |> should equal (vector 0.0 -1.0 0.0)

    [<Fact>]
    let ``A triangle is correctly identified as intersecting a bounding box`` () =
        let triangle = triangle (vector 1.5 0.5 0.5) (vector 4.0 2.0 3.0) (vector 4.0 2.0 9.0) blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = triangle.Intersects box
        result |> should be True

    [<Fact>]
    let ``A triangle is correctly identified as not intersecting a bounding box`` () =
        let triangle = triangle (vector 0.5 0.5 0.5) (vector -4.0 2.0 3.0) (vector -4.0 2.0 9.0) blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = triangle.Intersects box
        result |> should be False

    [<Fact>]
    let ``When a ray intersects a triangle, the result is a Hit with the value of the distance to the triangle`` () =
        let r = ray (vector 0.0 0.5 0.5) (vector 1.0 0.0 0.0)
        let triangle = triangle (vector 2.0 0.0 0.0) (vector 2.0 2.0 2.0) (vector 2.0 2.0 0.0) blackMaterial
        let result = triangle.Intersect r Double.MaxValue
        result |> should equal (2.0 |> Hit)

    [<Fact>]
    let ``When a ray intersects a triangle from the inside, the result is a HitFromInsidePrimitive with the value of the distance to the triangle`` () =
        let r = ray (vector 4.0 0.5 0.5) (vector -1.0 0.0 0.0)
        let triangle = triangle (vector 2.0 0.0 0.0) (vector 2.0 2.0 2.0) (vector 2.0 2.0 0.0) blackMaterial
        let result = triangle.Intersect r Double.MaxValue
        result |> should equal (2.0 |> HitFromInsidePrimitive)

    [<Fact>]
    let ``When a ray does not intersect a triangle, the result is a Miss`` () =
        let r = ray (vector 4.0 0.5 0.5) (vector 1.0 0.0 0.0)
        let triangle = triangle (vector 2.0 0.0 0.0) (vector 2.0 2.0 2.0) (vector 2.0 2.0 0.0) blackMaterial
        let result = triangle.Intersect r Double.MaxValue
        result |> should equal Miss

    [<Fact>]
    let ``The color of a triangle at any point equals the color it is created with`` () =
        let triangle = triangle (vector 2.0 0.0 0.0) (vector 2.0 2.0 2.0) (vector 2.0 2.0 0.0) redMaterial
        let result = triangle.GetColor (vector 0.0 0.0 0.0)
        result |> should equal red

    [<Fact>]
    let ``When a ray does not intersect a triangle by the given distance, the result is a Miss`` () =
        let r = ray (vector 0.0 0.5 0.5) (vector 1.0 0.0 0.0)
        let triangle = triangle (vector 2.0 0.0 0.0) (vector 2.0 2.0 2.0) (vector 2.0 2.0 0.0) blackMaterial
        let result = triangle.Intersect r 1.5
        result |> should equal Miss

    // ========== Sphere Primitive ========== //

    [<Fact>]
    let ``When creating a sphere, the vector argument is used for the position of the primitive`` () =
        let result = sphere (vector 1.0 2.0 3.0) 4.0 blackMaterial
        result.Position |> should equal (vector 1.0 2.0 3.0)

    [<Fact>]
    let ``When creating a sphere, it has the correct bounding box`` () =
        let result = sphere (vector 1.0 2.0 3.0) 1.0 blackMaterial
        result.BoundingBox |> should equal (boundingbox (vector 0.0 1.0 2.0) (vector 2.0 3.0 4.0))

    [<Fact>]
    let ``The normal for a sphere is correctly calculated`` () =
        let sphere = sphere (vector 1.0 2.0 3.0) 4.0 blackMaterial
        let result1 = sphere.GetNormal (vector 1.0 6.0 3.0)
        let result2 = sphere.GetNormal (vector 1.0 2.0 7.0)
        result1 |> should equal (vector 0.0 1.0 0.0)
        result2 |> should equal (vector 0.0 0.0 1.0)

    [<Fact>]
    let ``A sphere is correctly identified as intersecting a bounding box`` () =
        let sphere = sphere (vector 1.0 2.0 3.0) 4.0 blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = sphere.Intersects box
        result |> should be True

    [<Fact>]
    let ``A sphere is correctly identified as not intersecting a bounding box`` () =
        let sphere = sphere (vector 3.5 2.0 3.0) 1.0 blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = sphere.Intersects box
        result |> should be False

    [<Fact>]
    let ``The color of a sphere at any point equals the color it is created with`` () =
        let p = sphere (vector 1.0 2.0 3.0) 1.0 redMaterial
        let result = p.GetColor (vector 0.0 0.0 0.0)
        result |> should equal red

    [<Fact>]
    let ``When a ray intersects a sphere, the result is a Hit with the value of the distance to the sphere`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = sphere (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (0.5 |> Hit)

    [<Fact>]
    let ``When a ray intersects a sphere from the inside, the result is a HitFromInsidePrimitive with the value of the distance to the sphere`` () =
        let r = ray (vector 1.0 1.5 3.0) (vector 0.0 1.0 0.0)
        let p = sphere (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (1.5 |> HitFromInsidePrimitive)

    [<Fact>]
    let ``When a ray does not intersect a sphere, the result is a Miss``() =
        let test r p =
            let result = p.Intersect r Double.MaxValue
            result |> should equal Miss

        test (ray (vector 1.0 0.5 3.0) (vector 0.0 -1.0 0.0)) (sphere (vector 1.0 2.0 3.0) 1.0 blackMaterial)
        test (ray (vector 10.0 5.5 10.0) (vector 0.0 0.0 -1.0)) (sphere (vector 10.0 0.0 0.0) 5.0 blackMaterial)
        test (ray (vector 5.0 5.0 5.0) (vector -1.0 -1.0 -1.0)) (sphere (vector 10.0 10.0 10.0) 5.0 blackMaterial)

    [<Fact>]
    let ``When a ray does not intersect a sphere by the given distance, the result is a Miss`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = sphere (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r 0.25
        result |> should equal Miss

    // ========== Plane Primitive ========== //

    [<Fact>]
    let ``When creating a plane, the vector argument is used for the position of the primitive`` () =
        let result = plane (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0) blackMaterial
        result.Position |> should equal (vector 1.0 2.0 3.0) 

    [<Fact>]
    let ``The normal of a plane is the same as passed to the function`` () =
        let p = plane (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0) blackMaterial
        let result = p.GetNormal(vector 0.0 0.0 0.0)
        result |> should equal (vector 4.0 5.0 6.0) 

    [<Fact>]
    let ``Planes have an empty bounding box`` () =
        let p = plane (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0) blackMaterial
        let result = p.BoundingBox
        result |> should equal { Position = (vector 0.0 0.0 0.0); Size = (vector 0.0 0.0 0.0) }

    [<Fact>]
    let ``The color of a plane at any point equals the color it is created with`` () =
        let p = plane (vector 1.0 2.0 3.0) (vector 4.0 5.0 6.0) redMaterial
        let result = p.GetColor (vector 0.0 0.0 0.0)
        result |> should equal red

    [<Fact>]
    let ``When a ray intersects a plane, the result is a Hit with the value of the distance to the plane`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = plane (vector 1.0 2.0 3.0) (vector 0.0 1.0 0.0) blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (1.5 |> Hit)

    [<Fact>]
    let ``When a ray intersects a plane from the inside, the result is a HitFromInsidePrimitive with the value of the distance to the plane`` () =
        let r = ray (vector 1.0 1.5 3.0) (vector 0.0 1.0 0.0)
        let p = plane (vector 1.0 2.0 3.0) (vector 0.0 1.0 0.0) blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (0.5 |> HitFromInsidePrimitive)

    [<Fact>]
    let ``When a ray does not intersect a plane, the result is a Miss`` () =
        let r = ray (vector 1.0 1.5 3.0) (vector 0.0 -1.0 0.0)
        let p = plane (vector 1.0 2.0 3.0) (vector 0.0 1.0 0.0) blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal Miss

    [<Fact>]
    let ``When a ray does not intersect a plane by the given distance, the result is a Miss`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = plane (vector 1.0 2.0 3.0) (vector 0.0 1.0 0.0) blackMaterial
        let result = p.Intersect r 1.0
        result |> should equal Miss

    // ========== Light Primitive ========== //

    [<Fact>]
    let ``When creating a light, the vector argument is used for the position of the primitive`` () =
        let result = light (vector 1.0 2.0 3.0) 0.0 blackMaterial
        result.Position |> should equal (vector 1.0 2.0 3.0)

    [<Fact>]
    let ``The color of a light at any point equals the color it is created with`` () =
        let l = light (vector 1.0 2.0 3.0) 0.0 redMaterial
        let result = l.GetColor (vector 0.0 0.0 0.0)
        result |> should equal red

    [<Fact>]
    let ``When creating a light, it has the correct bounding box`` () =
        let result = light (vector 1.0 2.0 3.0) 1.0 blackMaterial
        result.BoundingBox |> should equal (boundingbox (vector 0.0 1.0 2.0) (vector 2.0 3.0 4.0))

    [<Fact>]
    let ``The normal for a light is correctly calculated`` () =
        let light = light (vector 1.0 2.0 3.0) 4.0 blackMaterial
        let result1 = light.GetNormal (vector 1.0 6.0 3.0)
        let result2 = light.GetNormal (vector 1.0 2.0 7.0)
        result1 |> should equal (vector 0.0 1.0 0.0)
        result2 |> should equal (vector 0.0 0.0 1.0)

    [<Fact>]
    let ``A light is correctly identified as intersecting a bounding box`` () =
        let light = light (vector 1.0 2.0 3.0) 4.0 blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = light.Intersects box
        result |> should be True

    [<Fact>]
    let ``A light is correctly identified as not intersecting a bounding box`` () =
        let light = light (vector 3.5 2.0 3.0) 1.0 blackMaterial
        let box = boundingbox (vector 1.0 0.0 0.0) (vector 1.0 1.0 1.0)
        let result = light.Intersects box
        result |> should be False

    [<Fact>]
    let ``When a ray intersects a light, the result is a Hit with the value of the distance to the light`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = light (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (0.5 |> Hit)

    [<Fact>]
    let ``When a ray intersects a light from the inside, the result is a HitFromInsidePrimitive with the value of the distance to the light`` () =
        let r = ray (vector 1.0 1.5 3.0) (vector 0.0 1.0 0.0)
        let p = light (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r Double.MaxValue
        result |> should equal (1.5 |> HitFromInsidePrimitive)

    [<Fact>]
    let ``When a ray does not intersect a light, the result is a Miss``() =
        let test r p =
            let result = p.Intersect r Double.MaxValue
            result |> should equal Miss

        test (ray (vector 1.0 0.5 3.0) (vector 0.0 -1.0 0.0)) (light (vector 1.0 2.0 3.0) 1.0 blackMaterial)
        test (ray (vector 10.0 5.5 10.0) (vector 0.0 0.0 -1.0)) (light (vector 10.0 0.0 0.0) 5.0 blackMaterial)
        test (ray (vector 5.0 5.0 5.0) (vector -1.0 -1.0 -1.0)) (light (vector 10.0 10.0 10.0) 5.0 blackMaterial)

    [<Fact>]
    let ``When a ray does not intersect a light by the given distance, the result is a Miss`` () =
        let r = ray (vector 1.0 3.5 3.0) (vector 0.0 -1.0 0.0)
        let p = light (vector 1.0 2.0 3.0) 1.0 blackMaterial
        let result = p.Intersect r 0.25
        result |> should equal Miss