using AForge.Imaging;
using AForge.Imaging.Filters;
using HisDocProUI.Renderer;
using HisDocProUI.Tools;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing.Imaging;

namespace HisDocProUI.Model
{
    public class ModelMain : ReactiveObject
    {
        public IList<string> FileList { get; private set; }
        private string fileSelected;
        public string FileSelected
        {
            get { return this.fileSelected; }
            set { this.RaiseAndSetIfChanged(ref this.fileSelected, value); }
        }

        public IList<ModelToken> TokenList { get; private set; }
        private ModelToken _tokenlSelected;
        public ModelToken TokenSelected
        {
            get { return this._tokenlSelected; }
            set { this.RaiseAndSetIfChanged(ref this._tokenlSelected, value); }
        }


        private Bitmap bitmapSource;

        private ImageSource imageSourceSource;
        public ImageSource ImageSourceSource
        {
            get { return this.imageSourceSource; }
            set { this.RaiseAndSetIfChanged(ref this.imageSourceSource, value); }
        }

        private ImageSource imageSourceTarget;
        public ImageSource ImageSourceTarget
        {
            get { return this.imageSourceTarget; }
            set { this.RaiseAndSetIfChanged(ref this.imageSourceTarget, value); }
        }

        private double _initialRotation;
        public double InitialRotation
        {
            get { return this._initialRotation; }
            set { this.RaiseAndSetIfChanged(ref this._initialRotation, value); }
        }

        private int _initialThreshold;
        public int InitialThreshold
        {
            get { return this._initialThreshold; }
            set { this.RaiseAndSetIfChanged(ref this._initialThreshold, value); }
        }


        private double _fineRotation;
        public double FineRotation
        {
            get { return this._fineRotation; }
            set { this.RaiseAndSetIfChanged(ref this._fineRotation, value); }
        }

        public ReactiveCommand CommandParse { get; private set; }

        public ModelMain()
        {
            this.FileList = new ObservableCollection<string>();       
            this.TokenList = new ObservableCollection<ModelToken>();
            this.CommandParse = ReactiveCommand.Create(ExecuteParse);
            this.InitialRotation = 180;
            this.InitialThreshold = 100;

            this.FineRotation = 0;
            LoadPDF(@"D:\Projects\TesseractWrap\TesseractData\todo\Tabellen1941deels.pdf");
            TokenList.Add(new ModelToken(@"D:\Projects\TesseractWrap\TesseractData\tokens\t_a_000.png"));
            ExecuteParse();
        }



        private void ExecuteParse()
        {
            if (FileSelected == null)
            {
                return;
            }
            bitmapSource = new Bitmap(FileSelected);
            if (bitmapSource == null)
            {
                return;
            }

            //Convert to gray
            bitmapSource = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmapSource);
            //Rotate 
            bitmapSource = new RotateBilinear(InitialRotation, true).Apply(bitmapSource);
            //Threshold
            bitmapSource = new Threshold(InitialThreshold).Apply(bitmapSource);
            //bitmapSource = AForge.Imaging.Image.Clone(bitmapSource, System.Drawing.Imaging.PixelFormat.Format24bppRgb);      

            // apply the filter
            ImageSourceSource = RenderBitmap(bitmapSource);
            HoughLineTransformation lineTransform = new HoughLineTransformation();
            // apply Hough line transofrm
            lineTransform.ProcessImage(bitmapSource);
            Bitmap houghLineImage = lineTransform.ToBitmap();
            ImageSourceTarget = RenderBitmap(houghLineImage);
            // get lines using relative intensity
            //HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.5);

            //foreach (HoughLine line in lines)
            //{
            //   
            //}
        }

        private ImageSource RenderBitmap(Bitmap bitmapSource)
        {
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmapSource);
            renderer.Render();
            return renderer.RenderedImage;
        }

        private Bitmap ApplyConvolution(Bitmap source, Bitmap kernel)
        {
            Bitmap target = null;
            int[,] kernelint = {
            { -2, -1,  0 },
            { -1,  1,  1 },
            {  0,  1,  2 } };
            // create filter
            Convolution filter = new Convolution(kernelint);
            // apply the filter
            filter.ApplyInPlace(target);
            return target;
        }


        public void LoadPDF(string filePath)
        {

            FileList.Clear();
            foreach (var item in ToolsPDF.Convert(filePath))
            {
                FileList.Add(item);
            }
            if (0 < FileList.Count)
            {
                FileSelected = FileList[0];
            }
        }
    }
}
