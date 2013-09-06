namespace Vector

open FrayTracer
open Xunit
open FsUnit.Xunit
open Mathematics
open FsUnitFix
open System

[<Trait("Mathematics","Vector")>]
type ``: ``() =

    [<Fact>]
    let ``When creating a vector, it has the correct properties`` () =
        let result = vector 1.0 2.0 3.0
        result |> should equal { X=1.0; Y=2.0; Z=3.0 }

    [<Fact>]
    let ``The string representation of a vector is properly formatted`` () =
        let result = (vector 1.0 2.0 3.0).ToString()
        result |> should equal "{ 1.000; 2.000; 3.000 }"

    [<Fact>]
    let ``Indexing into the values 0, 1 and 2 yields the three components`` () =
        let result = vector 1.0 2.0 3.0
        result.[0] |> should equal 1.0
        result.[1] |> should equal 2.0
        result.[2] |> should equal 3.0

    [<Fact>]
    let ``When adding two vectors, a vector is created from the sum of each component pair`` () =
        let v1 = vector 1.0 2.0 3.0
        let v2 = vector 4.0 5.0 6.0
        let result = v1 + v2
        result |> should equal { X=5.0; Y=7.0; Z=9.0 }

    [<Fact>]
    let ``When subtracting two vectors, a vector is created from the subtraction of each component pair`` () =
        let v1 = vector 4.0 5.0 6.0
        let v2 = vector 1.0 3.0 5.0
        let result = v1 - v2
        result |> should equal { X=3.0; Y=2.0; Z=1.0 }

    [<Fact>]
    let ``When inverting a vector, each component is inverted`` () =
        let v = vector 1.0 2.0 3.0
        let result = -v
        result |> should equal { X=(-1.0); Y=(-2.0); Z=(-3.0) }

    [<Fact>]
    let ``When taking the dot product of two vectors, the result is correct`` () =
        let v1 = vector 0. 2.0 3.0
        let v2 = vector 1.0 4.0 2.0
        let result = v1 * v2
        result |> should equal 14.0

    [<Fact>]
    let ``When multiplying a vector with a scalar, each component is multiplied by the value`` () =
        let v = vector 1.0 4.0 2.0
        let result = v * 2.0
        result |> should equal { X=2.0; Y=8.0; Z=4.0 }

    [<Fact>]
    let ``When multiplying a scalar with a vector, each component is multiplied by the value`` () =
        let v = vector 1.0 4.0 2.0
        let result = 2.0 * v
        result |> should equal { X=2.0; Y=8.0; Z=4.0 }

    [<Fact>]
    let ``When multiplying a scalar with a vector, each component is multiplied by the value`` () =
        let v = vector 1.0 4.0 2.0
        let result = 2.0 * v
        result |> should equal { X=2.0; Y=8.0; Z=4.0 }

    [<Fact>]
    let ``The squared length of a vector is correctly calculated`` () =
        let v = vector 1.0 4.0 2.0
        let result = lengthSquared v
        result |> should equal 21.0

    [<Fact>]
    let ``The length of a vector is correctly calculated`` () =
        let v = vector 2.0 3.0 4.0
        let result = length v
        result |> should (equalWithin 0.001) 5.385

    [<Fact>]
    let ``A normalized vector has length 1`` () =
        let v = vector 2.0 3.0 4.0
        let result = normalize v
        length result |> should equal 1.0

    [<Fact>]
    let ``Making a linear interpolation of two vectors by a certain scalar gives the correct result`` () =
        let v1 = vector 0. 2.0 3.0
        let v2 = vector 1.0 4.0 2.0
        let result = lerp v1 v2 0.5
        result |> should equal { X=0.5; Y=3.0; Z=2.5 }

    [<Fact>]
    let ``Projecting one vector unto another vector gives the correct result`` () =
        let v1 = vector 0. 2.0 3.0
        let v2 = vector 1.0 4.0 2.0
        let result = project v1 v2
        result |> should (equalWithin 0.001) { X=0.666; Y=2.666; Z=1.333 }

    [<Fact>]
    let ``The cross product of two vectors gives the correct result`` () =
        let v1 = vector 0. 2.0 3.0
        let v2 = vector 1.0 4.0 2.0
        let result = cross v1 v2
        result |> should (equalWithin 0.001) { X=(-8.0); Y=3.0; Z=(-2.0) }

    [<Fact>]
    let ``Calculating the normal vector from two vectors produce the correct normalized result`` () =
        let v1 = vector 0. 2.0 3.0
        let v2 = vector 1.0 4.0 2.0
        let result = normal v1 v2
        result |> should (equalWithin 0.001) { X=(-0.912); Y=0.342; Z=(-0.228) }

    [<Fact>]
    let ``Two parallel vectors are correctly identified as such`` () =
        let v1 = vector 0.0 2.0 3.0
        let v2 = vector 0.0 4.0 6.0
        let result = areParallel v1 v2
        result |> should be True

    [<Fact>]
    let ``Two non-parallel vectors are correctly identified as such`` () =
        let v1 = vector 0.0 2.0 3.0
        let v2 = vector 0.0 8.0 6.0
        let result = areParallel v1 v2
        result |> should be False

    [<Fact>]
    let ``The angle between two perpendicular vectors is calculated to π/2`` () =
        let v1 = vector 0.0 2.0 2.0
        let v2 = vector 1.0 0.0 0.0
        let result = angle v1 v2
        result |> should (equalWithin 0.001) (Math.PI/2.0)

    [<Fact>]
    let ``The angle between two parallel vectors is calculated to 0`` () =
        let v1 = vector 0.0 2.0 3.0
        let v2 = vector 0.0 4.0 6.0
        let result = angle v1 v2
        result |> should equal 0.0