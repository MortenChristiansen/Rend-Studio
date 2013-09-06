namespace PostProcessing

open FrayTracer
open Visual

[<AutoOpen>]
module PixelFilters =

    let greyScale (frame:Frame) =
        let greyScalePixel x y =
            let original = frame.Pixels.[x,y]
            let c = original.R * 0.299 + original.G * 0.587 + original.B * 0.114
            rgb c c c

        { Pixels = Array2D.init (frame.Pixels.GetLength(0)) (frame.Pixels.GetLength(1)) greyScalePixel; RenderTime = frame.RenderTime }