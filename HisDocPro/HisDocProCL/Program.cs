using AForge.Imaging.Filters;
using HisDocProUI.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProCL
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmap_0 = new Bitmap(@"D:\Projects\hisdocpro\HisDocPro\HisDocProUI\bin\Debug\tokens\t_0_c000.png");
            //Bitmap bitmap_0 = new Bitmap(PageFileSelected);
            Bitmap bitmapTemp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap_0);
            bitmapTemp = new Threshold(100).Apply(bitmapTemp);
            bitmapTemp = new Invert().Apply(bitmapTemp);

            double[,] image = ToolsConvolution.BitMapToDoubleArray(bitmapTemp, 0.5);
            double[,] kernel = ToolsConvolution.BitMapToDoubleArray(bitmapTemp, 0.5);
            Print(image);
            Print(kernel);
            //double[,] image = new double[6, 6];
            //image[2, 1] = 1;
            //image[2, 2] = 1;
            //image[2, 3] = 1;
            //image[2, 4] = 1;
            //double[,] kernel = new double[3, 3];
            //kernel[0, 0] = 1;
            //kernel[0, 1] = 1;
            //kernel[0, 2] = 1;
            //kernel[2, 1] = 1;
            //Print(image);
            //Print(kernel);

            //double[,] result = ToolsConvolution.CorrelationFFT(image, kernel);

            //Print(result);
            //Console.ReadLine();
            double [,] result = ToolsConvolution.CorrelationFFT(image, kernel);
            Print(result);
            //List<string> page = ToolsPDF.Convert(@"D:\Projects\hisdocpro\page.pdf");
        }

        private static void Print(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(0); j++)
                {
                    Console.Write(array[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void Print(double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(0); j++)
                {
                    Console.Write(array[i, j].ToString("0.0") + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
