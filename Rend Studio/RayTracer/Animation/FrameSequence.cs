using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Engine;
using System.Collections;
using System.IO;
using RayTracer.Engine.Screen;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using Splicer.Timeline;
using Splicer.Renderer;
namespace RayTracer.Animation
{
    public class FrameSequence : IEnumerable<Frame>
    {
        private Storyboard _storyboard;
        private float _currentTime;

        public FrameSequence(Storyboard storyboard)
        {
            _storyboard = storyboard;
        }

        public void MakeVideo(string file)
        {
            _currentTime = 0.001f;

            using (ITimeline timeline = new DefaultTimeline(_storyboard.FrameRate))
            {
                timeline.AddAudioGroup();
                Camera camera = RenderEngine.Scene.Camera;
                IGroup group = timeline.AddVideoGroup(24, camera.Width, camera.Height);
                ITrack track = group.AddTrack();

                RenderEngine.Out.WriteLine("Making video...");
                RenderEngine.Out.WriteLine("Rendering frames...");

                Stopwatch watch = Stopwatch.StartNew();

                float frameLength = 1f / _storyboard.FrameRate;
                int frameNo = 1;

                foreach (var frame in this)
                {
                    track.AddImage(frame.Image, 0, frameLength);
                    RenderEngine.Out.WriteLine(string.Format("Rendered frame #{0} in {1:0.0} seconds - post processing took {2:0.0} seconds", frameNo, frame.RenderTime.TotalSeconds, frame.PostProcessingTime.TotalSeconds));
                    frameNo++;
                }

                RenderEngine.Out.WriteLine("Outputting video file...");

                using (var renderer = new AviFileRenderer(timeline, file))
                {
                    try
                    {
                        renderer.Render();
                    }
                    catch (COMException e)
                    {
                        RenderEngine.Out.WriteLine("Error saving file: " + e.Message);
                        watch.Stop();
                        return;
                    }
                }
                RenderEngine.Out.WriteLine(string.Format("Video completed in {0:0.0} seconds.", watch.Elapsed.TotalSeconds));
                watch.Stop();
            }
        }

        #region IEnumerable<Frame> Members

        public IEnumerator<Frame> GetEnumerator()
        {
            while (_currentTime < _storyboard.Duration || _storyboard.Duration == 0)
            {
                foreach (var animation in _storyboard.Animations)
                {
                    animation.Animate(_currentTime);
                }
                //RenderEngine.PopulateSpatialStructure(); //Delete
                Frame frame = RenderEngine.RenderScene();
                _currentTime += 1f / _storyboard.FrameRate;
                yield return frame;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var frame in this)
            {
                yield return frame;
            }
        }

        #endregion
    }
}
