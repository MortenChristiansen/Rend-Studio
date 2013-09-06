namespace Mathematics

open FrayTracer

[<AutoOpen>]
module Vector =

    let vector x y z = Vector.Create x y z

    let lengthSquared (v:Vector) =
        v * v

    let length v =
        v |> lengthSquared |> sqrt

    let normalize v = 
        v * (1.0 / (length v))

    let lerp (v1:Vector) (v2:Vector) (a:double) =
        v1 + (v2 - v1) * a

    let project v1 v2 =
        (v1 * v2) / (lengthSquared v2) * v2

    let cross v1 v2 =
        vector (v1.Y * v2.Z - v1.Z * v2.Y) (v1.Z * v2.X - v1.X * v2.Z) (v1.X * v2.Y - v1.Y * v2.X)

    let normal v1 v2 =
        (v1, v2) ||> cross |> normalize

    let areParallel v1 v2 =
        cross v1 v2 |> length = 0.0

    let angle v1 v2 =
        (normalize v1) * (normalize v2) |> acos