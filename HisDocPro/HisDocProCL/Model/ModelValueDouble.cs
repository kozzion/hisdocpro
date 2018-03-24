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

        public ReactiveCommand CommandSubtractOne { get; }
        public ReactiveCommand CommandSubtractDecimal { get; }
        public ReactiveCommand CommandAddDecimal { get; }
        public ReactiveCommand CommandAddOne { get; }

        private Action _action;
        private double _value;


        public double Value
        {
            get { return this._value; }
            set { this.RaiseAndSetIfChanged(ref this._value, value);
                _action.Invoke();
            }
        }

        public ModelValueDouble(Action action, double value)
        {
            _action = action;
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
