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
using HisDocProCl.Tools;

namespace HisDocProUI.Model
{
    public class ModelMain : ReactiveObject, IModelApplication
    {
        //TODO token changed - > save!!!

        public IList<string> PageFileList { get; private set; }
        private string _pageFileSelected;
        public string PageFileSelected
        {
            get { return this._pageFileSelected; }
            set { this.RaiseAndSetIfChanged(ref this._pageFileSelected, value);
                LoadLayoutList();
                EventLayoutChanged();
            }
        }

        public IList<string> LayoutFileList { get; private set; }
        private string _layoutFileSelected;
        public string LayoutFileSelected
        {
            get { return this._layoutFileSelected; }
            set { this.RaiseAndSetIfChanged(ref this._layoutFileSelected, value); }
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
        private string _pageDir = @"pages\";
        private string _tokenDir = @"tokens\";

        public ModelMain()
        {     
            // string files = Directory.GetFiles(pageDir);
            this.PageFileList = new ObservableCollection<string>();
            this.LayoutFileList = new ObservableCollection<string>();
            this.TokenList = new ObservableCollection<ModelToken>();
            this.PageLayout = new ModelPageLayout(this, null, null);

            this.CommandRenderLayout = ReactiveCommand.Create(EventLayoutChanged);
            this.CommandParse = ReactiveCommand.Create(ExecuteParseAll);

            //LoadPDF(@"D:\Projects\hisdocpro\data\PrijscourantPagina.pdf");
            //LoadPDF(@"D:\Projects\hisdocpro\data\Tabellen1941deels.pdf"
            //AddPNG(@"D:\Projects\hisdocpro\plot\temp_1.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_2.png");
            //LoadPNG(@"D:\Projects\hisdocpro\plot\temp_3.png");
            LoadTokens();
            AddPageWatcher();
            LoadPages(null, null);
        }

        private void LoadLayoutList()
        {
            if (PageFileSelected != null)
            {

                string layoutDir = PageFileSelected.Replace(".png", "\\");
                if (!Directory.Exists(layoutDir))
                {
                    Directory.CreateDirectory(layoutDir);
                }
                string[] files = Directory.GetFiles(layoutDir);
                LayoutFileList.Clear();
                foreach (var file in files)
                {
                    if (file.Split('.').Last().Equals("json"))
                    {
                        LayoutFileList.Add(file);
                    }
                }
                if (LayoutFileList.Count == 0)
                {
                    string filePath = layoutDir + "\\layout_0.json";
                    LayoutFileList.Add(filePath);
                }
                string layoutFileSelected = LayoutFileList[0];
                this.PageLayout = new ModelPageLayout(this, layoutFileSelected, this.PageLayout);
            }
        }

        private void AddPageWatcher()
        {
            if (!Directory.Exists(_pageDir))
            {
                Directory.CreateDirectory(_pageDir);
            }

            _watcher = new FileSystemWatcher();
            _watcher.Path = _pageDir;
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
                string[] files = Directory.GetFiles(_pageDir);
                foreach (var file in files)
                {
                    if (file.Split('.').Last().Equals("png"))
                    {
                        PageFileList.Add(file);
                    }
                }
                if (PageFileList.Count != 0)
                {
                    PageFileSelected = PageFileList[0];
                }
            });
        }

        private void LoadTokens()
        {
            if (!Directory.Exists(_tokenDir))
            {
                Directory.CreateDirectory(_tokenDir);
            }

            string[] files = Directory.GetFiles(_tokenDir);
            foreach (var imageFilePath in files)
            {
                if (imageFilePath.Substring(imageFilePath.Length - 4).Equals(".png")){ 
                    string jsonFilePath = imageFilePath.Replace("png","json");
                    TokenList.Add(new ModelToken(this, imageFilePath, jsonFilePath));
                }
            }
        }


        public void EventTokenChanged()
        {
            TokenSelected.Save();
        }

        public void EventLayoutChanged()
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
                bitmapSource = ToolsConvolution.ConvertTo24(bitmapSource);
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
                bitmapSource = ToolsConvolution.ConvertTo24(bitmapSource);
            }


            //Rotate 
            var bitmapRot = new RotateBilinear(PageLayout.Rotation.Value, true).Apply(bitmapSource);
            var linesH = ToolsFindLine.FindLinesH(bitmapRot.Width, bitmapRot.Height, PageLayout.LineSize.Value, PageLayout.LineOffset.Value, PageLayout.LineCount.Value);
            var linesV = ToolsFindLine.FindLinesV(bitmapRot.Width, bitmapRot.Height, PageLayout.ColSize.Value, PageLayout.ColOffset.Value, PageLayout.ColCount.Value);
            List<RenderLine> rl_list = new List<RenderLine>();
            rl_list.AddRange(linesH);
            rl_list.AddRange(linesV);

            //Convert to gray
            Bitmap bitmapTemp = new Grayscale(1, 0, 0).Apply(bitmapRot);
            bitmapTemp = new Erosion().Apply(bitmapTemp);
            bitmapTemp = new Threshold(160).Apply(bitmapTemp);
            bitmapTemp = new Invert().Apply(bitmapTemp);


            //Tuple<List<ConnectedComponent>, double, double>[] componentSectionList = new Tuple<List<ConnectedComponent>, double, double>[1];
            //Bitmap[] BitmapArray = new Bitmap[1];
            Tuple<List<ConnectedComponent>, double, double>[,] componentTable = new Tuple<List<ConnectedComponent>, double, double>[PageLayout.LineCount.Value, PageLayout.ColCount.Value];

            for (int lineIndex = 0; lineIndex < PageLayout.LineCount.Value; lineIndex++)
            {
                for (int colIndex = 0; colIndex < PageLayout.ColCount.Value; colIndex++)
                {
                    //TODO add margin?
                    int x = (int)((PageLayout.ColSize.Value * colIndex) + PageLayout.ColOffset.Value);// - PageLayout.Margin.Value;
                    int y = (int)((PageLayout.LineSize.Value * lineIndex) + PageLayout.LineOffset.Value) - PageLayout.Margin.Value;
                    int w = (int)PageLayout.ColSize.Value;// + (PageLayout.Margin.Value * 2);
                    int h = (int)PageLayout.LineSize.Value + (PageLayout.Margin.Value * 2);
                    Bitmap bitmap = new Crop(new Rectangle(x, y, w, h)).Apply(bitmapTemp);
                    List<ConnectedComponent> lineComponentList = new List<ConnectedComponent>();
                    lineComponentList = GetTokens(bitmap);
                    componentTable[lineIndex,colIndex] = new Tuple<List<ConnectedComponent>, double, double>(lineComponentList, x, y);
                }
            }

            //ImageParse = RenderBitmap(bitmapRot, rl_list, componentTable);
            ImageLayout = RenderBitmap(bitmapRot, rl_list, componentTable);
            //Create output
            string[,] string_table = new string[PageLayout.LineCount.Value, PageLayout.ColCount.Value];
            for (int i = 0; i < PageLayout.LineCount.Value; i++)
            {
                for (int j = 0; j < PageLayout.ColCount.Value; j++)
                {
                    string_table[i,j] = TokensToString(componentTable[i,j].Item1, 3, " ");
                }
            }
            string csvFileSelected = LayoutFileSelected.Replace(".png", ".csv");
            ToolsIOCSV.WriteCSVFile(csvFileSelected, string_table, KozzionCore.IO.CSV.Delimiter.SemiColon);           
        }


        private double ComputeOverlap(ConnectedComponent a, ConnectedComponent b)
        {
            Rectangle inter = Rectangle.Intersect(a.GetBound(), b.GetBound());
            double areaInter = inter.Height * inter.Width;
            double overlap = Math.Max(areaInter / (a.Height * a.Width), areaInter / (b.Height * b.Width));
            return overlap;
        }

        private string TokensToString(List<ConnectedComponent > component, double splitThreshold, string splitChar)
        {
            if (component.Count == 0)
            {
                return "";
            }

            component.Sort((x, y) => (x.MeanX).CompareTo(y.MeanX));
            
            //Remove overlapping kokens here
            int tokenIndex = 1;
            while (tokenIndex < component.Count)
            {
                ConnectedComponent last = component[tokenIndex - 1];  
                ConnectedComponent current = component[tokenIndex];

                Rectangle inter = Rectangle.Intersect(last.GetBound(), current.GetBound());
                double overlap = ComputeOverlap(component[tokenIndex - 1], component[tokenIndex]);
                if (PageLayout.Overlap.Value < overlap) {
                    if (last.CorrMax * last.Token.Weigth.Value < current.CorrMax * current.Token.Weigth.Value) {
                        component.RemoveAt(tokenIndex - 1);
                    } else {
                        component.RemoveAt(tokenIndex);
                    }

                } else {
                    tokenIndex++;
                }
            }



            string line = "";
            for (int i = 0; i < component.Count -1; i++)
            {
                line = line + component[i].Token.Label;
                if (splitThreshold < (component[i + 1].MeanX - component[i + 1].Width) - component[i].MeanX)
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

            double[,] imageDouble = ToolsConvolution.BitMapToDoubleArray(bitmapTemp, 0.5);

            Parallel.For(0, TokenList.Count, tokenIndex =>
            {
                ModelToken token = TokenList[tokenIndex];
                var corr = ToolsConvolution.CorrelationFFT(imageDouble, token.GetKernel());
                component_list_array[tokenIndex] = ToolsConnectedComponent.GetConnectedComponents(corr, token.Threshold.Value, token);
            });

            List<ConnectedComponent> component_list = new List<ConnectedComponent>();
            foreach (List<ConnectedComponent> list in component_list_array)
            {
                component_list.AddRange(list);
            }

            return component_list;
        }
              

  

        private ImageSource RenderBitmap(Bitmap bitmap)
        {
            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
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
            foreach (var rl in rl_list)
            {
                DrawLine(bitmap, rl);
            }

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

            ModelRendererBitmapSource renderer = new ModelRendererBitmapSource(bitmap);
            renderer.Render();
            return renderer.RenderedImage;
        }

        private void DrawLine(Bitmap bitmap, RenderLine rl)
        {
            Graphics g = Graphics.FromImage(bitmap);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1);
            g.DrawLine(pen, (float)rl.X0, (float)rl.Y0, (float)rl.X1, (float)rl.Y1);
        }

        private void DrawComponent(Bitmap bitmap, ConnectedComponent comp, double offsetX, double offSetY)
        {
            int left = (int)comp.MeanX + (int)offsetX;
            int top = (int)comp.MeanY + (int)offSetY;

            Rectangle rect = new Rectangle(left, top, comp.Width, comp.Height);            

            Graphics g = Graphics.FromImage(bitmap);

            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            RectangleF rect_text = new RectangleF(left -2, top - 2, comp.Width, comp.Height);
            g.DrawString(comp.Token.Label, new Font("Tahoma", 18), System.Drawing.Brushes.Red, rect_text);
            g.DrawRectangle(redPen, rect);

            g.Flush();
        }

        private void DrawComponent(Bitmap bitmap, ConnectedComponent comp)
        {
            int left = (int)comp.MeanX;
            int top = (int)comp.MeanY;

            Rectangle rect = new Rectangle(left, top, comp.Width, comp.Height);
            RectangleF rectf = new RectangleF(left, top, comp.Width, comp.Height);

            Graphics g = Graphics.FromImage(bitmap);

            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
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
