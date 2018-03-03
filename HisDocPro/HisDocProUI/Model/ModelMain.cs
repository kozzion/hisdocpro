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
using HisDocProCL.Model;
using System.Threading;

namespace HisDocProUI.Model
{
    public class ModelMain : ReactiveObject, IModelApplication
    {
        public IList<string> PageFileList { get; private set; }
        private string _pageFileSelected;
        public string PageFileSelected
        {
            get { return this._pageFileSelected; }
            set { this.RaiseAndSetIfChanged(ref this._pageFileSelected, value);
                LoadLayout();
                ExecuteRenderLayout();
            }
        }



        public IList<ModelToken> TokenList { get; private set; }
        private ModelToken _tokenlSelected;
        public ModelToken TokenSelected
        {
            get { return this._tokenlSelected; }
            set { this.RaiseAndSetIfChanged(ref this._tokenlSelected, value); }
        }

        private ModelPageLayout _pageLayout;
        public ModelPageLayout PageLayout
        {
            get { return this._pageLayout; }
            set { this.RaiseAndSetIfChanged(ref this._pageLayout, value); }
        }

        private ImageSource _imageLayout;
        public ImageSource ImageLayout
        {
            get { return this._imageLayout; }
            set { this.RaiseAndSetIfChanged(ref this._imageLayout, value); }
        }

        private ImageSource _imageParse;
        public ImageSource ImageParse
        {
            get { return this._imageParse; }
            set { this.RaiseAndSetIfChanged(ref this._imageParse, value); }
        }


        public ReactiveCommand CommandRenderLayout { get; private set; }
        public ReactiveCommand CommandParse { get; private set; }



        private FileSystemWatcher _watcher;
        private Bitmap bitmapSource;
        private string pageDir = @"D:\Projects\hisdocpro\pages\";
        private string tokenDir = @"D:\Projects\hisdocpro\tokens\";

        public ModelMain()
        {     
            // string files = Directory.GetFiles(pageDir);
            this.PageFileList = new ObservableCollection<string>();
            this.TokenList = new ObservableCollection<ModelToken>();
            this.PageLayout = new ModelPageLayout(this, null, null);

            this.CommandRenderLayout = ReactiveCommand.Create(ExecuteRenderLayout);
            this.CommandParse = ReactiveCommand.Create(ExecuteParseAll);

            //LoadPDF(@"D:\Projects\hisdocpro\data\PrijscourantPagina.pdf");
            //LoadPDF(@"D:\Projects\hisdocpro\data\Tabellen1941deels.pdf"
            //AddPNG(@"D:\Projects\hisdocpro\plot\temp_1.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_2.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_3.png");
            LoadTokens(tokenDir);
            AddPageWatcher();
            LoadPages(null, null);
        }

        private void LoadLayout()
        {
            if (PageFileSelected != null)
            {
                string layoutFileSelected = PageFileSelected.Replace(".png", ".json");
                this.PageLayout = new ModelPageLayout(this, layoutFileSelected, this.PageLayout);
            }
        }

        private void AddPageWatcher()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = pageDir;
            //_watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Filter = "*.png";
            _watcher.Created += new FileSystemEventHandler(LoadPages);
            _watcher.Changed += new FileSystemEventHandler(LoadPages);
            _watcher.Deleted += new FileSystemEventHandler(LoadPages);
            _watcher.EnableRaisingEvents = true;
        }

        private void LoadPages(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                PageFileList.Clear();
                string[] files = Directory.GetFiles(pageDir);
                foreach (var file in files)
                {
                    if (file.Split('.').Last().Equals("png"))
                    {
                        PageFileList.Add(file);
                    }
                }
            });
        }

        private void LoadTokens(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                TokenList.Add(new ModelToken(file));
            }
        }


        public void RenderLayout()
        {
            ExecuteRenderLayout();
        }

        private void ExecuteRenderLayout()
        {
            if (PageFileSelected == null)
            {
                return;
            }
            bitmapSource = new Bitmap(PageFileSelected);
            if (bitmapSource == null)
            {
                return;
            }

            if ((bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) || 
                (bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                bitmapSource = ConvertTo24(bitmapSource);
            }
            PageLayout.Save();

            //Rotate 
            var bitmapRot = new RotateBilinear(PageLayout.Rotation.Value, true).Apply(bitmapSource);
            var linesH = ToolsFindLine.FindLinesH(bitmapRot.Width, bitmapRot.Height, PageLayout.LineSize.Value, PageLayout.LineOffset.Value, PageLayout.LineCount.Value);
            var linesV = ToolsFindLine.FindLinesV(bitmapRot.Width, bitmapRot.Height, PageLayout.ColSize.Value, PageLayout.ColOffset.Value, PageLayout.ColCount.Value);
            List<RenderLine> rl_list = new List<RenderLine>();
            rl_list.AddRange(linesH);
            rl_list.AddRange(linesV);


            //var linesV = ToolsFindLine.FindLinesV(bitmapRot.Width, bitmapRot.Height, PageLayout.LineSize.Value, PageLayout.LineOffset.Value);
            ImageLayout = RenderBitmap(bitmapRot, rl_list);
        }

        private void ExecuteParseAll()
        {
            if (PageFileSelected == null)
            {
                return;
            }
            bitmapSource = new Bitmap(PageFileSelected);
            if (bitmapSource == null)
            {
                return;
            }
            if ((bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) ||
                  (bitmapSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                bitmapSource = ConvertTo24(bitmapSource);
            }


            //Rotate 
            var bitmapRot = new RotateBilinear(PageLayout.Rotation.Value, true).Apply(bitmapSource);
            var linesH = ToolsFindLine.FindLinesH(bitmapRot.Width, bitmapRot.Height, PageLayout.LineSize.Value, PageLayout.LineOffset.Value, PageLayout.LineCount.Value);
            var linesV = ToolsFindLine.FindLinesV(bitmapRot.Width, bitmapRot.Height, PageLayout.ColSize.Value, PageLayout.ColOffset.Value, PageLayout.ColCount.Value);
            List<RenderLine> rl_list = new List<RenderLine>();
            rl_list.AddRange(linesH);
            rl_list.AddRange(linesV);

            //Convert to gray
            var imageGray = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmapRot);
            //var imageGrayInvert = new Invert().Apply(imageGray);
            //var imageDoubleInvert = ToolsConvolution.BitMapToDoubleArray(imageGrayInvert);
            var imageBin = new Threshold(100).Apply(imageGray);
            var imageBinInvert = new Invert().Apply(imageBin);


            //Tuple<List<ConnectedComponent>, double, double>[] componentSectionList = new Tuple<List<ConnectedComponent>, double, double>[1];
            //Bitmap[] BitmapArray = new Bitmap[1];
            Tuple<List<ConnectedComponent>, double, double>[,] componentTable = new Tuple<List<ConnectedComponent>, double, double>[PageLayout.LineCount.Value, PageLayout.ColCount.Value];

            for (int i = 0; i < PageLayout.LineCount.Value; i++)
            {
                for (int j = 0; j < PageLayout.ColCount.Value; j++)
                {
                    //TODO add margin?
                    int x = (int)((PageLayout.ColSize.Value * j) + PageLayout.ColOffset.Value);
                    int y = (int)((PageLayout.LineSize.Value * j) + PageLayout.LineOffset.Value);
                    int w = (int)PageLayout.ColSize.Value;
                    int h = (int)PageLayout.LineSize.Value;
                    Bitmap bitmap = new Crop(new Rectangle(x, y, w, h)).Apply(imageBinInvert);
                    List<ConnectedComponent> lineComponentList = GetTokens(bitmap);
                    componentTable[i,j] = new Tuple<List<ConnectedComponent>, double, double>(lineComponentList, x, y);
                }
            }

            ImageParse = RenderBitmap(bitmapRot, rl_list, componentTable);

            //Create output
            string[,] string_table = new string[PageLayout.LineCount.Value, PageLayout.ColCount.Value];
            for (int i = 0; i < PageLayout.LineCount.Value; i++)
            {
                for (int j = 0; j < PageLayout.ColCount.Value; j++)
                {
                    string_table[i,j] = TokensToString(componentTable[i,j].Item1, 3, " ");
                }
            }
            string csvFileSelected = PageFileSelected.Replace(".png", ".csv");
            ToolsIOCSV.WriteCSVFile(csvFileSelected, string_table);           
        }


        private string TokensToString(List<ConnectedComponent > component, double splitThreshold, string splitChar)
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
                Bitmap bitmapKernel = token.GetKernel(this.PageLayout.Threshold.Value);
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


        private ImageSource RenderBitmap(Bitmap bitmap, List<RenderLine> rl_list)
        {
            foreach (var rl in rl_list)
            {
                DrawLine(bitmap, rl);
            }

            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
            renderer.Render();
            return renderer.RenderedImage;
        }

        private ImageSource RenderBitmap(Bitmap bitmap, List<RenderLine> rl_list, List<ConnectedComponent> cc_list)
        {
            foreach (var rl in rl_list)
            {
                DrawLine(bitmap, rl);
            }

            foreach (var cc in cc_list)
            {
                DrawComponent(bitmap, cc);
            }

            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
            renderer.Render();
            return renderer.RenderedImage;
        }



 
        private ImageSource RenderBitmap(Bitmap bitmap, List<RenderLine> rl_list, Tuple<List<ConnectedComponent>, double, double>[,] cc_tuple_list)
        {
            //Create un
            for (int i = 0; i < cc_tuple_list.GetLength(0); i++)
            {
                for (int j = 0; j < cc_tuple_list.GetLength(1); j++)
                {
                    double offsetX = cc_tuple_list[i,j].Item2;
                    double offsetY = cc_tuple_list[i, j].Item3;
                    foreach (var cc in cc_tuple_list[i, j].Item1)
                    {
                        DrawComponent(bitmap, cc, offsetX, offsetY);
                    }
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
            PageFileList.Clear();
            foreach (var item in ToolsPDF.Convert(filePath))
            {
                PageFileList.Add(item);
            }
            if (0 < PageFileList.Count)
            {
                PageFileSelected = PageFileList[0];
            }
        }

        public void AddPNG(string filePath)
        {

            PageFileList.Clear();
            PageFileList.Add(filePath);
            if (0 < PageFileList.Count)
            {
                PageFileSelected = PageFileList[0];
            }
        }


    }
}
