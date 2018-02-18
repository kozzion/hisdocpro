using AForge.Imaging.Filters;
using AForge.Math;
using HisDocProUI.Renderer;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HisDocProUI.Model
{
    public class ModelToken : ReactiveObject
    {
        private Bitmap _bitmapSource;

        private ImageSource _image;
        public ImageSource Image
        {
            get { return this._image; }
            set { this.RaiseAndSetIfChanged(ref this._image, value); }
        }

        private string _label;
        public string Label
        {
            get { return this._label; }
            set { this.RaiseAndSetIfChanged(ref this._label, value); }
        }
        
        public int Width
        {
            get { return _bitmapSource.Width; }
         
        }
        public int Height
        {
            get { return _bitmapSource.Height; }
        }

        public ModelToken(string path)
        {
            _bitmapSource = new Bitmap(path);
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(_bitmapSource);
            renderer.Render();
            Image = renderer.RenderedImage;
            Label = path.Split('_')[1];
        }

        public ModelToken()
        {
        }

        public Bitmap GetKernel(int initialThreshold)
        {
            //Convert to gray
            Bitmap bitmapTemp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(_bitmapSource);
            //bitmapTemp = new Threshold(initialThreshold).Apply(bitmapTemp);
            
           
            return _bitmapSource;
        }
    }
}
