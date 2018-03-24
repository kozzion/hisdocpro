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
        private Action _action;
        public ReactiveCommand CommandSubtractOne { get; }
        public ReactiveCommand CommandAddOne { get; }

        private int _value;
        public int Value
        {
            get { return this._value; }
            set { this.RaiseAndSetIfChanged(ref this._value, value);
                _action.Invoke();
            }
        }

        public ModelValueInt(Action action, int value)
        {
            _action = action;
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
