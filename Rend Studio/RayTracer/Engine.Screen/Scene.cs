using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry.Lighting;
using RayTracer.Engine.Screen;
using RayTracer.Plugins.SpatialStructures;
using RayTracer.Animation;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Engine
{
    /// <summary>
    /// Describes the composition of geometric objects which
    /// make up the scene to be rendered.
    /// </summary>
    public class Scene
    {
        public Storyboard Storyboard { get; set; }
        public Camera Camera { get; set; }
        public List<Plane> Planes { get; private set; }
        public List<Primitive> Primitives { get; private set; }
        public List<PositionedLight> Lights { get; private set; }
        public List<AmbientLight> AmbientLights { get; private set; }
        /// <summary>
        /// Gets or sets the scale of the world, relative to the coordinate
        /// points used. The scale indicates how many times larger the actual
        /// world is. This is used for such effects as light falloff speed.
        /// </summary>
        public float WorldScale { get; set; }

        internal BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(new Vector(_minX, _minY, _minZ), new Vector(_maxX - _minX, _maxY - _minY, _maxZ - _minZ));
            }
        }

        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;
        private float _minZ;
        private float _maxZ;

        /// <summary>
        /// Creates a new scene with a custom camera.
        /// </summary>
        /// <param name="camera">The camera used to capture the scene</param>
        public Scene(Camera camera)
        {
            Camera = camera;

            Planes = new List<Plane>();
            Primitives = new List<Primitive>();
            Lights = new List<PositionedLight>();
            AmbientLights = new List<AmbientLight>();
            ClearScene();
            WorldScale = 1;
        }

        /// <summary>
        /// Creates a new scene with a default camera.
        /// </summary>
        public Scene()
            : this(new Camera())
        {
            
        }

        public void AddPrimitives(params Primitive[] primitives)
        {
            foreach (var primitive in primitives)
            {
                AddPrimitive(primitive);
            }
        }

        public void AddPrimitive(Primitive primitive)
        {
            if (primitive is Plane)
            {
                Planes.Add(primitive as Plane);
                return;
            }

            Primitives.Add(primitive);
            if (primitive is PositionedLight)
            {
                Lights.Add((PositionedLight)primitive);
            }

            BoundingBox bb = primitive.GetBoundingBox();
            if (Primitives.Count == 1)
            {
                _minX = bb.Position.X;
                _maxX = bb.Position.X + bb.Size.X;
                _minY = bb.Position.Y;
                _maxY = bb.Position.Y + bb.Size.Y;
                _minZ = bb.Position.Z;
                _maxZ = bb.Position.Z + bb.Size.Z;
            }
            else
            {
                _minX = bb.Position.X < _minX ? bb.Position.X : _minX;
                _maxX = bb.Position.X + bb.Size.X > _maxX ? bb.Position.X + bb.Size.X : _maxX;
                _minY = bb.Position.Y < _minY ? bb.Position.Y : _minY;
                _maxY = bb.Position.Y + bb.Size.Y > _maxY ? bb.Position.Y + bb.Size.Y : _maxY;
                _minZ = bb.Position.Z < _minZ ? bb.Position.Z : _minZ;
                _maxZ = bb.Position.Z + bb.Size.Z > _maxZ ? bb.Position.Z + bb.Size.Z : _maxZ;
            }
        }

        public void ClearScene()
        {
            Primitives.Clear();
            Lights.Clear();
            AmbientLights.Clear();
            _minX = 0;
            _maxX = 0;
            _minY = 0;
            _maxY = 0;
            _minZ = 0;
            _maxZ = 0;
        }
    }
}
