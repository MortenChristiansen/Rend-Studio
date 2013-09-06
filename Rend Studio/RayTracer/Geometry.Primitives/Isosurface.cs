//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RayTracer.Mathematics;
//using RayTracer.Geometry.Coloring;

//namespace RayTracer.Geometry.Primitives
//{
//    public class Isosurface : Primitive
//    {
//        private List<Vector> _centers;
//        private List<float> _potentials;
//        private const int zoneNumber = 10;
//        private float[] zoneTab = new float[zoneNumber]
//        {   
//            10.0f,
//            5.0f,   
//            3.33333f,
//            2.5f,
//            2.0f,
//            1.66667f,
//            1.42857f,
//            1.25f,
//            1.1111f,
//            1.0f,
//        };

//        public Isosurface()
//            : base(new Vector())
//        {
//            _centers = new List<Vector>();
//            _potentials = new List<float>();
//        }

//        public void AddPotential(Vector center, float potential)
//        {
//            _centers.Add(center);
//            _potentials.Add(potential);
//        }

//        internal override RayCollision Intersect(Ray ray, ref double distance)
//        {
//            // Having a static structure helps performance more than two times !
//            // It obviously wouldn't work if we were running in multiple threads..
//            // But it helps considerably for now
//            static vector<poly> polynomMap;
//            polynomMap.resize(0);

//            float rSquare, rInvSquare;
//            rSquare = b.size * b.size;
//            rInvSquare = b.invSizeSquare;
//            float maxEstimatedPotential = 0.0f;

//            // outside of all the influence spheres, the potential is zero
//            double a = 0.0f;
//            double b = 0.0f;
//            double c = 0.0f;

//            for (int i = 0; i < _centers.Count; i++)
//            {
//                Vector currentPoint = _centers[i];

//                Vector vDist = currentPoint - ray.Origin;
//                a = 1.0f;
//                b = - 2.0f * ray.Direction * vDist;
//                c = vDist * vDist; 
//                // Accelerate delta computation by keeping common computation outside of the loop
//                double BSquareOverFourMinusC = 0.25f * b * b - c;
//                double MinusBOverTwo = -0.5f * b; 
//                double ATimeInvSquare = a * rInvSquare;
//                double BTimeInvSquare = b * rInvSquare;
//                double CTimeInvSquare = c * rInvSquare;

//                // the current sphere, has N zones of influences
//                // we go through each one of them, as long as we've detected
//                // that the intersecting ray has hit them
//                // Since all the influence zones of many spheres
//                // are imbricated, we compute the influence of the current sphere
//                // by computing the delta of the previous polygon
//                // that way, even if we reorder the zones later by their distance
//                // on the ray, we can still have our estimate of 
//                // the potential function.
//                // What is implicit here is that it only works because we've approximated
//                // 1/dist^2 by a linear function of dist^2
//                for (int j=0; j < zoneNumber - 1; j++)
//                {
//                    // We compute the "delta" of the second degree equation for the current
//                    // spheric zone. If it's negative it means there is no intersection
//                    // of that spheric zone with the intersecting ray
//                    const float fDelta = BSquareOverFourMinusC + zoneTab[j].fCoef * rSquare;
//                    if (fDelta < 0.0f) 
//                    {
//                        // Zones go from bigger to smaller, so that if we don't hit the current one,
//                        // there is no chance we hit the smaller one
//                        break;
//                    }
//                    float sqrtDelta = (float)Math.Sqrt(fDelta);
//                    float t0 = (float)MinusBOverTwo - sqrtDelta; 
//                    float t1 = (float)MinusBOverTwo + sqrtDelta;

//                    // because we took the square root (a positive number), it's implicit that 
//                    // t0 is smaller than t1, so we know which is the entering point (into the current
//                    // sphere) and which is the exiting point.
//                    poly poly0 = {zoneTab[j].fGamma * ATimeInvSquare ,
//                                  zoneTab[j].fGamma * BTimeInvSquare , 
//                                  zoneTab[j].fGamma * CTimeInvSquare + zoneTab[j].fBeta,
//                                  t0,
//                                  zoneTab[j].fDeltaFInvSquare}; 
//                    poly poly1 = {- poly0.a, - poly0.b, - poly0.c, 
//                                  t1, 
//                                  -poly0.fDeltaFInvSquare};
                    
//                    maxEstimatedPotential += zoneTab[j].fDeltaFInvSquare;

//                    // just put them in the vector at the end
//                    // we'll sort all those point by distance later
//                    polynomMap.push_back(poly0);
//                    polynomMap.push_back(poly1);
//                };
//            }

//            if (polynomMap.size() < 2 || maxEstimatedPotential < 1.0f)
//            {
//                return RayCollision.Miss;
//            }
            
//            // sort the various entry/exit points per distance
//            // by going from the smaller distance to the bigger
//            // we can reconstruct the field approximately along the way
//            std::sort(polynomMap.begin(),polynomMap.end(), IsLessPredicate());

//            maxEstimatedPotential = 0.0f;
//            bool bResult = false;
//            vector<poly>::const_iterator it = polynomMap.begin();
//            vector<poly>::const_iterator itNext = it + 1;
//            for (; itNext != polynomMap.end(); it = itNext, ++itNext)
//            {
//                // A * x2 + B * y + C, defines the condition under which the intersecting
//                // ray intersects the equipotential surface. It works because we designed it that way
//                // (refer to the article).
//                A += it->a;
//                B += it->b;
//                C += it->c;
//                maxEstimatedPotential += it->fDeltaFInvSquare;
//                if (maxEstimatedPotential < 1.0f)
//                {
//                    // No chance that the potential will hit 1.0f in this zone, go to the next zone
//                    // just go to the next zone, we may have more luck
//                    continue;
//                }
//                const float fZoneStart =  it->fDistance;
//                const float fZoneEnd = itNext->fDistance;

//                // the current zone limits may be outside the ray start and the ray end
//                // if that's the case just go to the next zone, we may have more luck
//                if (t > fZoneStart &&  0.01f < fZoneEnd )
//                {
//                    // This is the exact resolution of the second degree
//                    // equation that we've built
//                    // of course after all the approximation we've done
//                    // we're not going to have the exact point on the iso surface
//                    // but we should be close enough to not see artifacts
//                    float fDelta = B * B - 4.0f * A * (C - 1.0f) ;
//                    if (fDelta < 0.0f)
//                    {
//                        continue;
//                    }

//                    float fInvA = (0.5f / A);
//                    float fSqrtDelta = (float)Math.Sqrt(fDelta);

//                    float t0 = fInvA * (- B - fSqrtDelta); 
//                    float t1 = fInvA * (- B + fSqrtDelta);
//                    if ((t0 > 0.01f ) && (t0 >= fZoneStart ) && (t0 < fZoneEnd) && (t0 <= t ))
//                    {
//                        t = t0;
//                        bResult = true;
//                    }
                    
//                    if ((t1 > 0.01f ) && (t1 >= fZoneStart ) && (t1 < fZoneEnd) && (t1 <= t ))
//                    {
//                        t = t1;
//                        bResult = true;
//                    }

//                    if (bResult)
//                    {
//                        return RayCollision.Hit;
//                    }
//                }
//            }
//            return RayCollision.Miss;
//        }

//        internal override Vector GetNormal(Vector intersection)
//        {
//            throw new NotImplementedException();
//        }

//        internal override BoundingBox GetBoundingBox()
//        {
//            throw new NotImplementedException();
//        }

//        internal override bool Intersect(BoundingBox box)
//        {
//            throw new NotImplementedException();
//        }

//        internal override Color GetColor(Vector intersection)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
