using HisDocProUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProCL
{
    class Program
    {
        static void Main(string[] args)
        {
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

            List<string> page = ToolsPDF.Convert(@"D:\Projects\hisdocpro\page.pdf");
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
                    Console.Write(array[i, j].ToString("0.") + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
