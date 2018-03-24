using HisDocProUI.Model;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProCL.Model
{

    public class ModelPageLayout : ReactiveObject
    {
        private string _filePath;

        private ModelValueDouble _rotation;
        public ModelValueDouble Rotation
        {
            get { return this._rotation; }
            set { this.RaiseAndSetIfChanged(ref this._rotation, value); }
        }

        private ModelValueInt _threshold;
        public ModelValueInt Threshold
        {
            get { return this._threshold; }
            set { this.RaiseAndSetIfChanged(ref this._threshold, value); }
        }

        private ModelValueDouble _lineSize;
        public ModelValueDouble LineSize
        {
            get { return this._lineSize; }
            set { this.RaiseAndSetIfChanged(ref this._lineSize, value); }
        }

        private ModelValueDouble _lineOffset;
        public ModelValueDouble LineOffset
        {
            get { return this._lineOffset; }
            set { this.RaiseAndSetIfChanged(ref this._lineOffset, value); }
        }

        private ModelValueInt _lineCount;
        public ModelValueInt LineCount
        {
            get { return this._lineCount; }
            set { this.RaiseAndSetIfChanged(ref this._lineCount, value); }
        }

        private ModelValueDouble _colSize;
        public ModelValueDouble ColSize
        {
            get { return this._colSize; }
            set { this.RaiseAndSetIfChanged(ref this._colSize, value); }
        }

        private ModelValueDouble _colOffset;
        public ModelValueDouble ColOffset
        {
            get { return this._colOffset; }
            set { this.RaiseAndSetIfChanged(ref this._colOffset, value); }
        }
        private ModelValueInt _colCount;
        public ModelValueInt ColCount
        {
            get { return this._colCount; }
            set { this.RaiseAndSetIfChanged(ref this._colCount, value); }
        }

        private ModelValueInt _margin;
        public ModelValueInt Margin
        {
            get { return this._margin; }
            set { this.RaiseAndSetIfChanged(ref this._margin, value); }
        }

        private ModelValueDouble _overlap;
        public ModelValueDouble Overlap
        {
            get { return this._overlap; }
            set { this.RaiseAndSetIfChanged(ref this._overlap, value); }
        }


        private IModelApplication _application;
        public ModelPageLayout(IModelApplication application, string filePath, ModelPageLayout other)
        {
            _application = application;
            _filePath = filePath;
            this._rotation = new ModelValueDouble(application.EventLayoutChanged, 0);
            this._threshold = new ModelValueInt(application.EventLayoutChanged, 0);

            this._lineSize = new ModelValueDouble(application.EventLayoutChanged, 0);
            this._lineOffset = new ModelValueDouble(application.EventLayoutChanged, 0);
            this._lineCount = new ModelValueInt(application.EventLayoutChanged, 0);

            this._colSize = new ModelValueDouble(application.EventLayoutChanged, 0);
            this._colOffset = new ModelValueDouble(application.EventLayoutChanged, 0);
            this._colCount = new ModelValueInt(application.EventLayoutChanged, 0);


            this._margin = new ModelValueInt(application.EventLayoutChanged, 0);
            this._overlap = new ModelValueDouble(application.EventLayoutChanged, 0);

            if (File.Exists(_filePath))
            {
                Load();
            }
            else
            {
                if (other == null)
                {
                    this._rotation.Value = 180;
                    this._threshold.Value = 100;

                    this._lineSize.Value = 20;
                    this._lineOffset.Value = 100;
                    this._lineCount.Value = 0;

                    this._colSize.Value = 20;
                    this._colOffset.Value = 100;
                    this._colCount.Value = 0;

                    this._margin.Value = 0;
                    this._overlap.Value = 0;
                }
                else {
                    SetValue(other.GetValue());
                }
                Save();
            }
        }

     
        public void Load()
        {
            string json = File.ReadAllText(_filePath);
            PageLayoutV001 layout = JsonConvert.DeserializeObject<PageLayoutV001>(json);
            SetValue(layout);
        }     

        public void Save()
        {
            if (_filePath != null)
            {
                string json = JsonConvert.SerializeObject(GetValue(), Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
        }

        private void SetValue(PageLayoutV001 layout)
        {
            this._rotation = new ModelValueDouble(_application.EventLayoutChanged, layout.Rotation);
            this._threshold = new ModelValueInt(_application.EventLayoutChanged, layout.Threshold);

            this._lineSize = new ModelValueDouble(_application.EventLayoutChanged, layout.LineSize);
            this._lineOffset = new ModelValueDouble(_application.EventLayoutChanged, layout.LineOffset);
            this._lineCount = new ModelValueInt(_application.EventLayoutChanged, layout.LineCount);

            this._colSize = new ModelValueDouble(_application.EventLayoutChanged, layout.ColSize);
            this._colOffset = new ModelValueDouble(_application.EventLayoutChanged, layout.ColOffset);
            this._colCount = new ModelValueInt(_application.EventLayoutChanged, layout.ColCount);

            this._margin = new ModelValueInt(_application.EventLayoutChanged, layout.Margin);
            this._overlap = new ModelValueDouble(_application.EventLayoutChanged, layout.Overlap);
        }

        private PageLayoutV001 GetValue()
        {
            PageLayoutV001 layout = new PageLayoutV001();
            layout.Rotation = Rotation.Value;
            layout.Threshold = Threshold.Value;

            layout.LineSize = LineSize.Value;
            layout.LineOffset = LineOffset.Value;
            layout.LineCount = LineCount.Value;

            layout.ColSize = ColSize.Value;
            layout.ColOffset = ColOffset.Value;
            layout.ColCount = ColCount.Value;

            layout.Margin = Margin.Value;
            layout.Overlap = Overlap.Value;
            return layout;
        }

        public class PageLayoutV001
        {
            public double Rotation;
            public int Threshold;

            public double LineSize;
            public double LineOffset;
            public int LineCount;

            public double ColSize;
            public double ColOffset;
            public int ColCount;

            public int Margin;
            public double Overlap;


        }
    }
}
