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

        public ModelToken(string path)
        {
    
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(new Bitmap(path));
            renderer.Render();
            Image = renderer.RenderedImage;
        }
    }
}
