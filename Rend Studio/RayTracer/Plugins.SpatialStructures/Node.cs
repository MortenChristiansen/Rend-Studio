using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry;
using System.Runtime.InteropServices;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.SpatialStructures
{
    [StructLayout(LayoutKind.Sequential)]
    struct KdTreeNode
    {
        public static readonly KdTreeNode NOWHERE = new KdTreeNode { SplitAxis = 3, IsLeaf = false };

        private uint _data;
        private float _splitPlanePosition;
        
        //private List<Primitive> _prims;
        //private bool _isLeaf;
        //private byte _splitAxis;
        //private uint _left;
        //private uint _right;
        //private bool init;

        public float SplitPlanePosition
        {
            get
            {
                return _splitPlanePosition;
            }
            set
            {
                if (!float.IsNaN(value) && IsLeaf) throw new Exception("Split position assigned to leaf node!");
                _splitPlanePosition = value;
            }
        }
        public bool IsNowhere
        {
            get
            {
                return !IsLeaf && SplitAxis == 3;
            }
        }
        public bool IsLeaf
        {
            get
            {
                //return _isLeaf; //
                return (_data & 4) > 0;
            }
            set
            {
                //_isLeaf = value; //

                _data = value ? (_data | 4) : (_data & 0xfffffffb);
                if (value)
                {
                    uint listPointer = MemoryManager.GetFreePrimitiveListSlot();
                    _data = (listPointer << 3) + (_data & 7);
                    SplitPlanePosition = float.NaN;
                }
                
                //if (value)
                //{
                    //if (!init)
                    //{
                  //      uint listPointer = MemoryManager.GetFreePrimitiveListSlot();
                   //     _data = (listPointer << 3) + (_data & 7);
                        //init = true;
                   // }
                //}
                //else
                //{
                    //if (init)
                    //{
                        //MemoryManager.FreePrimitiveListSlot((uint)((_data & 0xfffffff8) >> 3));
                        //init = false;
                    //}
                //}
                
            }
        }
        public byte SplitAxis
        {
            get
            {
                //return _splitAxis; //
                return (byte)(_data & 3);
            }
            set
            {
                if (value > 3) throw new Exception("Invalid axis");
                //IsLeaf = value == 3 ? true : false;
                //_splitAxis = value; //
                _data = (_data & 0xfffffffc) + (uint)value;
            }
        }
        public unsafe KdTreeNode LeftChild
        {
            get
            {
                if (IsLeaf) return NOWHERE;
                //return MemoryManager.Nodes[_left]; //
                return MemoryManager.Nodes[(uint)((_data & 0xfffffff8) >> 3)];
            }
        }
        public unsafe KdTreeNode RightChild
        {
            get
            {
                if (IsLeaf) return NOWHERE;
                //return MemoryManager.Nodes[_right]; //
                return MemoryManager.Nodes[(uint)((_data & 0xfffffff8) >> 3) + 1];
            }
        }
        public unsafe Primitive[] Primitives
        {
            get
            {
                
                uint id = (uint)((_data & 0xfffffff8) >> 3);
                return MemoryManager.PrimitiveLists[id];
                
                //if (_prims == null) _prims = new List<Primitive>(); //
                //return _prims; //
            }
            set
            {
                if (!IsLeaf) throw new Exception("Node is not a leaf!");
                uint id = (uint)((_data & 0xfffffff8) >> 3);
                MemoryManager.PrimitiveLists[id] = value;
            }
        }

        public void SpawnChildren(KdTreeNode leftNode, KdTreeNode rightNode)
        {
            //_left = MemoryManager.AllocateNodes(leftNode, rightNode); //
            //_right = _left + 1; //
            _data = (uint)(MemoryManager.AllocateNodes(leftNode, rightNode) << 3) + (_data & 7);
        }
    }
}
