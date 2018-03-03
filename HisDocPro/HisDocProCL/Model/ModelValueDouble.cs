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
    public class ModelValueDouble : ReactiveObject
    {
        public IModelApplication Application { get; }
        public ReactiveCommand CommandSubtractOne { get; }
        public ReactiveCommand CommandSubtractDecimal { get; }
        public ReactiveCommand CommandAddDecimal { get; }
        public ReactiveCommand CommandAddOne { get; }

        private double _value;
        public double Value
        {
            get { return this._value; }
            set { this.RaiseAndSetIfChanged(ref this._value, value);
                Application.RenderLayout();
            }
        }

        public ModelValueDouble(IModelApplication application, double value)
        {
            Application = application;
            _value = value;
            this.CommandSubtractOne = ReactiveCommand.Create(ExecuteSubtractOne);
            this.CommandSubtractDecimal = ReactiveCommand.Create(ExecuteSubtractDecimal);
            this.CommandAddDecimal = ReactiveCommand.Create(ExecuteAddDecimal);
            this.CommandAddOne = ReactiveCommand.Create(ExecuteAddOne);
        }

        private void ExecuteAddOne()
        {
            Value = Value + 1;
        }

        private void ExecuteAddDecimal()
        {
            Value = Value + 0.1;
        }

        private void ExecuteSubtractDecimal()
        {
            Value = Value - 0.1;
        }

        public void ExecuteSubtractOne()
        {
            Value = Value - 1;
        }
    }
}
