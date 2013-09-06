using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracer.PostProcessing
{
    public unsafe class RawBitmap : IDisposable
    {
        private BitmapData _bitmapData;
        private byte* _begin;

        private int _bytesPerChannel;
        private bool _singleChannel;
        private bool _alpha;
        private int _bytesPerPixel;

        public RawBitmap(Bitmap originBitmap)
            : this(originBitmap, PixelFormat.Format24bppRgb)
        {
            //OriginBitmap = originBitmap;
            //_bitmapData = OriginBitmap.LockBits(new Rectangle(0, 0, OriginBitmap.Width, OriginBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //_begin = (byte*)(void*)_bitmapData.Scan0;
        }

        public RawBitmap(Bitmap originBitmap, PixelFormat pixelFormat)
        {
            OriginBitmap = originBitmap;
            _bitmapData = OriginBitmap.LockBits(new Rectangle(0, 0, OriginBitmap.Width, OriginBitmap.Height), ImageLockMode.ReadWrite, pixelFormat);
            _begin = (byte*)(void*)_bitmapData.Scan0;

            _bytesPerChannel = 0;
            _singleChannel = false;
            _alpha = false;
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    _bytesPerChannel = 1;
                    break;
                case PixelFormat.Format32bppArgb:
                    _bytesPerChannel = 1;
                    _alpha = true;
                    break;
                case PixelFormat.Format48bppRgb:
                    _bytesPerChannel = 2;
                    break;
                case PixelFormat.Format64bppArgb:
                    _bytesPerChannel = 2;
                    _alpha = true;
                    break;
                case PixelFormat.Alpha:
                case PixelFormat.Canonical:
                case PixelFormat.DontCare:
                case PixelFormat.Extended:
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format64bppPArgb:
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Gdi:
                case PixelFormat.Indexed:
                case PixelFormat.Max:
                case PixelFormat.PAlpha:
                default:
                    throw new ArgumentException("Pixel format not supported");
            }
            _bytesPerPixel = _bytesPerChannel * (_singleChannel ? 1 : 3);
            if (_alpha) _bytesPerChannel += _bytesPerChannel;
        }

        #region IDisposable Members

        public void Dispose()
        {
            OriginBitmap.UnlockBits(_bitmapData);
        }

        #endregion

        public unsafe byte* Begin
        {
            get { return _begin; }
        }

        public unsafe byte* this[int x, int y]
        {
            get
            {
                return _begin + y * (_bitmapData.Stride) + x * _bytesPerPixel;
            }
        }

        public unsafe byte* this[int x, int y, int offset]
        {
            get
            {
                return _begin + y * (_bitmapData.Stride) + x * _bytesPerPixel + offset;
            }
        }

        public unsafe void SetColor(int x, int y, RayTracer.Geometry.Coloring.Color color)
        {
            byte* p = this[x, y];
            p[0] = color.B;
            p[1] = color.G;
            p[2] = color.R;
        }

        public unsafe RayTracer.Geometry.Coloring.Color GetColor(int x, int y)
        {
            byte* p = this[x, y];

            return new RayTracer.Geometry.Coloring.Color
            (
                p[2],
                p[1],
                p[0]
            );
        }

        public int Stride
        {
            get { return _bitmapData.Stride; }
        }

        public int Width
        {
            get { return _bitmapData.Width; }
        }

        public int Height
        {
            get { return _bitmapData.Height; }
        }

        public int GetOffset()
        {
            return _bitmapData.Stride - _bitmapData.Width * _bytesPerPixel;
        }

        public Bitmap OriginBitmap { get; private set; }
    }
}
