namespace FrayTracer

open System
open System.Globalization

[<AutoOpen>]
module DomainTypes =

    type Vector = {
        X: double
        Y: double
        Z: double
    } with
        static member Create x y z =
            { X = x; Y = y; Z = z }

        static member (*) (v, a) =
            Vector.Create (v.X * a) (v.Y * a) (v.Z * a)

        static member (*) (a, v) =
            Vector.Create (v.X * a) (v.Y * a) (v.Z * a)

        static member (*) (v1, v2) =
            v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z

        static member (+) (v1, v2) =
            Vector.Create (v1.X + v2.X) (v1.Y + v2.Y) (v1.Z + v2.Z)

        static member (-) (v1, v2) =
            Vector.Create (v1.X - v2.X) (v1.Y - v2.Y) (v1.Z - v2.Z)

        static member (~-) v =
            Vector.Create -v.X -v.Y -v.Z

        static member Epsilon =
            0.0001

        member this.Item i =
            match i with
            | 0 -> this.X
            | 1 -> this.Y
            | 2 -> this.Z
            | _ -> failwith ("Invalid vector component index: " + i.ToString())

        override this.ToString() =
            String.Format(new CultureInfo("en-US"), "{{ {0:0.000}; {1:0.000}; {2:0.000} }}", this.X, this.Y, this.Z)

    type Color = {
        R: float
        G: float
        B: float
    } with
        static member Create r g b =
            { R = r; G = g; B = b }

        static member (+) (c1, c2) =
            Color.Create (c1.R + c2.R) (c1.G + c2.G) (c1.B + c2.B)

        static member (-) (c1, c2) =
            Color.Create (c1.R - c2.R) (c1.G - c2.G) (c1.B - c2.B)

        static member (*) (c, f) =
            Color.Create (c.R * f) (c.G * f) (c.B * f)

        static member (*) (f, c) =
            Color.Create (c.R * f) (c.G * f) (c.B * f)

        override this.ToString() =
            String.Format(new CultureInfo("en-US"), "{{ {0:0.}; {1:0.}; {2:0.} }}", this.R, this.G, this.B)

    let black = { R= 0.0; G=0.0; B=0.0 }
    let white = { R= 255.0; G=255.0; B=255.0 }
    let red = { R= 255.0; G=0.0; B=0.0 }
    let green = { R= 0.0; G=255.0; B=0.0 }
    let blue = { R= 0.0; G=0.0; B=255.0 }

    type Ray = {
        Origin: Vector
        Direction: Vector
    } with
        override this.ToString() =
            String.Format("{0} -> {1}", this.Origin, this.Direction)

    let ray origin direction =
        { Origin = origin; Direction = direction }

    type BoundingBox = {
        Position: Vector
        Size: Vector
    } with
        override this.ToString() =
            String.Format("[{0} -> {1}]", this.Position, this.Size)

    type RayCollision = 
        | Hit of float
        | HitFromInsidePrimitive of float
        | Miss

    type Camera = {
        Position: Vector
        Direction: Vector
    }

    type Material = {
        Color: Color
        Specular: float
        Diffuse: float
        Reflection : float
        RefractionIndex: float
        Transparency: float
    }

    let material = { Color = black; Specular = 0.0; Diffuse = 0.0; Reflection = 0.0; RefractionIndex = 0.0; Transparency = 0.0 }

    type Primitive = {
        Position: Vector
        BoundingBox: BoundingBox
        Intersect: Ray -> float -> RayCollision
        GetNormal: Vector -> Vector
        GetColor: Vector -> Color
        Intersects: BoundingBox -> bool
        Material: Material
    }

    type ScreenSize = {
        Width: int
        Height: int
    }

    [<Measure>] type ms

    type Frame = {
        Pixels: Color[,]
        RenderTime: int64<ms>
    }

    type Scene = {
        Lights : Primitive list
        Items : Primitive list
    }