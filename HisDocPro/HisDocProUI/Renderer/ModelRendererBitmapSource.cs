using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HisDocProUI.Renderer
{
    public class ModelRendererBitmapSource : ReactiveObject
    {
        private readonly int _width;
        private readonly int _height;
        private BitmapSource _renderedImage;
        public BitmapSource RenderedImage
        {
            get { return this._renderedImage; }
            set { this.RaiseAndSetIfChanged(ref this._renderedImage, value); }
        }

        private BitmapFast _bitmapFast;

        public ModelRendererBitmapSource(Bitmap bitmap)
        {
            _width = bitmap.Width;
            _height = bitmap.Height;
            _bitmapFast = new BitmapFast(bitmap);
            RenderedImage = CreateBitmapSourceFromBitmap(_bitmapFast.Bitmap);
        }

        public void Render()
        {
            _bitmapFast.Lock();

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    //_bitmapFast.SetPixel(x, y, bitmap[automata.CurrentState[x + y * _width]]);
                }
            }

            _bitmapFast.Unlock();
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmap_data = bitmap.LockBits(
                rectangle,
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            try
            {
                var size = rectangle.Width * rectangle.Height * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    System.Windows.Media.PixelFormats.Bgr32,
                    null,
                    bitmap_data.Scan0,
                    size,
                    bitmap_data.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmap_data);
            }
        }


    }
}
