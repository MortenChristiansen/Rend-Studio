using System;
using RayTracer.Mathematics;

namespace RayTracer.Geometry.Coloring
{
    public class Material
    {
        /// <summary>
        /// The amount of light which is diffusely reflected.
        /// </summary>
        public float Diffuse { get { return Math.Max(1 - Transparency - Reflection, 0); } }

        /// <summary>
        /// The amount of light which is perfectly reflected.
        /// </summary>
        public float Reflection { get; set; }

        /// <summary>
        /// The amount of specular reflection.
        /// </summary>
        public float Specular { get; set; }

        /// <summary>
        /// The refraction index of the material, determining the angle
        /// at which the light travels through the material. Is only used
        /// for materials which have some level of transparency.
        /// </summary>
        public float RefractionIndex { get; set; }

        /// <summary>
        /// The amount of light which travels through the material. 0 is entirely
        /// opaque while 1 is entirely translucent.
        /// </summary>
        public float Transparency { get; set; }

        /// <summary>
        /// The amount of light which is absorbed while traveling
        /// through the material. Is only used for materials which 
        /// have some level of transparency.
        /// </summary>
        public float Absorbance { get; set; }

        /// <summary>
        /// The base color of the material.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The texture used to determine the color of the material
        /// at a given point. Replaces the Color property unless textures
        /// are disabled.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// The texture uses a texture to change the intersection normals
        /// based on the grey scale value of the texture pixel. The lighter
        /// the pixel is, the higher the elevation of the pixel. The same
        /// texture coordinates as the diffuse texture is used.
        /// </summary>
        public Texture BumpMap { get; set; }

        /// <summary>
        /// Determines how much height difference the bump map simulates.
        /// </summary>
        public float BumpFactor { get; set; }

        public Material(float reflection, float transparency, float refractionIndex, Color color)
        {
            Reflection = reflection.Clamp(0, 1);
            Specular = reflection.Clamp(0, 1);
            Transparency = transparency.Clamp(0, 1);
            RefractionIndex = refractionIndex;
            Color = color;
        }

        public Material(float reflection, float diffuse, float specular, float refractionIndex, float transparency, Color color)
        {
            Reflection = reflection.Clamp(0, 1);
            Specular = specular.Clamp(0, 1);
            RefractionIndex = refractionIndex;
            Transparency = transparency.Clamp(0, 1);
            Color = color;
        }

        public Material()
        {
            Reflection = 0;
            Specular = 0;
            RefractionIndex = 1;
            Transparency = 0;
            Color = Color.Black;
            Absorbance = 0;
            BumpFactor = 1;
        }

        private bool _isLiquid = true;
        public void SetAsLiquid()
        {
            _isLiquid = true;
        }

        public void SetAsGas()
        {
            _isLiquid = false;
        }

        public float GetBeerCoefficient(float distance)
        {
            if (_isLiquid) return (float)Math.Pow(10, Absorbance * -distance);
            else return (float)Math.Exp(Absorbance * -distance);
        }

        public Material Clone()
        {
            return new Material()
            {
                Texture = Texture,
                Absorbance = Absorbance,
                Reflection = Reflection,
                Specular = Specular,
                RefractionIndex = RefractionIndex,
                Transparency = Transparency,
                Color = Color,
                BumpMap = BumpMap,
                BumpFactor = BumpFactor
            };
        }

        #region Static Materials

        public static readonly Material WhiteDiamond = new Material(0.5f, 0, 0.7f, 2.417f, 0.95f, Color.White) { Absorbance = 0.002f }; //0.044 dispersion
        public static readonly Material Ruby = new Material(0.5f, 0, 0.7f, 1.77f, 0.95f, Color.Red) { Absorbance = 0.005f }; //0.018 dispersion
        public static readonly Material MatteWhite = new Material(0, 0, 1, Color.White);
        public static readonly Material MatteRed = new Material(0, 0, 1, Color.Red);
        public static readonly Material MatteGreen = new Material(0, 0, 1, Color.Green);
        public static readonly Material MatteBlue = new Material(0, 0, 1, Color.Blue);
        public static readonly Material GlossyWhite = new Material(.4f, 0, 1, Color.White);
        public static readonly Material GlossyRed = new Material(.4f, 0, 1, Color.Red);
        public static readonly Material GlossyGreen = new Material(.4f, 0, 1, Color.Green);
        public static readonly Material GlossyBlue = new Material(.4f, 0, 1, Color.Blue);

        #endregion
    }
}
