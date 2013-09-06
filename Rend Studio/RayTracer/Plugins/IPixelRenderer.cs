using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Plugins
{
    public interface IPixelRenderer : IPlugin
    {
        void RenderPixels();
        IPixelRenderer GetPixelHandlerInstance(int threadId);
    }
}
