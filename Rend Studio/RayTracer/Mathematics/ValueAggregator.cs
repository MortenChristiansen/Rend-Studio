using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    public class ValueAggregator
    {
        public int ByteValues { get; private set; }
        public int IntValues { get; private set; }
        public int FloatValues { get; private set; }
        public int DoubleValues { get; private set; }

        private byte _byteValue;
        private int _intValue;
        private float _floatValue;
        private double _doubleValue;

        public ValueAggregator()
        {
            ClearValues();
        }

        public void ClearValues()
        {
            _byteValue = 0;
            ByteValues = 0;
            _intValue = 0;
            IntValues = 0;
            _floatValue = 0;
            FloatValues = 0;
            _doubleValue = 0;
            DoubleValues = 0;
        }

        public void AddValue(byte value)
        {
            _byteValue += value;
            ByteValues++;
        }

        public void AddValue(int value)
        {
            _intValue += value;
            IntValues++;
        }

        public void AddValue(float value)
        {
            _floatValue += value;
            FloatValues++;
        }

        public void AddValue(double value)
        {
            _doubleValue += value;
            DoubleValues++;
        }

        public byte AverageBytes()
        {
            return (byte)(_byteValue / ByteValues);
        }

        public int AverageInts()
        {
            return _intValue / IntValues;
        }

        public float AverageFloats()
        {
            return _floatValue / (float)FloatValues;
        }

        public double AverageDoubles()
        {
            return _doubleValue / (double)DoubleValues;
        }
    }
}
