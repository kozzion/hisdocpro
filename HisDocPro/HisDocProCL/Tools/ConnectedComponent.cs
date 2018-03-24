using HisDocProCL.Model;
using HisDocProUI.Model;
using System;
using System.Collections.Generic;

namespace HisDocProCl.Tools
{
    public class ConnectedComponent
    {
        public List<Tuple<int, int>> Component { get; }
        public double MeanX { get; private set; }
        public double MeanY { get; private set; }
        public double CorrMax { get; private set; }
        public int Width { get { return (int)Token.Width; } }
        public int Height { get { return (int)Token.Height; } }


        public ModelToken Token { get; private set; }


        public ConnectedComponent(List<Tuple<int, int>> component, ModelToken token, double [,] corr)
        {
            Component = component;
            double sumx = 0;
            double sumy = 0;
            foreach (var item in component)
            {
                sumx += item.Item1;
                sumy += item.Item2;
                CorrMax = Math.Max(CorrMax, corr[item.Item1, item.Item2]);
            }
            MeanX = sumx / component.Count;
            MeanY = sumy / component.Count;
            Token = token;
        }
    }
}