using AForge.Imaging.Filters;
using AForge.Math;
using HisDocProUI.Renderer;
using HisDocProUI.Tools;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HisDocProCL.Model
{
    public class ModelToken : ReactiveObject
    {
        public int Width
        {
            get { return _bitmapSource.Width; }

        }
        public int Height
        {
            get { return _bitmapSource.Height; }
        }

        private string _filePathImage;
        private string _filePathJson;
        private Bitmap _bitmapSource;
        private double [,] _kernel;

        private ImageSource _image;
        public ImageSource Image
        {
            get { return this._image; }
            set { this.RaiseAndSetIfChanged(ref this._image, value); }
        }

        private ModelValueDouble _threshold;
        public ModelValueDouble Threshold
        {
            get { return this._threshold; }
            set { this.RaiseAndSetIfChanged(ref this._threshold, value); }
        }

        private ModelValueDouble _weigth;
        public ModelValueDouble Weigth
        {
            get { return this._weigth; }
            set { this.RaiseAndSetIfChanged(ref this._weigth, value); }
        }

        private string _label;
        public string Label
        {
            get { return this._label; }
            set { this.RaiseAndSetIfChanged(ref this._label, value); Save(); }
        }
        private IModelApplication _application;

        public ModelToken(IModelApplication application, string filePathImage, string filePathJson)
        {
            _application = application;
            _filePathImage = filePathImage;
            _filePathJson = filePathJson;
            _bitmapSource = new Bitmap(filePathImage);

            if ((_bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) ||
                (_bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                _bitmapSource = ToolsConvolution.ConvertTo24(_bitmapSource);
            }

            Bitmap bitmapTemp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(_bitmapSource);
            bitmapTemp = new Threshold(100).Apply(bitmapTemp);
            bitmapTemp = new Invert().Apply(bitmapTemp);
            _kernel = ToolsConvolution.BitMapToDoubleArray(bitmapTemp);
            _threshold = new ModelValueDouble(application.EventTokenChanged, 0);
            _weigth = new ModelValueDouble(application.EventTokenChanged, 0);


            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(_bitmapSource);
            renderer.Render();
            Image = renderer.RenderedImage;

            if (File.Exists(filePathJson))
            {
                Load();
            }
            else
            {         
                this._threshold = new ModelValueDouble(_application.EventTokenChanged, 0.8);
                this._weigth = new ModelValueDouble(_application.EventTokenChanged, 1.0);
                this._label = filePathImage.Split('_')[1];
                Save();
            }
        }

        public ModelToken()
        {
        }

        public double[,] GetKernel()
        {
            return _kernel;
        }

        public void Load()
        {
            string json = File.ReadAllText(_filePathJson);
            TokenV001 layout = JsonConvert.DeserializeObject<TokenV001>(json);
            SetValue(layout);
        }

        public void Save()
        {
            if (_filePathJson != null)
            {
                string json = JsonConvert.SerializeObject(GetValue(), Formatting.Indented);
                File.WriteAllText(_filePathJson, json);
            }
        }

        private void SetValue(TokenV001 token)
        {
            this._threshold = new ModelValueDouble(_application.EventTokenChanged, token.Threshold);
            this._weigth = new ModelValueDouble(_application.EventTokenChanged, token.Weigth);
            this._label = token.Label;
        }

        private TokenV001 GetValue()
        {
            TokenV001 token = new TokenV001();
            token.Threshold = Threshold.Value;
            token.Weigth = Weigth.Value;
            token.Label = Label;
            return token;
        }

        public class TokenV001
        {
            public double Threshold;
            public double Weigth;
            public string Label;
        }
    }
}
