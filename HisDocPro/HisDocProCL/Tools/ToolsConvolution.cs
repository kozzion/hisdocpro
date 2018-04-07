using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProUI.Tools
{
    public class ToolsConvolution
    {


        //public static double[,] ConvolutionFFT(int[,] image, int[,] kernel)
        //{
        //    int maxSize = Math.Max(Math.Max(Math.Max(image.GetLength(0), image.GetLength(1)), kernel.GetLength(0)), kernel.GetLength(1));
        //    int size = 1;
        //    while (size < maxSize)
        //    { 
        //        size *= 2;
        //    }
        //    size *= 2;
        //    Console.WriteLine(size);

        //    //Create complext image
        //    Complex[,] image_complex = new Complex[size, size];    
        //    for (int i = 0; i < image.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < image.GetLength(1); j++)
        //        {
        //            image_complex[i,j] = new Complex(image[i,j], 0);
        //        }
        //    }
        //    //Create complext kernel
        //    Complex[,] kernel_complexl = new Complex[size, size];
        //    for (int i = 0; i < kernel.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < kernel.GetLength(1); j++)
        //        {
        //            kernel_complexl[i, j] = new Complex(kernel[i, j], 0);
        //        }
        //    }
        //    kernel_complexl[0, 0] = new Complex(1, 0);
        //    //Do work
        //    FourierTransform.FFT2(image_complex, FourierTransform.Direction.Forward);
        //    FourierTransform.FFT2(kernel_complexl, FourierTransform.Direction.Forward);
        //    for (int i = 0; i < size; i++)
        //    {
        //        for (int j = 0; j < size; j++)
        //        {
        //            image_complex[i, j] = image_complex[i, j] * new Complex(kernel_complexl[i, j].Re, -kernel_complexl[i, j].Im);
        //        }
        //    }
        //    FourierTransform.FFT2(image_complex, FourierTransform.Direction.Backward);


        //    //Do get result
        //    double normalization = size * size;
        //    double[,] result = new double[image.GetLength(0), image.GetLength(1)];
        //    for (int i = 0; i < result.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < result.GetLength(0); j++)
        //        {
        //            result[i, j] = image_complex[i, j].Re * normalization;
        //        }
        //    }
        //    return result;
        //}


        public static double[,] CorrelationFFT(double[,] image, double[,] kernel)
        {
            double image_min = double.MaxValue;
            double image_max = double.MinValue;

            double kernel_min = double.MaxValue;
            double kernel_max = double.MinValue;
        
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image_min = Math.Min(image_min, image[i, j]);
                    image_max = Math.Max(image_max, image[i, j]);
                }
            }

            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    kernel_min = Math.Min(kernel_min, kernel[i, j]);
                    kernel_max = Math.Max(kernel_max, kernel[i, j]);
                }
            }

            int maxSize = Math.Max(Math.Max(Math.Max(image.GetLength(0), image.GetLength(1)), kernel.GetLength(0)), kernel.GetLength(1));
            int size = 1;
            while (size < maxSize)
            {
                size *= 2;
            }
            //size *= 2;
            Console.WriteLine(size);

            //Create complext image
            Complex[,] image_complex = new Complex[size, size];
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image_complex[i, j] = new Complex((image[i, j] - image_min) / (image_max - image_min), 0);
                }
            }
            //Create complext kernel
            double kernel_sum = 0;
            Complex[,] kernel_complexl = new Complex[size, size];
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    kernel_sum += (kernel[i, j] - kernel_min) / (kernel_max - kernel_min);
                    kernel_complexl[i, j] = new Complex((kernel[i, j] - kernel_min)/ (kernel_max - kernel_min), 0);
                }
            }
            kernel_complexl[0, 0] = new Complex(1, 0);
            //Do work
            FourierTransform.FFT2(image_complex, FourierTransform.Direction.Forward);
            FourierTransform.FFT2(kernel_complexl, FourierTransform.Direction.Forward);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    image_complex[i, j] = image_complex[i, j] * new Complex(kernel_complexl[i, j].Re, -kernel_complexl[i, j].Im);
                }
            }
            FourierTransform.FFT2(image_complex, FourierTransform.Direction.Backward);

            double normalization = size * size / kernel_sum;
            double[,] result = new double[image.GetLength(0), image.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = image_complex[i, j].Re * normalization;
       
                }
            }
            return result;
        }


        public static int[,] BitMapToIntArray(Bitmap source)
        {
            int[,] array = new int[source.Width, source.Height];
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    int r = source.GetPixel(i, j).R;
                    int g = source.GetPixel(i, j).G;
                    int b = source.GetPixel(i, j).B;
                    array[i, j] = (int)(r * 0.2125 + g * 0.7154 + b * 0.0721);
                }
            }
            return array;
        }

        public static double[,] BitMapToDoubleArray(Bitmap source, double threshold)
        {
            double[,] array = new double[source.Width, source.Height];
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {

                    int r = source.GetPixel(i, j).R;
                    int g = source.GetPixel(i, j).G;
                    int b = source.GetPixel(i, j).B;
                    double gray = (r * 0.2125 + g * 0.7154 + b * 0.0721);
                    if (threshold < gray)
                    {
                        array[i, j] = 1.0;
                    }
                    else
                    {
                        array[i, j] = -1.0;
                    }
                }
            }
            return array;
        }

        public static Complex[,] BitMapToComplexArray(Bitmap source)
        {
            Complex[,] array = new Complex[source.Width, source.Height];
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    int r = source.GetPixel(i, j).R;
                    int g = source.GetPixel(i, j).G;
                    int b = source.GetPixel(i, j).B;           
                    array[i, j] = new Complex((r * 0.2125 + g * 0.7154 + b * 0.0721), 0);
                }
            }
            return array;
        }

        public static Bitmap IntArrayToBitMap(int[,] array, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
            var img = UnmanagedImage.Create(array.GetLength(0), array.GetLength(1), format);

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {

                    img.SetPixel(i, j, new AForge.Imaging.RGB((byte)array[i, j], (byte)array[i, j], (byte)array[i, j]).Color);
                }
            }
            return img.ToManagedImage();
        }

        public static Bitmap DoubleArrayToBitMap(double[,] array, double rescale, double intercept)
        {
            var img = UnmanagedImage.Create(array.GetLength(0), array.GetLength(1), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    double d_value = ((array[i, j] * rescale) + intercept);
                   byte value = (byte)Math.Min(Math.Max(0, d_value), 255);                
                    img.SetPixel(i, j, new AForge.Imaging.RGB(value, value, value).Color);
                }
            }
            return img.ToManagedImage();
        }


        public static Bitmap ComplexArrayToBitMap(Complex[,] array)
        {
            var img = UnmanagedImage.Create(array.GetLength(0), array.GetLength(1), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    img.SetPixel(i, j, new AForge.Imaging.RGB((byte)array[i, j].Re, (byte)array[i, j].Re, (byte)array[i, j].Re).Color);
                }
            }
            return img.ToManagedImage();
        }

        public static Bitmap ConvertTo24(Bitmap bmpIn)
        {
            Bitmap converted = new Bitmap(bmpIn.Width, bmpIn.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(converted))
            {
                // Prevent DPI conversion
                g.PageUnit = GraphicsUnit.Pixel;
                // Draw the image
                g.DrawImageUnscaled(bmpIn, 0, 0);
            }
            return converted;
        }
    }
}
