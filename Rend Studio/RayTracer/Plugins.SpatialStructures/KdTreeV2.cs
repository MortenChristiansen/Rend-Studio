//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RayTracer.Geometry;
//using RayTracer.Mathematics;

//namespace RayTracer.Plugins.SpatialStructures
//{
//    public unsafe class KdTreeV2 : ISpatialStructure
//    {
//        private int _maxDepth;
//        private int _minElements;
//        private bool _isTreeBuilt;
//        private BoundingBox _boundingBox;
//        private KdTreeNode _root;
//        protected List<Primitive> _primitives;

//        private byte _axis = 0;

//        private const double COST_INTERSECTION = 1.0d;
//        private const double COST_TRAVERSAL = 0.3d;

//        public int NodeCount { get; private set; }

//        ~KdTreeV2()
//        {
//            MemoryManager.FreeNodes();
//        }

//        private KdTreeV2(int maxDepth, int minElements, List<Primitive> primitives, BoundingBox boundingBox)
//        {
//            _primitives = primitives;
//            _maxDepth = maxDepth;
//            _minElements = minElements;
//            _boundingBox = boundingBox;

//            MemoryManager.FreeNodes();
//            NodeCount = 1;
//        }

//        public KdTreeV2()
//        {

//        }

//        private KdTreeNode BuildTree(KdTreeNode node, int depth, List<Primitive> primitives, BoundingBox boundingBox)
//        {
//            List<Primitive> leftList;
//            BoundingBox leftBox;
//            List<Primitive> rightList;
//            BoundingBox rightBox;

//            if (depth >= _maxDepth || !Split(ref node, primitives, out leftList, out rightList, boundingBox, out leftBox, out rightBox))
//            {
//                node.SplitAxis = 3;
//                node.IsLeaf = true;
//                node.SplitPlanePosition = float.NaN;
//                node.Primitives = primitives.ToArray();
//            }
//            else
//            {
//                NodeCount += 2;
//                KdTreeNode left = BuildTree(node.LeftChild, depth + 1, leftList, leftBox);
//                KdTreeNode right = BuildTree(node.RightChild, depth + 1, rightList, rightBox);
//                node.SpawnChildren(left, right);
//            }

//            return node;
//        }

//        private bool Split(ref KdTreeNode node, List<Primitive> primitives, out List<Primitive> leftPrimitives, out List<Primitive> rightPrimitives, BoundingBox boundingBox, out BoundingBox leftBox, out BoundingBox rightBox)
//        {
//            leftPrimitives = new List<Primitive>();
//            rightPrimitives = new List<Primitive>();
//            leftBox = new BoundingBox();
//            rightBox = new BoundingBox();

//            //Dertermine splittability
//            if (primitives.Count < _minElements)
//            {
//                return false;
//            }

//            byte axis;
//            double splitPosition = FindOptimalSplitPosition(out axis, primitives, boundingBox);
//            if (double.IsNaN(splitPosition))
//            {
//                return false;
//            }
//            node.SplitAxis = axis;
//            node.SplitPlanePosition = (float)splitPosition;

//            List<Primitive> listA = new List<Primitive>();
//            List<Primitive> listB = new List<Primitive>();

//            Vector positionB;
//            Vector sizeA = boundingBox.Size;
//            Vector sizeB = boundingBox.Size;

//            //Calculate position and size for split boxes
//            sizeA = Vector.SetValue(sizeA, axis, splitPosition - boundingBox.Position[axis]);
//            sizeB = Vector.SetValue(sizeB, axis, boundingBox.Size[axis] - sizeA[axis]);
//            positionB = Vector.AddValue(boundingBox.Position, axis, sizeA[axis]);

//            leftBox = new BoundingBox(boundingBox.Position, sizeA);
//            rightBox = new BoundingBox(positionB, sizeB);

//            BoundingBox b1 = new BoundingBox(leftBox.Position - new Vector(0.01d, 0.01d, 0.01d), leftBox.Size + new Vector(0.02d, 0.02d, 0.02d));
//            BoundingBox b2 = new BoundingBox(rightBox.Position - new Vector(0.01d, 0.01d, 0.01d), rightBox.Size + new Vector(0.02d, 0.02d, 0.02d));

//            //Put primitives into the proper list
//            foreach (Primitive primitive in primitives)
//            {
//                //Detect which list it belongs to
//                if (primitive.Intersect(b1))
//                {
//                    listA.Add(primitive);
//                }
//                if (primitive.Intersect(b2))
//                {
//                    listB.Add(primitive);
//                }
//            }
//            leftPrimitives = listA;
//            rightPrimitives = listB;

//            return true;
//        }

//        private double FindOptimalSplitPosition(out byte axis, List<Primitive> primitives, BoundingBox boundingbox)
//        {
//            //Determine best axis - not done currently
//            axis = _axis;

//            double lowestCost = primitives.Count * boundingbox.CalculateSurfaceArea() * COST_INTERSECTION;
//            double bestPosition = double.NaN;
//            bool atLeftSceneEdge = false;
//            bool atRightSceneEdge = false;

//            foreach (Primitive primitive in primitives)
//            {
//                double leftExtreme;
//                double rightExtreme;
//                BoundingBox bb = primitive.GetBoundingBox();

//                //Calculate the boundaries for the primitive
//                leftExtreme = bb.Position[axis];
//                rightExtreme = (bb.Position[axis] + bb.Size[axis]);
//                if (leftExtreme == boundingbox.Position[axis]) atLeftSceneEdge = true;
//                if (rightExtreme == (boundingbox.Position + boundingbox.Size)[axis]) atRightSceneEdge = true;

//                if (!atLeftSceneEdge)
//                {
//                    double cost = CalculateCost(leftExtreme, axis, boundingbox, primitives);
//                    if (cost < lowestCost)
//                    {
//                        lowestCost = cost;
//                        bestPosition = leftExtreme;
//                    }
//                }
//                if (!atRightSceneEdge)
//                {
//                    double cost = CalculateCost(rightExtreme, axis, boundingbox, primitives);
//                    if (cost < lowestCost)
//                    {
//                        lowestCost = cost;
//                        bestPosition = rightExtreme;
//                    }
//                }
//            }

//            _axis = Vector.NextAxis[_axis];
//            return bestPosition;
//        }

//        private double CalculateCost(double splitPosition, byte axis, BoundingBox boundingBox, List<Primitive> primitives)
//        {
//            int leftCount = 0;
//            int rightCount = 0;
//            BoundingBox leftBB;
//            BoundingBox rightBB;

//            //Create bounding boxes for new nodes
//            double splitOffset = splitPosition - boundingBox.Position[axis];
//            Vector newSizeA = Vector.SetValue(boundingBox.Size, axis, splitOffset);
//            Vector newPositionB = Vector.AddValue(boundingBox.Position, axis, splitOffset);
//            Vector newSizeB = Vector.AddValue(boundingBox.Size, axis, -splitOffset);
//            leftBB = new BoundingBox(boundingBox.Position, newSizeA);
//            rightBB = new BoundingBox(newPositionB, newSizeB);

//            double leftArea = leftBB.CalculateSurfaceArea();
//            double rightArea = rightBB.CalculateSurfaceArea();

//            foreach (Primitive primitive in primitives)
//            {
//                if (primitive.Intersect(leftBB)) leftCount++;
//                if (primitive.Intersect(rightBB)) rightCount++;
//            }

//            return COST_TRAVERSAL + COST_INTERSECTION * (leftArea * leftCount + rightArea * rightCount);
//        }

//        private bool RayBoxIntersect(Ray ray, BoundingBox box, ref double entryPointLength, ref double exitPointLength)
//        {
//            entryPointLength = double.MaxValue;
//            exitPointLength = double.MaxValue;

//            if (box.Contains(ray.Origin))
//            {
//                box.Intersect(ray, ref exitPointLength);
//                Ray inverseRay = new Ray(ray.Origin, -ray.Direction);
//                box.Intersect(inverseRay, ref entryPointLength);
//                entryPointLength *= -1.0d;
//                return true;
//            }
//            else
//            {
//                RayCollision collision = box.Intersect(ray, ref entryPointLength);
//                if (collision == RayCollision.Hit)
//                {
//                    Ray newRay = new Ray(ray.Origin + ray.Direction * entryPointLength + (ray.Direction * Vector.EPSILON), ray.Direction);
//                    box.Intersect(newRay, ref exitPointLength);
//                    exitPointLength += Math.Max(0.0d, entryPointLength) + (ray.Direction * Vector.EPSILON).Length;

//                    return true;
//                }
//            }
//            return false;
//        }

//        private struct StackElement
//        {
//            public double T;
//            public Vector PB;
//            public int PreviousElement;
//            public KdTreeNode Node;
//        }

//        #region SpatialStructure Members

//        public Primitive GetClosestIntersectionPrimitive(Ray ray, ref double distance)
//        {
//            double a = 0.0d; //Entry signed distance
//            double b = 0.0d; //Exit signed distance
//            double t = 0.0d; //Signed distance to splitting plane

//            //Intersect ray with scene box, find entry and exit signed distances
//            bool intersects = RayBoxIntersect(ray, _boundingBox, ref a, ref b);

//            if (!intersects) return null;

//            //Stack required for traversal to store far childre
//            StackElement[] stack = new StackElement[_maxDepth];

//            //Pointers to the far child node and current node
//            KdTreeNode farChild;
//            KdTreeNode currNode;
//            currNode = _root; //Start form the tree's root node

//            //Setup the entrypoint of the stack
//            int entryPoint = 0;
//            stack[entryPoint].T = a;

//            //Distinguish between internal and external origin
//            if (a > 0.0d)
//            {
//                stack[entryPoint].PB = ray.Origin + ray.Direction * a;
//            }
//            else //A ray with internal origin
//            {
//                stack[entryPoint].PB = ray.Origin;
//            }

//            //Setup the initial exit point in the stack
//            int exitPoint = 1; //Pointer to the stack
//            stack[exitPoint].T = b;
//            stack[exitPoint].PB = ray.Origin + ray.Direction * b;
//            stack[exitPoint].Node = KdTreeNode.NOWHERE; //Set termination flag

//            //Loop, traverse through the whole kd-tree, until an object
//            //is intersected or ray leaves the scene
//            while (!currNode.IsNowhere)
//            {
//                //Loop until a leaf is found
//                while (!currNode.IsLeaf)
//                {
//                    //Retrieve position of splitting plane
//                    double splitVal = (double)currNode.SplitPlanePosition;

//                    //Similar code for all axes
//                    byte axis = currNode.SplitAxis;

//                    if (splitVal.AlmostEqual(ray.Origin[axis])) //Cases Z
//                    {
//                        if (a > 0.0d)
//                        {
//                            if (stack[exitPoint].PB[axis] > 0.0d) //Case Z1
//                            {
//                                currNode = currNode.RightChild;
//                                continue;
//                            }
//                            else //Case Z2, Z3
//                            {
//                                currNode = currNode.LeftChild;
//                                continue;
//                            }
//                        }
//                        else
//                        {
//                            if (stack[exitPoint].PB[axis] > 0.0d) //Case Z1
//                            {
//                                farChild = currNode.RightChild;
//                                currNode = currNode.LeftChild;
//                            }
//                            else //Case Z2, Z3
//                            {
//                                farChild = currNode.LeftChild;
//                                currNode = currNode.RightChild;
//                            }
//                        }
//                    }
//                    else if (stack[entryPoint].PB[axis].AlmostEqual(splitVal)) //Cases I1, I2
//                    {
//                        if (stack[exitPoint].PB[axis] > 0.0d) //Case I1
//                        {
//                            farChild = currNode.RightChild;
//                            currNode = currNode.LeftChild;
//                        }
//                        else //Case I2
//                        {
//                            farChild = currNode.LeftChild;
//                            currNode = currNode.RightChild;
//                        }
//                    }
//                    else if (stack[exitPoint].PB[axis].AlmostEqual(splitVal))
//                    {
//                        if (stack[entryPoint].PB[axis] > 0.0d) //Case I3
//                        {
//                            farChild = currNode.LeftChild;
//                            currNode = currNode.RightChild;
//                        }
//                        else //Case I4
//                        {
//                            farChild = currNode.RightChild;
//                            currNode = currNode.LeftChild;
//                        }
//                    }
//                    else if (stack[entryPoint].PB[axis] < splitVal)
//                    {
//                        if (stack[exitPoint].PB[axis] < splitVal)
//                        {
//                            currNode = currNode.LeftChild;
//                            continue; //Case N1, N2, N3, P5
//                        }

//                        //Case N4
//                        farChild = currNode.RightChild;
//                        currNode = currNode.LeftChild;
//                    }
//                    else
//                    {
//                        if (stack[exitPoint].PB[axis] > splitVal)
//                        {//Case P1, P2, P3 and N5
//                            currNode = currNode.RightChild;
//                            continue;
//                        }
//                        //Case P4
//                        farChild = currNode.LeftChild;
//                        currNode = currNode.RightChild;
//                    }
//                    //Case P4 or N4 ... traverse both children
//                    //This is the most expensive part of the algorithm

//                    //Signed distance to the splitting plane
//                    if (splitVal - ray.Origin[axis] != 0.0d)
//                    {
//                        t = ((splitVal - ray.Origin[axis]) / ray.Direction[axis]);
//                    }
//                    else
//                    {
//                        t = 0.0d;
//                    }

//                    //Setup new exit point
//                    int tmp = exitPoint;
//                    exitPoint++;

//                    //Possibly skip current entrypoint so not to overwrite data
//                    if (exitPoint == entryPoint)
//                    {
//                        exitPoint++;
//                    }

//                    //Push values onto stack
//                    stack[exitPoint].PreviousElement = tmp;
//                    stack[exitPoint].T = t;
//                    stack[exitPoint].Node = farChild;
//                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, axis, splitVal);
//                    byte next = Vector.NextAxis[axis];
//                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, next, ray.Origin[next] + ray.Direction[next] * t);
//                    byte prev = Vector.PreviousAxis[axis];
//                    stack[exitPoint].PB = Vector.SetValue(stack[exitPoint].PB, prev, ray.Origin[prev] + ray.Direction[prev] * t);
//                }

//                //Current node is the leaf ... empty or full

//                //Intersect ray with each object in the object list, discarding
//                //those lying before stack[entryPoint].T or farther than stack[exitPoint].T
//                double dist = stack[exitPoint].T;
//                bool intersection = false;
//                Primitive closestPrimitive = null;

//                foreach (Primitive primitive in currNode.Primitives)
//                {
//                    if (primitive.Intersect(ray, ref dist) == RayCollision.Hit)
//                    {
//                        distance = dist;
//                        closestPrimitive = primitive;
//                        intersection = true;
//                    }
//                }

//                if (intersection) return closestPrimitive;

//                //Pop from the stack
//                entryPoint = exitPoint; //The signed distance intervals are adjacent

//                //Retrieve the pointer to the next node, it is possible that ray traversal terminates
//                currNode = stack[exitPoint].Node;

//                exitPoint = stack[entryPoint].PreviousElement;
//            }

//            //currNode = NOWHERE, ray leaves the scene
//            return null;
//        }

//        public void Initialize()
//        {
//            if (_isTreeBuilt == false)
//            {
//                _root = new KdTreeNode() { SplitAxis = 3, SplitPlanePosition = float.NaN };
//                _root.SpawnChildren(new KdTreeNode(), new KdTreeNode());
//                _root = BuildTree(_root, 0, _primitives, _boundingBox);
//                _isTreeBuilt = true;
//            }
//        }

//        public ISpatialStructure GetStructureInstance(List<Primitive> primitives)
//        {
//            return new KdTreeV2(16, 3, primitives, BoundingBox.CalculateBoundingBox(primitives));
//        }

//        #endregion

//        #region IPlugin Members

//        public string Name
//        {
//            get { return "Optimized Kd Tree"; }
//        }

//        #endregion
//    }
//}
