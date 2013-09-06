using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RayTracer.PostProcessing
{
    [Serializable]
    public enum BlurType
    {
        Both,
        HorizontalOnly,
        VerticalOnly,
    }

    [Serializable]
    public class GaussianBlur
    {
        private int _radius = 1;
        private int[] _kernel;
        private int _kernelSum;
        private int[,] _multable;

        public GaussianBlur()
        {
            PreCalculateSomeStuff();
        }

        public GaussianBlur(int radius)
        {
            _radius = radius;
            PreCalculateSomeStuff();
        }

        private void PreCalculateSomeStuff()
        {
            int sz = _radius * 2 + 1;
            _kernel = new int[sz];
            _multable = new int[sz, 256];
            for (int i = 1; i <= _radius; i++)
            {
                int szi = _radius - i;
                int szj = _radius + i;
                _kernel[szj] = _kernel[szi] = (szi + 1) * (szi + 1);
                _kernelSum += (_kernel[szj] + _kernel[szi]);
                for (int j = 0; j < 256; j++)
                {
                    _multable[szj, j] = _multable[szi, j] = _kernel[szj] * j;
                }
            }
            _kernel[_radius] = (_radius + 1) * (_radius + 1);
            _kernelSum += _kernel[_radius];
            for (int j = 0; j < 256; j++)
            {
                _multable[_radius, j] = _kernel[_radius] * j;
            }
        }

        public long t1;
        public long t2;
        public long t3;
        public long t4;

        public Bitmap ProcessImage(Image inputImage)
        {
            Bitmap origin = new Bitmap(inputImage);
            Bitmap blurred = new Bitmap(inputImage.Width, inputImage.Height);

            using (RawBitmap src = new RawBitmap(origin))
            using (RawBitmap dest = new RawBitmap(blurred))
            {
                int pixelCount = src.Width * src.Height;

                int[] b = new int[pixelCount];
                int[] g = new int[pixelCount];
                int[] r = new int[pixelCount];

                int[] b2 = new int[pixelCount];
                int[] g2 = new int[pixelCount];
                int[] r2 = new int[pixelCount];

                int offset = src.GetOffset();
                int index = 0;
                unsafe
                {
                    byte* ptr = src.Begin;
                    for (int i = 0; i < src.Height; i++)
                    {
                        for (int j = 0; j < src.Width; j++)
                        {
                            b[index] = *ptr;
                            ptr++;
                            g[index] = *ptr;
                            ptr++;
                            r[index] = *ptr;
                            ptr++;

                            ++index;
                        }
                        ptr += offset;
                    }

                    int bsum;
                    int gsum;
                    int rsum;
                    int read;
                    int start = 0;
                    index = 0;

                    if (BlurType != BlurType.VerticalOnly)
                    {
                        for (int i = 0; i < src.Height; i++)
                        {
                            for (int j = 0; j < src.Width; j++)
                            {
                                bsum = gsum = rsum = 0;
                                read = index - _radius;

                                for (int z = 0; z < _kernel.Length; z++)
                                {
                                    if (read < start)
                                    {
                                        bsum += _multable[z, b[start]];
                                        gsum += _multable[z, g[start]];
                                        rsum += _multable[z, r[start]];
                                    }
                                    else if (read > start + src.Width - 1)
                                    {
                                        int idx = start + src.Width - 1;
                                        bsum += _multable[z, b[idx]];
                                        gsum += _multable[z, g[idx]];
                                        rsum += _multable[z, r[idx]];
                                    }
                                    else
                                    {
                                        bsum += _multable[z, b[read]];
                                        gsum += _multable[z, g[read]];
                                        rsum += _multable[z, r[read]];
                                    }
                                    ++read;
                                }

                                b2[index] = (bsum / _kernelSum);
                                g2[index] = (gsum / _kernelSum);
                                r2[index] = (rsum / _kernelSum);

                                if (BlurType == BlurType.HorizontalOnly)
                                {
                                    byte* pcell = dest[j, i];
                                    *pcell = (byte)(bsum / _kernelSum);
                                    pcell++;
                                    *pcell = (byte)(gsum / _kernelSum);
                                    pcell++;
                                    *pcell = (byte)(rsum / _kernelSum);
                                    pcell++;
                                }

                                ++index;
                            }
                            start += src.Width;
                        }
                    }

                    if (BlurType == BlurType.HorizontalOnly) return blurred;

                    int tempy;
                    for (int i = 0; i < src.Height; i++)
                    {
                        int y = i - _radius;
                        start = y * src.Width;
                        for (int j = 0; j < src.Width; j++)
                        {
                            bsum = gsum = rsum = 0;
                            read = start + j;
                            tempy = y;
                            for (int z = 0; z < _kernel.Length; z++)
                            {
                                if (BlurType == BlurType.VerticalOnly)
                                {
                                    if (tempy < 0)
                                    {
                                        bsum += _multable[z, b[j]];
                                        gsum += _multable[z, g[j]];
                                        rsum += _multable[z, r[j]];
                                    }
                                    else if (tempy > src.Height - 1)
                                    {
                                        int idx = pixelCount - (src.Width - j);
                                        bsum += _multable[z, b[idx]];
                                        gsum += _multable[z, g[idx]];
                                        rsum += _multable[z, r[idx]];
                                    }
                                    else
                                    {
                                        bsum += _multable[z, b[read]];
                                        gsum += _multable[z, g[read]];
                                        rsum += _multable[z, r[read]];
                                    }
                                }
                                else
                                {
                                    if (tempy < 0)
                                    {
                                        bsum += _multable[z, b2[j]];
                                        gsum += _multable[z, g2[j]];
                                        rsum += _multable[z, r2[j]];
                                    }
                                    else if (tempy > src.Height - 1)
                                    {
                                        int idx = pixelCount - (src.Width - j);
                                        bsum += _multable[z, b2[idx]];
                                        gsum += _multable[z, g2[idx]];
                                        rsum += _multable[z, r2[idx]];
                                    }
                                    else
                                    {
                                        bsum += _multable[z, b2[read]];
                                        gsum += _multable[z, g2[read]];
                                        rsum += _multable[z, r2[read]];
                                    }
                                }


                                read += src.Width;
                                ++tempy;
                            }

                            byte* pcell = dest[j, i];

                            pcell[0] = (byte)(bsum / _kernelSum);
                            pcell[1] = (byte)(gsum / _kernelSum);
                            pcell[2] = (byte)(rsum / _kernelSum);
                        }
                    }
                }
            }
            return blurred;
        }

        public int Radius
        {
            get { return _radius; }
            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException("Radius must be greater then 0");
                }
                _radius = value;
                PreCalculateSomeStuff();
            }
        }

        public BlurType BlurType { get; set; }
    }
}
