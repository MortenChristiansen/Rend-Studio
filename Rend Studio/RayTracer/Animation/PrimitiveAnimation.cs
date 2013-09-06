using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Animation
{
    public class PrimitiveAnimation : IAnimation
    {
        private Action<Primitive, float> _animation;
        private Primitive _primitive;

        public PrimitiveAnimation(Primitive primitive, Action<Primitive, float> animation)
        {
            _animation = animation;
            _primitive = primitive;
        }

        #region IAnimation Members

        public void Animate(float time)
        {
            _animation(_primitive, time);
        }

        #endregion
    }
}
