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
using System.Drawing.Imaging;
using AForge.Math;
using HisDocProCL.Tools;
using HisDocProUI.Renderer;
using System.Drawing.Drawing2D;
using System.IO;

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



        private ImageSource imageSourceTarget;
        public ImageSource ImageSourceTarget
        {
            get { return this.imageSourceTarget; }
            set { this.RaiseAndSetIfChanged(ref this.imageSourceTarget, value); }
        }

        private ImageSource imageSourceHelp;
        public ImageSource ImageSourceHelp
        {
            get { return this.imageSourceHelp; }
            set { this.RaiseAndSetIfChanged(ref this.imageSourceHelp, value); }
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


        private int _splitCountVertical;
        public int SplitCountVertical
        {
            get { return this._splitCountVertical; }
            set { this.RaiseAndSetIfChanged(ref this._splitCountVertical, value); }
        }

        private int _splitCountHotizontal;
        public int SplitCountHotizontal
        {
            get { return this._splitCountHotizontal; }
            set { this.RaiseAndSetIfChanged(ref this._splitCountHotizontal, value); }
        }

        private double _lineSize;
        public double LineSize
        {
            get { return this._lineSize; }
            set { this.RaiseAndSetIfChanged(ref this._lineSize, value); }
        }

        private double _lineOffset;
        public double LineOffset
        {
            get { return this._lineOffset; }
            set { this.RaiseAndSetIfChanged(ref this._lineOffset, value); }
        }

        public ReactiveCommand CommandParse { get; private set; }

        public ModelMain()
        {
            this.FileList = new ObservableCollection<string>();
            this.TokenList = new ObservableCollection<ModelToken>();
            this.CommandParse = ReactiveCommand.Create(ExecuteParse);
            this.InitialRotation = 180.2;
            this.InitialThreshold = 100;
            this.LineSize = 23.5;
            this.LineOffset = 283.2;


            this.SplitCountVertical = 0;
            LoadPDF(@"D:\Projects\hisdocpro\data\PrijscourantPagina.pdf");


            //LoadPDF(@"D:\Projects\hisdocpro\data\Tabellen1941deels.pdf"
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_1.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_2.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_3.png");
            LoadTokens(@"D:\Projects\hisdocpro\tokens\");
            ExecuteParse();
        }

        private void LoadTokens(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                TokenList.Add(new ModelToken(file));
            }
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
            if (bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                bitmapSource = ConvertTo24(bitmapSource);
            }

            //Convert to gray
            //Rotate 
            var bitmapRot = new RotateBilinear(InitialRotation, true).Apply(bitmapSource);

            var imageGray = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmapRot);
            //var imageGrayInvert = new Invert().Apply(imageGray);
            //var imageDoubleInvert = ToolsConvolution.BitMapToDoubleArray(imageGrayInvert);
            var linesH = ToolsFindLine.FindLinesH(bitmapRot.Width, bitmapRot.Height, LineSize, LineOffset);
            var imageBin = new Threshold(100).Apply(imageGray);
            var imageBinInvert = new Invert().Apply(imageBin);



            //var imageBinInvertFilteredBig = ToolsConnectedComponent.Filter(imageBinInvert, 100);
            //var imageBinInvertFilteredBigDouble = ToolsConvolution.BitMapToDoubleArray(imageGrayInvert);
            //var linesV = ToolsFindLine.FindLinesV(imageBinInvertFilteredBigDouble, AlfaV);

            //List<RenderLine> linesH = SplitLines(imageBinInvertFiltered, 0, 3, 0.1);
            //List<RenderLine> linesV = SplitLines(imageBinInvertFilteredBig, 90, 5, 0.1);

            List<RenderLine> linesList = new List<RenderLine>();
            linesList.AddRange(linesH);
            //linesList.AddRange(linesV);

            var image_bin = new Threshold(100).Apply(imageGray);
            image_bin = new Invert().Apply(image_bin);



            //Tuple<List<ConnectedComponent>, double, double>[] componentSectionList = new Tuple<List<ConnectedComponent>, double, double>[1];
            //Bitmap[] BitmapArray = new Bitmap[1];

            List<Tuple<List<ConnectedComponent>, double, double>> componentSectionList = new List<Tuple<List<ConnectedComponent>, double, double>>();
            Bitmap[] BitmapArray = new Bitmap[linesH.Count - 1];

             for (int i = 0; i < linesH.Count - 1; i++)
            //for (int i = 0; i < 1; i++)
            {
                int x = 0;
                int y = (int)linesH[i].Y0;
                int w = imageBinInvert.Width;
                int h = (int)(linesH[i + 1].Y0 - linesH[i].Y0);
                BitmapArray[i] = new Crop(new Rectangle(x, y, w, h)).Apply(imageBinInvert);
            //    List<ConnectedComponent> lineComponentList = GetTokens(BitmapArray[i]);
            //    componentSectionList[i] = new Tuple<List<ConnectedComponent>, double, double>(lineComponentList, x, y);
            };


            //List<ConnectedComponent> component_list = GetTokens(imageBinInvert);
            //List<ConnectedComponent> component_list = new List<ConnectedComponent>();
            ImageSourceTarget = RenderBitmap(bitmapRot, componentSectionList, linesList);

            //ImageSourceHelp = RenderBitmap(imageGrayInvert);

            //Create output
            List<string> lines = new List<string>();
            foreach (var item in componentSectionList)
            {
                lines.Add( TokensToLine(item.Item1, 3, " "));
            }

            File.WriteAllLines(@"D:\Projects\hisdocpro\plot\temp_1_out.txt", lines);
        }


        private string TokensToLine(List<ConnectedComponent > component, double splitThreshold, string splitChar)
        {
            component.Sort((x, y) => (x.Mean0 - x.Width).CompareTo(y.Mean0 - y.Width));
            //TODO Remove overlapping kokens here

            string line = "";
            for (int i = 0; i < component.Count -1; i++)
            {
                line = line + component[i].Token.Label;
                if (splitThreshold < (component[i + 1].Mean0 - component[i + 1].Width) - component[i].Mean0)
                {
                    line = line + splitChar;
                }
            }
            line = line + component.Last().Token.Label;
            return line;
        }



        private List<ConnectedComponent> GetTokens(Bitmap bitmapTemp)
        {
            List<ConnectedComponent>[] component_list_array = new List<ConnectedComponent>[TokenList.Count];

            double[,] imageDouble = ToolsConvolution.BitMapToDoubleArray(bitmapTemp);

            Parallel.For(0, TokenList.Count, tokenIndex =>
            {
                ModelToken token = TokenList[tokenIndex];
                Bitmap bitmapKernel = token.GetKernel(this.InitialThreshold);
                // create filter
                bitmapKernel = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmapKernel);
                bitmapKernel = new Threshold(100).Apply(bitmapKernel);
                bitmapKernel = new Invert().Apply(bitmapKernel);


                var corr = ToolsConvolution.CorrelationFFT(imageDouble, bitmapKernel);
                component_list_array[tokenIndex] = ToolsConnectedComponent.GetConnectedComponents(corr, 0.8, token);
            });

            List<ConnectedComponent> component_list = new List<ConnectedComponent>();
            foreach (List<ConnectedComponent> list in component_list_array)
            {
                component_list.AddRange(list);
            }

            return component_list;
        }


        private List<RenderLine> SplitLines(Bitmap imageBin, double thetaTarget, double thetaTolerance, double intensityThreshold)
        {

            HoughLineTransformation lineTransform = new HoughLineTransformation();
            lineTransform.ProcessImage(imageBin);
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(intensityThreshold);

            List<RenderLine> lines_list = new List<RenderLine>();
            int w2 = imageBin.Width / 2;
            int h2 = imageBin.Height / 2;

            foreach (HoughLine line in lines)
            {
                // get line's radius and theta values
                int r = line.Radius;
                double td = line.Theta;
            

                // check if line is in lower part of the image
                if (r < 0)
                {
                    td += 180;
                    r = -r;
                }

                // convert degrees to radians
                var t = (td / 180) * Math.PI;

                // get image centers (all coordinate are measured relative
                // to center)

                double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                if (line.Theta != 0)
                {
                    // none-vertical line
                    x0 = -w2; // most left point
                    x1 = w2;  // most right point

                    // calculate corresponding y values
                    y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                    y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                }
                else
                {
                    // vertical line
                    x0 = line.Radius;
                    x1 = line.Radius;

                    y0 = h2;
                    y1 = -h2;
                }

                if (Math.Abs(td - thetaTarget) < thetaTolerance)
                {
                    Console.WriteLine(td);


                    double rot = Math.PI;

                    x0 = x0 * Math.Cos(rot) - y0 * Math.Sin(rot);
                    y0 = x0 * Math.Sin(rot) + y0 * Math.Cos(rot);

                    x1 = x1 * Math.Cos(rot) - y1 * Math.Sin(rot);
                    y1 = x1 * Math.Sin(rot) + y1 * Math.Cos(rot);


                    x0 = w2 - x0;
                    y0 += h2;

                    x1 = w2 - x1;
                    y1 += h2;
                    Console.WriteLine("x0: " + x0);
                    Console.WriteLine("y0: " + y0);
                    Console.WriteLine("x1: " + x1);
                    Console.WriteLine("y1: " + y1);
                    if ((Math.Abs(y1 - y0) < 10) || (Math.Abs(x1 - x0) < 10))
                    {
                        lines_list.Add(new RenderLine(x0, y0, x1, y1));
                    }
                }

            }
            return lines_list;
        }

        private static Bitmap ConvertTo24(Bitmap bmpIn)
        {    
            Bitmap converted = new Bitmap(bmpIn.Width, bmpIn.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(converted))
            {
                // Prevent DPI conversion
                g.PageUnit = GraphicsUnit.Pixel;
                // Draw the image
                g.DrawImageUnscaled(bmpIn, 0, 0);
            }
            return converted;
        }

        private ImageSource RenderBitmap(Bitmap bitmapSource)
        {
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmapSource);
            renderer.Render();
            return renderer.RenderedImage;
        }

        private ImageSource RenderBitmap(Bitmap bitmap, List<ConnectedComponent> cc_list, List<RenderLine> rl_list)
        {
            //Create un
            foreach (var cc in cc_list)
            {
                DrawComponent(bitmap, cc);
            }
            foreach (var rl in rl_list)
            {
                DrawLine(bitmap, rl);
            }
  
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
            renderer.Render();
            return renderer.RenderedImage;
        }


        private ImageSource RenderBitmap(Bitmap bitmap, IList<Tuple<List<ConnectedComponent>, double, double>> cc_tuple_list, List<RenderLine> rl_list)
        {
            //Create un
            foreach (var cc_tuple in cc_tuple_list)
            {
                double offsetX = cc_tuple.Item2;
                double offsetY = cc_tuple.Item3;
                foreach (var cc in cc_tuple.Item1)
                {
                    DrawComponent(bitmap, cc, offsetX, offsetY);
                }
            }
            foreach (var rl in rl_list)
            {
                DrawLine(bitmap, rl);
            }

            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
            renderer.Render();
            return renderer.RenderedImage;
        }

        private void DrawLine(Bitmap bitmap, RenderLine rl)
        {
            int w2 = bitmap.Width / 2;
            int h2 = bitmap.Height / 2;

            Graphics g = Graphics.FromImage(bitmap);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Blue, 3);
            g.DrawLine(pen, (float)rl.X0, (float)rl.Y0, (float)rl.X1, (float)rl.Y1);
        }

        private void DrawComponent(Bitmap bitmap, ConnectedComponent comp, double offsetX, double offSetY)
        {
            int left = (int)comp.Mean0 + (int)offsetX;
            int top = (int)comp.Mean1 + (int)offSetY;

            Rectangle rect = new Rectangle(left, top, comp.Width, comp.Height);
            

            Graphics g = Graphics.FromImage(bitmap);

            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            RectangleF rect_text = new RectangleF(left -2, top - 2, comp.Width, comp.Height);
            g.DrawString(comp.Token.Label, new Font("Tahoma", 6), System.Drawing.Brushes.Red, rect_text);
            g.DrawRectangle(redPen, rect);

            g.Flush();
        }

        private void DrawComponent(Bitmap bitmap, ConnectedComponent comp)
        {
            int left = (int)comp.Mean0;
            int top = (int)comp.Mean1;

            Rectangle rect = new Rectangle(left, top, comp.Width, comp.Height);
            RectangleF rectf = new RectangleF(left, top, comp.Width, comp.Height);

            Graphics g = Graphics.FromImage(bitmap);

            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(comp.Token.Label, new Font("Tahoma", 8), System.Drawing.Brushes.Red, rectf);
            g.DrawRectangle(redPen, rect);

            g.Flush();
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

        public void LoadPNG(string filePath)
        {

            FileList.Clear();
         
                FileList.Add(filePath);
            if (0 < FileList.Count)
            {
                FileSelected = FileList[0];
            }
        }
    }
}
