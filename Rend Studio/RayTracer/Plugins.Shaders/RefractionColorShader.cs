using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Shaders
{
    public class RefractionColorShader : IRecurseShader
    {
        #region IRecurseShader Members

        public void AdjustColor(ref Color colorAcc, Vector intersection, Primitive hitPrimitive, Ray ray, int depth, float rayRefraction, RayCollision collisionResult)
        {
            if (hitPrimitive.Material.RefractionIndex <= 0 || hitPrimitive.Material.Transparency <= 0 || depth >= RenderEngine.Configuration.Depth)
            {
                return;
            }

            float primitiveRefraction = hitPrimitive.Material.RefractionIndex;
            float n = rayRefraction / primitiveRefraction;
            Vector nn = hitPrimitive.GetNormal(intersection) * (float)collisionResult;
            float cosI = -Vector.Dot(nn, ray.Direction);
            float cosT2 = 1 - n * n * (1 - cosI * cosI);
            if (cosT2 > 0)
            {
                Vector t = (n * ray.Direction) + (n * cosI - (float)Math.Sqrt(cosT2)) * nn;
                Color refractedColor = Color.Black;
                float dist;
                Primitive refractionPrimitive = RenderEngine.RenderUnit.RayTrace(new Ray(intersection + t * Vector.Epsilon, t), ref refractedColor, depth + 1, primitiveRefraction, out dist);
                if (collisionResult == RayCollision.Hit && hitPrimitive.Material.Absorbance > 0)
                {
                    //Apply Beer's law (light_out = light_in * e–(distance * materialDensity))
                    float coeff = hitPrimitive.Material.GetBeerCoefficient(dist);// (float)Math.Pow(3, hitPrimitive.Material.Absorbance * -dist);
                    Color cOut = refractedColor * coeff;

                    //Here we take a number of samples inside the transparent material to determine the amount of light which it gets
                    int shadeIterations = 20;
                    float distanceSegment = dist / shadeIterations;
                    Color diffuseBeer = Color.Black;
                    for (int i = 0; i < shadeIterations; i++)
                    {
                        foreach (var light in RenderEngine.Scene.Lights)
                        {
                            Vector intersection2 = intersection + ray.Direction * distanceSegment * i;
                            float shade = RenderEngine.Configuration.ShadowTracer == null ? 1.0f : RenderEngine.Configuration.ShadowTracer.CalculateShade(intersection2, light, depth);
                            diffuseBeer += hitPrimitive.Material.Color * ((1 - coeff)* shade * light.GetLightCoefficient(intersection2) / shadeIterations);
                        }
                    }
                    cOut += (refractedColor * (1 - coeff)).Filter(hitPrimitive.Material.Color * (1 - coeff)) + diffuseBeer;
                    RenderEngine.Scene.AmbientLights.ForEach(light => cOut += (light.Color * light.Brightness).Filter(hitPrimitive.Material.Color * (1 - coeff)));

                    //This line gives the same amount of color regardless of light
                    //cOut += hitPrimitive.Material.Color * (1 - coeff);
                    colorAcc += cOut * hitPrimitive.Material.Transparency;
                }
                else
                {
                    colorAcc += refractedColor * hitPrimitive.Material.Transparency;
                }
            }
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Default Refraction"; }
        }

        #endregion
    }
}

//for each 10th of a segment between the intersection and the intersection of the refracted beam,
//  add the ligh of each light source to the output light (each light only adds one 10th) - based on shade