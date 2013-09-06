namespace Visual

open FrayTracer

[<AutoOpen>]
module Color =
    
    let rgb r g b =
        { R = r; G = g; B = b }

    let filter color f =
        rgb (f.R * (color.R / 255.0)) (f.G * (color.G / 255.0)) (f.B * (color.B / 255.0))