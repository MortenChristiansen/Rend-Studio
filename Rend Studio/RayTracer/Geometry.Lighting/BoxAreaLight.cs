using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Geometry.Lighting
{
    public class BoxAreaLight : AreaLight
    {
        public BoxAreaLight(float brightness, Box shape)
            : base(brightness, shape)
        {

        }

        internal override Vector[] GetSamplingVectors(Vector point)
        {
            int gridSize = 5;

            BoundingBox box = Shape.GetBoundingBox();
            List<Vector> ret = new List<Vector>();
            Vector direction = box.GetCenter() - point;
            float squareAngle = (float)Math.PI * 0.5f;
            MersenneTwister rnd = new MersenneTwister(true);

            for (byte axis = 0; axis < 3; axis++)
            {
                float edgeHeightIncrement;
                float edgeWidthIncrement;
                Vector edgePosition;
                byte prev = Vector.NextAxis[axis];
                byte next = Vector.PreviousAxis[axis];

                Vector normal = Vector.AddValue(new Vector(), axis, 1);
                float angle = Vector.Angle(direction, normal);
                if (angle > squareAngle)
                {
                    //Set grid to the one with normal1
                    edgePosition = Vector.AddValue(box.Position, axis, box.Size[axis]);
                }
                else
                {
                    if (angle == squareAngle) continue;
                    //Set grid to the one with normal1.inverse
                    edgePosition = box.Position;
                }
                //Find vectors from edge grid
                edgeWidthIncrement = box.Size[next] / gridSize;
                edgeHeightIncrement = box.Size[prev] / gridSize;

                for (int y = 0; y < gridSize; y++) for (int x = 0; x < gridSize; x++)
                {
                    float xOffset = x * edgeWidthIncrement + rnd.Rand() * edgeWidthIncrement;
                    float yOffset = y * edgeHeightIncrement + rnd.Rand() * edgeHeightIncrement;
                    ret.Add(Vector.AddValue(Vector.AddValue(edgePosition, next, xOffset), prev, yOffset) - point);
                }
            }

            return ret.ToArray();
        }
    }
}
