using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;

namespace RayTracer.Engine.Screen
{
    public class Camera
    {
        public Vector Position { get; set; }
        private Vector direction;
        public Vector Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value.Normal;
                XAxis = -Vector.NormalVector(direction, Vector.YUnit);
                YAxis = Vector.NormalVector(direction, XAxis);
            }
        }

        private int _height;
        private int _width;
        public int Height { get { return _height; } set { _height = value; RecalculateBaseZoom(); } }
        public int Width { get { return _width; } set { _width = value; RecalculateBaseZoom(); } }

        private float _baseZoom;
        public float Zoom { get; set; }

        internal Vector XAxis { get; private set; }
        internal Vector YAxis { get; private set; }

        public Camera(Vector position, Vector direction, float zoom)
        {
            Position = position;
            Direction = direction;
            Width = RenderEngine.Configuration.Width;
            Height = RenderEngine.Configuration.Height;
            Zoom = zoom;
            RecalculateBaseZoom();
        }

        public Camera()
            : this(new Vector(0, 0, -500), Vector.ZUnit, 1)
        {

        }

        /// <summary>
        /// Provides a direction vector for a given image pixel coordinate. The coordinates start
        /// at 0,0 in the top left corner of the image.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel</param>
        /// <param name="y">The y-coordinate of the pixel</param>
        /// <returns>The direction to the pixel</returns>
        public Vector GetFrameCoordinateDirection(float x, float y)
        {
            Vector direction = Direction * Zoom * _baseZoom; //Direction = center of frame
            direction += XAxis * (x - Width / 2); //Direction factors the X coordinate
            direction -= YAxis * (y - Height / 2); //Direction factors the Y coordinate
            return direction.Normal;
        }

        public void LookAt(Vector point)
        {
            Direction = point - Position;
        }

        private void RecalculateBaseZoom()
        {
            _baseZoom = (float)Math.Sqrt(Width * Width + Height * Height);
        }
    }
}
