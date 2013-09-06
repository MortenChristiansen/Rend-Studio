using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry;
using System.Runtime.InteropServices;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.SpatialStructures
{
    static unsafe class MemoryManager
    {
        private static uint _nodePointer;
        private static uint _maxNodes;

        public static KdTreeNode[] Nodes { get; private set; }
        public static Dictionary<uint, Primitive[]> PrimitiveLists { get; private set; }

        static MemoryManager()
        {
            FreeNodes();
        }

        public static uint GetFreePrimitiveListSlot()
        {
            for (uint i = 0; i < Nodes.GetLength(0); i++)
            {
                if (!PrimitiveLists.ContainsKey(i))
                {
                    PrimitiveLists.Add(i, new Primitive[0]);
                    return i;
                }
            }

            throw new Exception("No available slots!");
        }

        public static void FreePrimitiveListSlot(uint pointer)
        {
            PrimitiveLists.Remove(pointer);
        }
        /*
        public static KdTreeNode* AllocateNodes()
        {
            if (_nodePointer >= _maxNodes)
            {
                throw new Exception("Not tested yet!");
                / *
                KdTreeNode[] tmp = new KdTreeNode[_maxNodes];
                Buffer.BlockCopy(_nodes, 0, tmp, 0, _maxNodes);
                _maxNodes *= 2;
                _nodes = new KdTreeNode[_maxNodes];
                Buffer.BlockCopy(tmp, 0, _nodes, 0, tmp.GetLength(0));
                 * * /
            }
            _nodePointer += 2;
            _nodes[_nodePointer - 2] = new KdTreeNode();
            _nodes[_nodePointer - 1] = new KdTreeNode();
            //NodePointers[_nodes[_nodePointer - 2]] = _nodePointer - 2;
            //NodePointers[_nodes[_nodePointer - 1]] = _nodePointer - 1;
            fixed (KdTreeNode* n1 = &(_nodes[_nodePointer - 2])) fixed (KdTreeNode* n2 = &_nodes[_nodePointer - 1])
            {
                n1->SplitAxis = 3;
                n2->SplitAxis = 3;
                n1->SplitPlanePosition = float.NaN;
                n2->SplitPlanePosition = float.NaN;
                return n1;
            }
        }*/

        public static void FreeNodes()
        {
            _maxNodes = 4096;
            _nodePointer = 0;
            PrimitiveLists = new Dictionary<uint, Primitive[]>();
            Nodes = new KdTreeNode[_maxNodes];
        }
        /*
        public static uint AllocateNodes()
        {
            _nodePointer += 2;
            if (_nodePointer >= _maxNodes)
            {
                throw new Exception("Not tested yet!");
            }

            Nodes[_nodePointer - 2] = new KdTreeNode() { SplitAxis = 3, SplitPlanePosition = float.NaN };
            Nodes[_nodePointer - 1] = new KdTreeNode() { SplitAxis = 3, SplitPlanePosition = float.NaN };

            return _nodePointer - 2;
        }*/

        public static uint AllocateNodes(KdTreeNode leftNode, KdTreeNode rightNode)
        {
            if (_nodePointer + 2 >= _maxNodes)
            {
                throw new Exception("Not tested yet!");
            }

            Nodes[_nodePointer] = leftNode;
            Nodes[_nodePointer + 1] = rightNode;

            _nodePointer += 2;
            return _nodePointer - 2;
        }
    }
}
