using HisDocProUI.Model;
using HisDocProUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HisDocProCl.Tools;
using HisDocProCL.Model;

namespace HisDocProCL.Tools
{
    public class ToolsConnectedComponent
    {
        public static List<ConnectedComponent> GetConnectedComponents(double[,] image, double threshold, ModelToken token)
        {
            List<ConnectedComponent> cc = new List<ConnectedComponent>();
            bool[,] blacklist = new bool[image.GetLength(0), image.GetLength(1)];
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    if (!blacklist[i, j])
                    {
                        if (threshold < image[i, j])
                        {
                            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
                            List<Tuple<int, int>> node_list = new List<Tuple<int, int>>();
                            queue.Enqueue(new Tuple<int, int>(i, j));
                            while (0 < queue.Count)
                            {                    
                                ProcessNode(image, threshold, blacklist, queue, node_list);
                            }
                            cc.Add(new ConnectedComponent(node_list, token, image));
                        }
                    }
                }
            }
            return cc;
        }

        private static void ProcessNode(double[,] image, double threshold, bool[,] blacklist, Queue<Tuple<int, int>> queue, List<Tuple<int, int>> node_list)
        {
            Tuple<int, int> node = queue.Dequeue();
            if (!blacklist[node.Item1, node.Item2])
            {
                blacklist[node.Item1, node.Item2] = true;
                if (threshold < image[node.Item1, node.Item2])
                {
                    node_list.Add(node);

                    if (node.Item1 != 0)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1 - 1, node.Item2));
                    }
                    if (node.Item2 != 0)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 - 1));
                    }

                    if (node.Item1 != image.GetLength(0) - 1)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1 + 1, node.Item2));
                    }
                    if (node.Item2 != image.GetLength(1) - 1)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 + 1));
                    }
                }
            }
        }

        public static List<ConnectedComponent> GetConnectedComponents(int[,] image)
        {
            List<ConnectedComponent> cc = new List<ConnectedComponent>();
            bool[,] blacklist = new bool[image.GetLength(0), image.GetLength(1)];
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    if (!blacklist[i, j])
                    {
                        if (0 < image[i, j])
                        {
                            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
                            List<Tuple<int, int>> node_list = new List<Tuple<int, int>>();
                            queue.Enqueue(new Tuple<int, int>(i, j));
                            while (0 < queue.Count)
                            {
                                ProcessNode(image, 0, blacklist, queue, node_list);
                            }
                            throw new NotImplementedException();
                            //cc.Add(new ConnectedComponent(node_list, new ModelToken(), null));
                        }
                    }
                }
            }
            return cc;
        }
        private static void ProcessNode(int[,] image, double threshold, bool[,] blacklist, Queue<Tuple<int, int>> queue, List<Tuple<int, int>> node_list)
        {
            Tuple<int, int> node = queue.Dequeue();
            if (!blacklist[node.Item1, node.Item2])
            {
                blacklist[node.Item1, node.Item2] = true;
                if (threshold < image[node.Item1, node.Item2])
                {
                    node_list.Add(node);

                    if (node.Item1 != 0)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1 - 1, node.Item2));
                    }
                    if (node.Item2 != 0)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 - 1));
                    }

                    if (node.Item1 != image.GetLength(0) - 1)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1 + 1, node.Item2));
                    }
                    if (node.Item2 != image.GetLength(1) - 1)
                    {
                        queue.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 + 1));
                    }
                }
            }
        }


        public static Bitmap Filter(Bitmap imageBin, int minSize)
        {
            int [,] image = ToolsConvolution.BitMapToIntArray(imageBin);
            List<ConnectedComponent>  component_list = GetConnectedComponents(image);
            foreach (var component in component_list)
            {
                if (component.Component.Count < minSize)
                {
                    foreach (var pixel in component.Component)
                    {
                        image[pixel.Item1, pixel.Item2] = 0;
                    }
                }
            }
            return ToolsConvolution.IntArrayToBitMap(image, imageBin.PixelFormat);
        }  
    }
}
