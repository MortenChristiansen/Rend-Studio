using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Engine.Screen;
using RayTracer.Engine;
using RayTracer.Geometry;

namespace RayTracer.Animation
{
    public class CameraAnimation : IAnimation
    {
        private Action<Camera, float> _animation;
        public CameraAnimation(Action<Camera, float> animation)
        {
            _animation = animation;
        }

        private Vector _oldPosition;
        private IPath _cameraPath;
        public CameraAnimation(IPath cameraPath)
        {
            _cameraPath = cameraPath;
        }

        public void Animate(float time)
        {
            if (_animation != null)
            {
                _animation(RenderEngine.Scene.Camera, time);
            }
            else if (_cameraPath != null)
            {
                _oldPosition = RenderEngine.Scene.Camera.Position;
                RenderEngine.Scene.Camera.Position = _cameraPath.GetValue(time / RenderEngine.Scene.Storyboard.Duration);
                RenderEngine.Scene.Camera.Direction = (RenderEngine.Scene.Camera.Position - _oldPosition).Normal;
            }
        }
    }
}
