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

namespace HisDocProCL.Model
{
    public class ModelValueInt : ReactiveObject
    {
        public IModelApplication Application { get; }
        public ReactiveCommand CommandSubtractOne { get; }
        public ReactiveCommand CommandAddOne { get; }

        private int _value;
        public int Value
        {
            get { return this._value; }
            set { this.RaiseAndSetIfChanged(ref this._value, value);
                Application.RenderLayout();
            }
        }

        public ModelValueInt(IModelApplication application, int value)
        {
            Application = application;
            _value = value;
            this.CommandSubtractOne = ReactiveCommand.Create(ExecuteSubtractOne);
            this.CommandAddOne = ReactiveCommand.Create(ExecuteAddOne);
        }

        private void ExecuteAddOne()
        {
            Value = Value + 1;
        }

        public void ExecuteSubtractOne()
        {
            Value = Value - 1;
        }
    }
}
