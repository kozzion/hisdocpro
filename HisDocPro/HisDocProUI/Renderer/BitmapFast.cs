
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HisDocProUI.Renderer
{
    public class BitmapFast
    {
        private BitmapData _bitmapData;
        private bool _locked;

        public Bitmap Bitmap { get; }

        public BitmapFast(int size_x, int size_y)
        {
            Bitmap = new Bitmap(size_x, size_y, PixelFormat.Format32bppArgb);
            _locked = false;
        }

        public BitmapFast(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
            }
            Bitmap = bitmap;
            _locked = false;
        }


        public void Lock()
        {
            if (_locked)
            {
                throw new Exception("Bitmap already locked.");
            }

            var rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            _bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            _locked = true;
        }

        public void Unlock()
        {
            if (!_locked)
            {
                throw new Exception("Bitmap not locked.");
            }

            Bitmap.UnlockBits(_bitmapData);
        }

        public void SetPixel(int x, int y, Color colour)
        {
            if (!_locked)
            {
                throw new Exception("Bitmap not locked.");
            }

            var argb = colour.ToArgb();
            var offset = _bitmapData.Stride * y + x * 4;
            Marshal.WriteInt32(_bitmapData.Scan0, offset, argb);
        }
    }
}
