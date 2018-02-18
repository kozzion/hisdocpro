using HisDocProUI.Model;
using System;
using System.Collections.Generic;

namespace HisDocProUI.Tools
{
    public class ConnectedComponent
    {
        public List<Tuple<int, int>> Component { get; }
        public double Mean0 { get; private set; }
        public double Mean1 { get; private set; }
        public int Width { get { return (int)Token.Width; } }
        public int Height { get { return (int)Token.Height; } }
        public ModelToken Token { get; private set; }


        public ConnectedComponent(List<Tuple<int, int>> component, ModelToken token)
        {
            Component = component;
            double sum0 = 0;
            double sum1 = 0;
            foreach (var item in component)
            {
                sum0 += item.Item1;
                sum1 += item.Item2;
            }
            Mean0 = sum0 / component.Count;
            Mean1 = sum1 / component.Count;
            Token = token;
        }
    }
}