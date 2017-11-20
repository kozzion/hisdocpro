using AForge.Imaging;
using AForge.Imaging.Filters;
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

        public IList<string> KernelList { get; private set; }
        private string kernelSelected;
        public string KernelSelected
        {
            get { return this.kernelSelected; }
            set { this.RaiseAndSetIfChanged(ref this.kernelSelected, value); }
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

        public ReactiveCommand CommandParse { get; private set; }

        public ModelMain()
        {
            this.FileList = new ObservableCollection<string>();        
            this.CommandParse = ReactiveCommand.Create(ExecuteParse);
        }

        private void ExecuteParse()
        {
            if (bitmapSource == null)
            {
                return;
            }
            
            HoughLineTransformation lineTransform = new HoughLineTransformation();
            // apply Hough line transofrm
            lineTransform.ProcessImage(bitmapSource);
            Bitmap houghLineImage = lineTransform.ToBitmap();
            // get lines using relative intensity
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.5);

            foreach (HoughLine line in lines)
            {
               
            }
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
