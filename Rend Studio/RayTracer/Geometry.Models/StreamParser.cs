using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using RayTracer.Mathematics;
using RayTracer.Geometry.Coloring;

namespace RayTracer.Geometry.Models
{
    public class StreamParser
    {
        private char[] _trimCharacters;
        private StreamReader _stream;

        public StreamParser(StreamReader stream, params char[] trimCharacters)
        {
            _stream = stream;
            _trimCharacters = trimCharacters;
        }

        public string Trim(string line)
        {
            return line.Trim(_trimCharacters);
        }

        public int ParseInteger()
        {
            string line = Trim(_stream.ReadLine());
            if (line == "")
            {
                line = Trim(_stream.ReadLine());
            }
            return Int32.Parse(line);
        }

        public int[] ParseIntegers(int count)
        {
            int[] integers = new int[count];
            for (int i = 0; i < count; i++)
            {
                integers[i] = ParseInteger();
            }

            return integers;
        }

        public float ParseFloat()
        {
            return float.Parse(Trim(_stream.ReadLine()), CultureInfo.InvariantCulture.NumberFormat);
        }

        public Vector ParseVector()
        {
            string[] values = Trim(_stream.ReadLine()).Split(new char[] { ';' });
            return new Vector(
                float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat), 
                float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat), 
                float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat));
        }

        public Vector[] ParseVectors(int count)
        {
            Vector[] vectors = new Vector[count];
            for (int i = 0; i < count; i++)
            {
                vectors[i] = ParseVector();
            }

            return vectors;
        }

        public Color ParseColor()
        {
            string[] values = Trim(_stream.ReadLine()).Split(new char[] { ';' });
            return new Color(
                (byte)(float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat) * 255),
                (byte)(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat) * 255),
                (byte)(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat) * 255));
        }

        public string ParseString()
        {
            return Trim(_stream.ReadLine());
        }

        public Coordinate[] ParseCoordinates()
        {
            int count = ParseInteger();
            Coordinate[] coords = new Coordinate[count];
            for (int i = 0; i < count; i++)
            {
                string[] values = Trim(_stream.ReadLine()).Split(new char[] { ';' });
                coords[i] = new Coordinate(float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat));
            }

            return coords;
        }
    }
}
