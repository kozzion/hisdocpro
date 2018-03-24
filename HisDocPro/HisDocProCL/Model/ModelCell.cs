using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProCL.Model
{
    public class ModelCell : ReactiveObject
    {

        private string _labelFound;
        public string LabelFound
        {
            get { return this._labelFound; }
            set { this.RaiseAndSetIfChanged(ref this._labelFound, value);}
        }

        private string _labelAccepted;
        public string LabelAccepted
        {
            get { return this._labelAccepted; }
            set { this.RaiseAndSetIfChanged(ref this._labelAccepted, value); }
        }
    }
}
