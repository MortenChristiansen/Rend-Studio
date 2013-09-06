using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Animation
{
    public class Storyboard
    {
        public float FrameRate { get; set; }
        public float Duration { get; set; }
        public List<IAnimation> Animations { get; private set; }

        public Storyboard(float frameRate, float duration, IAnimation animation)
        {
            FrameRate = frameRate;
            Duration = duration;
            Animations = new List<IAnimation>();
            Animations.Add(animation);
        }

        public Storyboard(float frameRate, float duration)
        {
            FrameRate = frameRate;
            Duration = duration;
            Animations = new List<IAnimation>();
        }
    }
}
