using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.SpatialStructures
{
    public class KdTree : ISpatialStructure
    {
        private int _maxDepth;
        private int _minElements;
        private Node _root;
        private bool _isTreeBuilt;
        protected Primitive[] _primitives;

        public int NodeCount { get; private set; }
        public BoundingBox BoundingBox { get; private set; }

        private KdTree(int maxDepth, int minElements, Primitive[] primitives, BoundingBox boundingBox)
        {
            BoundingBox = boundingBox;

            _primitives = primitives;
            _maxDepth = maxDepth;
            _minElements = minElements;
            _root = new Node(primitives, boundingBox);

            NodeCount = 1;
        }

        public KdTree()
        {

        }

        private void BuildTree(Node node, byte axis, int depth)
        {
            if (depth >= _maxDepth || !node.Split(axis, _minElements))
            {
                return;
            }
            NodeCount += 2;
            axis = Vector.NextAxis[axis];
            BuildTree(node.NodeA, axis, depth + 1);
            BuildTree(node.NodeB, axis, depth + 1);
        }

        private Node NOWHERE = new Node(new Primitive[0], new BoundingBox(new Vector(), new Vector()));

        private bool RayBoxIntersect(Ray ray, BoundingBox box, ref float entryPointLength, ref float exitPointLength)
        {
            entryPointLength = float.MaxValue;
            exitPointLength = float.MaxValue;

            if (box.Contains(ray.Origin))
            {
                box.Intersect(ray, ref exitPointLength);
                Ray inverseRay = new Ray(ray.Origin, -ray.Direction);
                box.Intersect(inverseRay, ref entryPointLength);
                entryPointLength *= -1;
                return true;
            }
            else
            {
                RayCollision collision = box.Intersect(ray, ref entryPointLength);
                if (collision == RayCollision.Hit)
                {
                    Ray newRay = new Ray(ray.Origin + ray.Direction * entryPointLength + (ray.Direction * Vector.Epsilon), ray.Direction);
                    box.Intersect(newRay, ref exitPointLength);
                    exitPointLength += Math.Max(0, entryPointLength) + (ray.Direction * Vector.Epsilon).Length;

                    return true;
                }
            }
            return false;
        }

        private struct StackElement
        {
            public float T;
            public Vector PB;
            public int PreviousElement;
            public Node Node;
        }

        private class Node
        {
            public Node NodeA { get; private set; }
            public Node NodeB { get; private set; }
            public Primitive[] Primitives { get; set; }
            public BoundingBox BoundingBox { get; private set; }
            public bool IsLeaf { get { return NodeA == null && NodeB == null; } }
            public float? SplitPosition { get; private set; }
            public byte SplitAxis { get; private set; }

            private float _area;
            private float _noSplitCost;

            private const float COST_INTERSECTION = 1;
            private const float COST_TRAVERSAL = 0.3f;

            public Node(Primitive[] primitives, BoundingBox boundingBox)
            {
                Primitives = primitives;
                BoundingBox = boundingBox;

                _area = BoundingBox.CalculateSurfaceArea();
                SplitPosition = null;
                SplitAxis = 3;
            }

            public Node(BoundingBox boundingBox)
                : this(null, boundingBox)
            {
                
            }

            public bool Split(byte axis, int minElements)
            {
                //Dertermine splittability
                if (Primitives.Length < minElements)
                {
                    return false;
                }

                _noSplitCost = Primitives.Length * _area * COST_INTERSECTION;
                SplitPosition = FindOptimalSplitPosition(axis);
                SplitAxis = axis;
                if (SplitPosition == null)
                {
                    return false;
                }
                float splitPosition = (float)SplitPosition;

                List<Primitive> listA = new List<Primitive>();
                List<Primitive> listB = new List<Primitive>();

                Vector positionB;
                Vector sizeA = BoundingBox.Size;
                Vector sizeB = BoundingBox.Size;

                //Calculate position and size for split boxes
                sizeA = Vector.SetValue(sizeA, axis, splitPosition - BoundingBox.Position[axis]);
                sizeB = Vector.SetValue(sizeB, axis, BoundingBox.Size[axis] - sizeA[axis]);
                positionB = Vector.AddValue(BoundingBox.Position, axis, sizeA[axis]);

                NodeA = new Node(new BoundingBox(BoundingBox.Position, sizeA));
                NodeB = new Node(new BoundingBox(positionB, sizeB));

                //Put primitives into the proper list
                foreach (Primitive primitive in Primitives)
                {
                    //Detect which list it belongs to
                    if (primitive.Intersect(NodeA.BoundingBox))
                    {
                        listA.Add(primitive);
                    }
                    if (primitive.Intersect(NodeB.BoundingBox))
                    {
                        listB.Add(primitive);
                    }
                }
                NodeA.Primitives = listA.ToArray();
                NodeB.Primitives = listB.ToArray();
                Primitives = null;

                return true;
            }

            private float? FindOptimalSplitPosition(byte axis)
            {
                float lowestCost = _noSplitCost;
                float? bestPosition = null;
                
                foreach (Primitive primitive in Primitives)
                {
                    bool atLeftSceneEdge = false;
                    bool atRightSceneEdge = false;
                    float leftExtreme;
                    float rightExtreme;
                    BoundingBox bb = primitive.GetBoundingBox();

                    //Calculate the boundaries for the primitive
                    leftExtreme = bb.Position[axis];
                    rightExtreme = bb.Position[axis] + bb.Size[axis];
                    if (leftExtreme == BoundingBox.Position[axis]) atLeftSceneEdge = true;
                    if (rightExtreme == (BoundingBox.Position + BoundingBox.Size)[axis]) atRightSceneEdge = true;

                    if (!atLeftSceneEdge)
                    {
                        float cost = CalculateCost(leftExtreme, axis);
                        if (cost < lowestCost)
                        {
                            lowestCost = cost;
                            bestPosition = leftExtreme;
                        }
                    }
                    if (!atRightSceneEdge)
                    {
                        float cost = CalculateCost(rightExtreme, axis);
                        if (cost < lowestCost)
                        {
                            lowestCost = cost;
                            bestPosition = rightExtreme;
                        }
                    }
                }

                return bestPosition;
            }

            private float CalculateCost(float splitPosition, byte axis)
            {
                int leftCount = 0;
                int rightCount = 0;
                BoundingBox leftBB;
                BoundingBox rightBB;

                //Create bounding boxes for new nodes
                float splitOffset = splitPosition - BoundingBox.Position[axis];
                Vector newSizeA = Vector.SetValue(BoundingBox.Size, axis, splitOffset);
                Vector newPositionB = Vector.AddValue(BoundingBox.Position, axis, splitOffset);
                Vector newSizeB = Vector.AddValue(BoundingBox.Size, axis, -splitOffset);
                leftBB = new BoundingBox(BoundingBox.Position, newSizeA);
                rightBB = new BoundingBox(newPositionB, newSizeB);

                float leftArea = leftBB.CalculateSurfaceArea();
                float rightArea = rightBB.CalculateSurfaceArea();

                foreach (Primitive primitive in Primitives)
                {
                    if (primitive.Intersect(leftBB)) leftCount++;
                    if (primitive.Intersect(rightBB)) rightCount++;
                }

                return COST_TRAVERSAL + COST_INTERSECTION * (leftArea * leftCount + rightArea * rightCount);
            }
        }

        #region SpatialStructure Members

        public Primitive GetClosestIntersectionPrimitive(Ray ray, ref float distance, out RayCollision collisionResult)
        {
            float a = 0; //Entry signed distance
            float b = 0; //Exit signed distance
            float t = 0; //Signed distance to splitting plane

            //Intersect ray with scene box, find entry and exit signed distances
            bool intersects = RayBoxIntersect(ray, _root.BoundingBox, ref a, ref b);

            if (!intersects)
            {
                collisionResult = RayCollision.Miss;
                return null;
            }

            //Stack required for traversal to store far childre
            StackElement[] stack = new StackElement[_maxDepth];

            //Pointers to the far child node and current node
            Node farChild;
            Node currNode = _root; //Start form the tree's root node

            //Setup the entrypoint of the stack
            int entryPoint = 0;
            stack[entryPoint].T = a;

            //Distinguish between internal and external origin
            if (a > 0.0d)
            {
                stack[entryPoint].PB = ray.Origin + ray.Direction * a;
            }
            else //A ray with internal origin
            {
                stack[entryPoint].PB = ray.Origin;
            }

            //Setup the initial exit point in the stack
            int exitPoint = 1; //Pointer to the stack
            stack[exitPoint].T = b;
            stack[exitPoint].PB = ray.Origin + ray.Direction * b;
            stack[exitPoint].Node = NOWHERE; //Set termination flag

            //Loop, traverse through the whole kd-tree, until an object
            //is intersected or ray leaves the scene
            while (currNode != NOWHERE)
            {
                //Loop until a leaf is found
                while (!currNode.IsLeaf)
                {
                    //Retrieve position of splitting plane
                    float splitVal = (float)currNode.SplitPosition;

                    //Similar code for all axes
                    byte axis = currNode.SplitAxis;

                    if (splitVal.AlmostEqual(ray.Origin[axis])) //Cases Z
                    {
                        if (a > 0)
                        {
                            if (stack[exitPoint].PB[axis] > 0) //Case Z1
                            {
                                currNode = currNode.NodeB;
                                continue;
                            }
                            else //Case Z2, Z3
                            {
                                currNode = currNode.NodeA;
                                continue;
                            }
                        }
                        else
                        {
                            if (stack[exitPoint].PB[axis] > 0) //Case Z1
                            {
                                farChild = currNode.NodeB;
                                currNode = currNode.NodeA;
                            }
                            else //Case Z2, Z3
                            {
                                farChild = currNode.NodeA;
                                currNode = currNode.NodeB;
                            }
                        }
                    }
                    else if (stack[entryPoint].PB[axis].AlmostEqual(splitVal)) //Cases I1, I2
                    {
                        if (stack[exitPoint].PB[axis] > 0) //Case I1
                        {
                            farChild = currNode.NodeB;
                            currNode = currNode.NodeA;
                        }
                        else //Case I2
                        {
                            farChild = currNode.NodeA;
                            currNode = currNode.NodeB;
                        }
                    }
                    else if (stack[exitPoint].PB[axis].AlmostEqual(splitVal))
                    {
                        if (stack[entryPoint].PB[axis] > 0) //Case I3
                        {
                            farChild = currNode.NodeA;
                            currNode = currNode.NodeB;
                        }
                        else //Case I4
                        {
                            farChild = currNode.NodeB;
                            currNode = currNode.NodeA;
                        }
                    }
                    else if (stack[entryPoint].PB[axis] < splitVal)
                    {
                        if (stack[exitPoint].PB[axis] < splitVal)
                        {
                            currNode = currNode.NodeA;
                            continue; //Case N1, N2, N3, P5
                        }

                        //Case N4
                        farChild = currNode.NodeB;
                        currNode = currNode.NodeA;
                    }
                    else
                    {
                        if (stack[exitPoint].PB[axis] > splitVal)
                        {//Case P1, P2, P3 and N5
                            currNode = currNode.NodeB;
                            continue;
                        }
                        //Case P4
                        farChild = currNode.NodeA;
                        currNode = currNode.NodeB;
                    }
                    //Case P4 or N4 ... traverse both children
                    //This is the most expensive part of the algorithm

                    //Signed distance to the splitting plane
                    if (splitVal - ray.Origin[axis] != 0)
                    {
                        t = (splitVal - ray.Origin[axis]) / ray.Direction[axis];
                    }
                    else
                    {
                        t = 0;
                    }

                    //Setup new exit point
                    int tmp = exitPoint;
                    exitPoint++;

                    //Possibly skip current entrypoint so not to overwrite data
                    if (exitPoint == entryPoint)
                    {
                        exitPoint++;
                    }

                    //Push values onto stack
                    stack[exitPoint].PreviousElement = tmp;
                    stack[exitPoint].T = t;
                    stack[exitPoint].Node = farChild;
                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, axis, splitVal);
                    byte next = Vector.NextAxis[axis];
                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, next, ray.Origin[next] + ray.Direction[next] * t);
                    byte prev = Vector.PreviousAxis[axis];
                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, prev, ray.Origin[prev] + ray.Direction[prev] * t);
                }

                //Current node is the leaf ... empty or full

                //Intersect ray with each object in the object list, discarding
                //those lying before stack[entryPoint].T or farther than stack[exitPoint].T
                float dist = stack[exitPoint].T;
                bool intersection = false;
                Primitive closestPrimitive = null;

                collisionResult = RayCollision.Miss;
                foreach (Primitive primitive in currNode.Primitives)
                {
                    RayCollision result = primitive.Intersect(ray, ref dist);
                    if (result != RayCollision.Miss)
                    {
                        distance = dist;
                        closestPrimitive = primitive;
                        intersection = true;
                        collisionResult = result;
                    }
                }

                if (intersection) return closestPrimitive;

                //Pop from the stack
                entryPoint = exitPoint; //The signed distance intervals are adjacent

                //Retrieve the pointer to the next node, it is possible that ray traversal terminates
                currNode = stack[exitPoint].Node;

                exitPoint = stack[entryPoint].PreviousElement;
            }

            //currNode = NOWHERE, ray leaves the scene
            collisionResult = RayCollision.Miss;
            return null;
        }

        public void Initialize()
        {
            if (_isTreeBuilt == false)
            {
                BuildTree(_root, 0, 0);
                _isTreeBuilt = true;
            }
        }

        public ISpatialStructure GetStructureInstance(Primitive[] primitives)
        {
            return new KdTree(16, 30, primitives, BoundingBox.CalculateBoundingBox(primitives.ToList()));
        }

        public int MinimumPrimitives
        {
            get
            {
                return _minElements;
            }
        }

        #endregion

        #region IPlugin Members

        public string Name
        {
            get { return "Kd Tree"; }
        }

        #endregion

        #region InSync

        public Primitive[] GetClosestIntersectionPrimitiveInSync(Ray[] rays, ref float[] distances, out RayCollision[] collisionResults)
        {
            collisionResults = new RayCollision[rays.Length];
            var intersectionPrimitives = new Primitive[rays.Length];

            float[] a = new float[rays.Length]; //Entry signed distance
            float[] b = new float[rays.Length]; //Exit signed distance
            float[] t = new float[rays.Length]; //Signed distance to splitting plane

            //Intersect ray with scene box, find entry and exit signed distances
            var intersects = new bool[rays.Length];
            for (int i = 0; i < rays.Length; i++)
            {
                a[i] = 0;
                b[i] = 0;
                t[i] = 0;
                intersects[i] = RayBoxIntersect(rays[i], _root.BoundingBox, ref a[i], ref b[i]);
                if (!intersects[i]) collisionResults[i] = RayCollision.Miss;
            }

            for (int i = 0; i < rays.Length; i++)
            {
                if (!intersects[i]) continue;

                //Stack required for traversal to store far childre
                StackElement[] stack = new StackElement[_maxDepth];

                //Pointers to the far child node and current node
                Node farChild;
                Node currNode = _root; //Start form the tree's root node

                //Setup the entrypoint of the stack
                int entryPoint = 0;
                stack[entryPoint].T = a[i];

                //Distinguish between internal and external origin
                if (a[i] > 0.0d)
                {
                    stack[entryPoint].PB = rays[i].Origin + rays[i].Direction * a[i];
                }
                else //A ray with internal origin
                {
                    stack[entryPoint].PB = rays[i].Origin;
                }

                //Setup the initial exit point in the stack
                int exitPoint = 1; //Pointer to the stack
                stack[exitPoint].T = b[i];
                stack[exitPoint].PB = rays[i].Origin + rays[i].Direction * b[i];
                stack[exitPoint].Node = NOWHERE; //Set termination flag

                //Loop, traverse through the whole kd-tree, until an object
                //is intersected or ray leaves the scene
                while (currNode != NOWHERE)
                {
                    //Loop until a leaf is found
                    while (!currNode.IsLeaf)
                    {
                        //Retrieve position of splitting plane
                        float splitVal = (float)currNode.SplitPosition;

                        //Similar code for all axes
                        byte axis = currNode.SplitAxis;

                        if (splitVal.AlmostEqual(rays[i].Origin[axis])) //Cases Z
                        {
                            if (a[i] > 0)
                            {
                                if (stack[exitPoint].PB[axis] > 0) //Case Z1
                                {
                                    currNode = currNode.NodeB;
                                    continue;
                                }
                                else //Case Z2, Z3
                                {
                                    currNode = currNode.NodeA;
                                    continue;
                                }
                            }
                            else
                            {
                                if (stack[exitPoint].PB[axis] > 0) //Case Z1
                                {
                                    farChild = currNode.NodeB;
                                    currNode = currNode.NodeA;
                                }
                                else //Case Z2, Z3
                                {
                                    farChild = currNode.NodeA;
                                    currNode = currNode.NodeB;
                                }
                            }
                        }
                        else if (stack[entryPoint].PB[axis].AlmostEqual(splitVal)) //Cases I1, I2
                        {
                            if (stack[exitPoint].PB[axis] > 0) //Case I1
                            {
                                farChild = currNode.NodeB;
                                currNode = currNode.NodeA;
                            }
                            else //Case I2
                            {
                                farChild = currNode.NodeA;
                                currNode = currNode.NodeB;
                            }
                        }
                        else if (stack[exitPoint].PB[axis].AlmostEqual(splitVal))
                        {
                            if (stack[entryPoint].PB[axis] > 0) //Case I3
                            {
                                farChild = currNode.NodeA;
                                currNode = currNode.NodeB;
                            }
                            else //Case I4
                            {
                                farChild = currNode.NodeB;
                                currNode = currNode.NodeA;
                            }
                        }
                        else if (stack[entryPoint].PB[axis] < splitVal)
                        {
                            if (stack[exitPoint].PB[axis] < splitVal)
                            {
                                currNode = currNode.NodeA;
                                continue; //Case N1, N2, N3, P5
                            }

                            //Case N4
                            farChild = currNode.NodeB;
                            currNode = currNode.NodeA;
                        }
                        else
                        {
                            if (stack[exitPoint].PB[axis] > splitVal)
                            {//Case P1, P2, P3 and N5
                                currNode = currNode.NodeB;
                                continue;
                            }
                            //Case P4
                            farChild = currNode.NodeA;
                            currNode = currNode.NodeB;
                        }
                        //Case P4 or N4 ... traverse both children
                        //This is the most expensive part of the algorithm

                        //Signed distance to the splitting plane
                        if (splitVal - rays[i].Origin[axis] != 0)
                        {
                            t[i] = (splitVal - rays[i].Origin[axis]) / rays[i].Direction[axis];
                        }
                        else
                        {
                            t[i] = 0;
                        }

                        //Setup new exit point
                        int tmp = exitPoint;
                        exitPoint++;

                        //Possibly skip current entrypoint so not to overwrite data
                        if (exitPoint == entryPoint)
                        {
                            exitPoint++;
                        }

                        //Push values onto stack
                        stack[exitPoint].PreviousElement = tmp;
                        stack[exitPoint].T = t[i];
                        stack[exitPoint].Node = farChild;
                        stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, axis, splitVal);
                        byte next = Vector.NextAxis[axis];
                        stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, next, rays[i].Origin[next] + rays[i].Direction[next] * t[i]);
                        byte prev = Vector.PreviousAxis[axis];
                        stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, prev, rays[i].Origin[prev] + rays[i].Direction[prev] * t[i]);
                    }

                    //Current node is the leaf ... empty or full

                    //Intersect ray with each object in the object list, discarding
                    //those lying before stack[entryPoint].T or farther than stack[exitPoint].T
                    float dist = stack[exitPoint].T;
                    bool intersection = false;
                    Primitive closestPrimitive = null;

                    collisionResults[i] = RayCollision.Miss;
                    foreach (Primitive primitive in currNode.Primitives)
                    {
                        RayCollision result = primitive.Intersect(rays[i], ref dist);
                        if (result != RayCollision.Miss)
                        {
                            distances[i] = dist;
                            closestPrimitive = primitive;
                            intersection = true;
                            collisionResults[i] = result;
                        }
                    }

                    if (intersection)
                    {
                        intersectionPrimitives[i] = closestPrimitive;
                        break;
                    }

                    //Pop from the stack
                    entryPoint = exitPoint; //The signed distance intervals are adjacent

                    //Retrieve the pointer to the next node, it is possible that ray traversal terminates
                    currNode = stack[exitPoint].Node;

                    exitPoint = stack[entryPoint].PreviousElement;
                }
                if (intersectionPrimitives[i] != null) continue;

                //currNode = NOWHERE, ray leaves the scene
                collisionResults[i] = RayCollision.Miss;
                intersectionPrimitives[i] = null;
            }
            return intersectionPrimitives;
        }

        #endregion
    }
}

/*
 Alternative algorithm
 * 
 * 1 -Have six lists a1&2, b1&2 and c1&2, which each contains all the leaf nodes, sorted by one dimansion each (nodes at the same coordinate value are put in a sub list sorted in the next direction).
 * Each pair is sorted along a direction forwards and backwards. Possibly, when more nodes are available for a direction value, an interval on the next axis is specified insted (make sure endless loops are not possible in this way)
 * 
 * 2 - first, use a binary search to find the starting node
 * 
 * 3 - then, compare with all primitives in node
 * 
 * 4 - then, find the next candidates by taking the next nodes from each list where the relevant axis of the ray direction is positive and the previous where is is negative. If there 
 * are more than one node in a direction (i.e. a list), binary search is used on the next direction to find the best match (and in the third direction if applicable)
 * 
 * 5 - then, calculate the actual next node to use and goto 3
 * 
 * whenever a primitive is hit, return
 */