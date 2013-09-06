using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Plugins.SpatialStructures;
using RayTracer.Geometry.Lighting;
using RayTracer.Plugins.Tracers;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Geometry.Coloring;
using RayTracer.Plugins.Shaders;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Engine
{
    class RenderUnit
    {
        private ISpatialStructure _spatialStructure;
        private List<PositionedLight> _lights;
        private List<AmbientLight> _ambientLights;
        private IShadowTracer _shadowTracer;
        private int _traceDepth;
        private List<Primitive> _primitives; //Consider changing to array (also in Scene)
        private List<ILightShader> _lightShaders;
        private List<IRecurseShader> _recurseShaders;
        private bool _initialized = false;

        public int Traces { get; private set; }
        public Func<Color, Color> ColorFilter { get; set; }

        public Color[] RenderRays(Ray[] rays)
        {
            Color[] colors = new Color[rays.Length];

            for (int i = 0; i < rays.Length; i++)
            {
                colors[i] = Color.Black;
                float distance;
                RayTrace(rays[i], ref colors[i], 0, 1, out distance);
                if (ColorFilter != null) colors[i] = ColorFilter(colors[i]);
            }

            return colors;
        }

        private void InitUnit()
        {
            _spatialStructure = RenderEngine.Configuration.SpatialStructure;
            _lights = RenderEngine.Scene.Lights;
            _ambientLights = RenderEngine.Scene.AmbientLights;
            _shadowTracer = RenderEngine.Configuration.ShadowTracer;
            _traceDepth = RenderEngine.Configuration.Depth;
            _primitives = RenderEngine.Scene.Primitives;
            _lightShaders = RenderEngine.Configuration.LightShaders;
            _recurseShaders = RenderEngine.Configuration.RecurseShaders;

            Traces = 0;
            _initialized = true;
        }

        public Primitive RayTrace(Ray ray, ref Color colorAcc, int depth, float refractionIndex, out float distance)
        {
            if (!_initialized) InitUnit();

            distance = float.MaxValue;
            if (depth > _traceDepth)
            {
                return null;
            }
            //Performance measurements
            Traces++;

            //Find nearest intersection
            RayCollision collisionResult = RayCollision.Miss;
            Primitive hitPrimitive = FindNearest(ref distance, ray, ref collisionResult);

            //If light has been hit, set color to color of the light and return
            if (hitPrimitive is PositionedLight)
            {
                //hitPrimitive.Material.Absorbance = 0.01f;
                //_recurseShaders[1].AdjustColor(ref colorAcc, ray.Origin + ray.Direction * distance, hitPrimitive, ray, depth, refractionIndex, collisionResult);
                colorAcc += hitPrimitive.Material.Color * (((PositionedLight)hitPrimitive).Brightness / (distance * distance));
                //return hitPrimitive;
            }
            //No hit
            if (hitPrimitive == null)
            {
                return null;
            }
            //Handle intersection
            Vector intersection = ray.Origin + ray.Direction * distance;

            //Apply shaders

            if (collisionResult != RayCollision.HitFromInsidePrimitive)
            {
                ApplyLightShaders(ray.Direction, ref colorAcc, depth, hitPrimitive, intersection);
            }

            ApplyRecurseShaders(ray, ref colorAcc, depth, refractionIndex, collisionResult, hitPrimitive, intersection);
            
            if (collisionResult != RayCollision.HitFromInsidePrimitive)
            {
                ApplyAmbientLight(ref colorAcc, hitPrimitive, intersection);
            }

            //if (hitPrimitive is PositionedLight && collisionResult == RayCollision.Hit)
            //{
            //    Vector lightDirection = (hitPrimitive.Position - intersection).Normal;
            //    Vector rayDirection = ray.Direction;
            //    float angle = Vector.Dot(lightDirection, rayDirection);
            //    if (angle <= 0) return hitPrimitive;
            //    //float coeff = angle * ((PositionedLight)hitPrimitive).Brightness / (ray.Origin - hitPrimitive.Position).LengthSquared;
            //    float coeff = angle * (float)Math.Log(((PositionedLight)hitPrimitive).Brightness, 2000);
            //    colorAcc += hitPrimitive.Material.Color * (float)Math.Pow(coeff, 1.5f);

            //    //float total = ((float)Math.Pow(angle, 4) * ((PositionedLight)hitPrimitive).Brightness) / (ray.Origin - hitPrimitive.Position).LengthSquared;
            //    //colorAcc += hitPrimitive.Material.Color * total;

            //    //float coefficient = angle * hitPrimitive.Material.Diffuse * lightPrimitive.Brightness * shade / lengthSquare;

            //    //Vector hitPrimitiveNormal = hitPrimitive.GetNormal(intersection);
            //    //Vector reflectedDirection = lightDirection - 2 * Vector.Dot(lightDirection, hitPrimitiveNormal) * hitPrimitiveNormal;
            //    //Vector viewRay = intersection - RenderEngine.Scene.Camera.Position;
            //    //float phongTerm = Math.Max(Vector.Dot(reflectedDirection.Normal, viewRay.Normal), 0);
            //    //float coefficient = hitPrimitive.Material.Specular * shade * lightPrimitive.Brightness / (lightPrimitive.Position - intersection).LengthSquared;
            //    //phongTerm = (float)Math.Pow(phongTerm, 20) * coefficient;
            //}

            return hitPrimitive;
        }

        public void ApplyAmbientLight(ref Color colorAcc, Primitive hitPrimitive, Vector intersection)
        {
            foreach (var ambientLight in _ambientLights)
            {
                colorAcc += (ambientLight.Color * ambientLight.Brightness).Filter(hitPrimitive.GetColor(intersection)) * hitPrimitive.Material.Diffuse;
            }
        }

        public void ApplyRecurseShaders(Ray ray, ref Color colorAcc, int depth, float refractionIndex, RayCollision collisionResult, Primitive hitPrimitive, Vector intersection)
        {
            foreach (var shader in _recurseShaders)
            {
                shader.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, refractionIndex, collisionResult);
            }
        }

        public void ApplyLightShaders(Vector direction, ref Color colorAcc, int depth, Primitive hitPrimitive, Vector intersection)
        {
            foreach (PositionedLight light in _lights)
            {
                float shade = _shadowTracer == null ? 1.0f : _shadowTracer.CalculateShade(intersection, light, depth);
                foreach (var shader in _lightShaders)
                {
                    shader.AdjustColor(ref colorAcc, hitPrimitive, light, intersection, direction, shade);
                }
            }
        }

        public Primitive FindNearest(ref float distance, Ray ray, ref RayCollision collisionResult)
        {
            if (!_initialized) InitUnit();

            Primitive hitPrimitive = null;

            if (_spatialStructure == null || RenderEngine.Scene.Primitives.Count < _spatialStructure.MinimumPrimitives)
            {
                //Default collision detection
                foreach (Primitive primitive in _primitives)
                {
                    RayCollision collision = primitive.Intersect(ray, ref distance);
                    if (collision != RayCollision.Miss)
                    {
                        hitPrimitive = primitive;
                        collisionResult = collision;
                    }
                }
            }
            else
            {
                RayCollision collision;
                Primitive prim = _spatialStructure.GetClosestIntersectionPrimitive(ray, ref distance, out collision);

                if (collision != RayCollision.Miss)
                {
                    hitPrimitive = prim;
                    collisionResult = collision;
                }
            }

            //Search the planes
            foreach (var plane in RenderEngine.Scene.Planes)
            {
                RayCollision collision = plane.Intersect(ray, ref distance);
                if (collision != RayCollision.Miss)
                {
                    hitPrimitive = plane;
                    collisionResult = collision;
                }
            }

            return hitPrimitive;
        }

        #region InSync

        public Color[] RenderRaysInSync(Ray[] rays)
        {
            var colors = new Color[rays.Length];
            var refractionIndices = new float[rays.Length];
            var distances = new float[rays.Length];

            for (int i = 0; i < rays.Length; i++)
            {
                colors[i] = Color.Black;
                refractionIndices[i] = 1;
            }

            RayTrace(rays, ref colors, 0, refractionIndices, out distances);

            if (ColorFilter != null)
            {
                for (int i = 0; i < rays.Length; i++)
                {
                    colors[i] = ColorFilter(colors[i]);
                }
            }

            return colors;
        }

        public Primitive[] RayTrace(Ray[] rays, ref Color[] colorAcc, int depth, float[] refractionIndices, out float[] distances)
        {
            if (!_initialized) InitUnit();
            distances = new float[rays.Length].Select(val => float.MaxValue).ToArray();
            if (depth > _traceDepth) return null;

            var hitPrimitives = new Primitive[rays.Length];
            var collisionResults = new RayCollision[rays.Length];
            var intersections = new Vector[rays.Length];

            //Performance measurements
            Traces += rays.Length;

            //Find nearest intersection
            hitPrimitives = FindNearestInSync(ref distances, rays, ref collisionResults);
            for (int i = 0; i < rays.Length; i++)
            {
                //If light has been hit, set color to color of the light and return
                if (hitPrimitives[i] is PositionedLight)
                {
                    colorAcc[i] += hitPrimitives[i].Material.Color * ((PositionedLight)hitPrimitives[i]).Brightness;
                }
            }

            for (int i = 0; i < rays.Length; i++)
            {
                //Handle intersection
                intersections[i] = rays[i].Origin + rays[i].Direction * distances[i];
            }

            //Apply shaders

            for (int i = 0; i < rays.Length; i++)
            {
                //No hit
                if (hitPrimitives[i] == null) continue;
                if (collisionResults[i] != RayCollision.HitFromInsidePrimitive)
                {
                    ApplyLightShaders(rays[i].Direction, ref colorAcc[i], depth, hitPrimitives[i], intersections[i]);
                }
            }

            for (int i = 0; i < rays.Length; i++)
            {
                //No hit
                if (hitPrimitives[i] == null) continue;
                ApplyRecurseShaders(rays[i], ref colorAcc[i], depth, refractionIndices[i], collisionResults[i], hitPrimitives[i], intersections[i]);
            }

            for (int i = 0; i < rays.Length; i++)
            {
                //No hit
                if (hitPrimitives[i] == null) continue;
                if (collisionResults[i] != RayCollision.HitFromInsidePrimitive)
                {
                    ApplyAmbientLight(ref colorAcc[i], hitPrimitives[i], intersections[i]);
                }
            }

            return hitPrimitives;
        }

        public Primitive[] FindNearestInSync(ref float[] distances, Ray[] rays, ref RayCollision[] collisionResults)
        {
            if (!_initialized) InitUnit();

            var hitPrimitives = new Primitive[rays.Length];
            if (_spatialStructure == null)
            {
                for (int i = 0; i < rays.Length; i++)
                {
                    //Default collision detection
                    foreach (Primitive primitive in _primitives)
                    {
                        RayCollision collision = primitive.Intersect(rays[i], ref distances[i]);
                        if (collision != RayCollision.Miss)
                        {
                            hitPrimitives[i] = primitive;
                            collisionResults[i] = collision;
                        }
                    }
                }
            }
            else
            {
                hitPrimitives = ((KdTree) _spatialStructure).GetClosestIntersectionPrimitiveInSync(rays, ref distances, out collisionResults);
            }

            return hitPrimitives;
        }

        #endregion
    }
}
