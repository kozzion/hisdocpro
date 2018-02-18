using HisDocProUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProCL.Tools
{
    public class ToolsFindLine
    {
        public static List<RenderLine> FindLinesH(double width, double heigth, double lineSize, double lineOffset)
        {
            double y = lineOffset;
            List<RenderLine> renderLines = new List<RenderLine>();
            while (y < heigth)
            {
                renderLines.Add(new RenderLine(0, y, width, y));
                y += lineSize;
            }
            return renderLines;
        }

        public static List<RenderLine> FindLinesH(double[,] image, double a)
        {
            int[] histogram = CreateHistogramH(image);

            List<Tuple<int, int>> tuples = FindIntervals(histogram, a);
            List<RenderLine> renderLines = new List<RenderLine>();
            if (1 < tuples.Count)
            {
                double w = image.GetLength(0) - 1;

                //renderLines.Add(new RenderLine(0, tuples[0].Item1, w, tuples[0].Item1));
                for (int i = 0; i < tuples.Count; i++)
                {
                    double y = (tuples[i].Item1 + tuples[i].Item2) / 2.0;
                    renderLines.Add(new RenderLine(0, y, w, y));
                }
                //renderLines.Add(new RenderLine(0, tuples.Last().Item2, w, tuples.Last().Item2));
            }
            return renderLines;
        }

        public static List<RenderLine> FindLinesV(double[,] image, double a)
        {
            int[] histogram = CreateHistogramV(image);

            List<Tuple<int, int>> tuples = FindIntervals(histogram, a);
            List<RenderLine> renderLines = new List<RenderLine>();
            if (1 < tuples.Count)
            {
                double h = image.GetLength(1) - 1;

                renderLines.Add(new RenderLine(tuples[0].Item1, 0, tuples[0].Item1, h));
                for (int i = 0; i < tuples.Count - 1; i++)
                {
                    double x = (tuples[i].Item2 + tuples[i + 1].Item1) / 2.0;
                    renderLines.Add(new RenderLine(x, 0, x, h));
                }
                renderLines.Add(new RenderLine(tuples.Last().Item2, 0, tuples.Last().Item2, h));
            }
            return renderLines;
        }


        public static int[] CreateHistogramH(double[,] image)
        {
            int[] hist = new int[image.GetLength(1)];
            Parallel.For(0, image.GetLength(1),j => 
            {
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    hist[j] += (int)image[i, j];
                }               
            });
            return hist;
        }

        public static int[] CreateHistogramV(double[,] image)
        {
            int[] hist = new int[image.GetLength(0)];
            Parallel.For(0, image.GetLength(0), i =>
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    hist[i] += (int)image[i, j];
                }
            });
            return hist;
        }

        public static List<Tuple<int, int>> FindIntervals(int[] g, double a)
        {
            List<Tuple<int, int>> intervals = new List<Tuple<int, int>>();
            int i = 1;
            int n = g.Length - 1;
            int imin = 0;
            int imax = 0;
            while (i < n)
            {
                if ((g[i + 1] < a * g[imax]) && (g[imin] < a * g[imax]))
                {
                    int j = imax;
                    while (g[j - 1] >= a * g[imax])
                    {
                        j = j - 1;
                    }
                    intervals.Add(new Tuple<int, int>(j, i));
                }

                if ((g[i + 1] < g[imin]) && (g[i + 1] < a * g[imax]))
                {
                    i++;
                    imin = i;
                    imax = i;
                }
                else if(g[i + 1] > g[imax])
                {
                    i++;
                    imax = i;
                }
                else
                {
                    i++;
                }
            }
            return intervals;
        }

    }
}
