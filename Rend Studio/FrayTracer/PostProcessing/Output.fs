namespace PostProcessing

open FrayTracer
open System
open System.Drawing
open System.Drawing.Imaging

[<AutoOpen>]
module Output =
    let addCaption (bm:Bitmap) (caption: string) =
        let g = Graphics.FromImage(bm)
        let p = new PointF((float32 bm.Width) - (float32 caption.Length) * 6.15f - 3.0f, 3.0f)
        g.DrawString(caption, new Font("Consolas", 8.0f), Brushes.White, p)

    let addRenderTimeCaption (bm:Bitmap) (renderTime:int64<ms>) =
        let caption = String.Format("Rendered in {0:n0}ms", renderTime)
        addCaption bm caption

    let createBitmap frame =
        let bm = new Bitmap(frame.Pixels.GetLength(0), frame.Pixels.GetLength(1))
        let convertColor color = System.Drawing.Color.FromArgb(int color.R, int color.G, int color.B)
        let setPixel x y color = 
            bm.SetPixel(x, y, convertColor color)

        Array2D.iteri setPixel frame.Pixels
        addRenderTimeCaption bm frame.RenderTime
        bm

    let saveToPng (path:string) frame =
        let bm = createBitmap frame
        bm.Save(path, ImageFormat.Png)
