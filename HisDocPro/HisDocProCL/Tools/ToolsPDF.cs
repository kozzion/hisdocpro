
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDocProUI.Tools
{
    public class ToolsPDF
    {
        public static List<string> Convert(string filename)
        {
            List<string> targeList = new List<string>();
            PdfDocument document = PdfReader.Open(filename);

            int imageCount = 0;
            // Iterate pages
            foreach (PdfPage page in document.Pages)
            {
                // Get resources dictionary
                PdfDictionary resources = page.Elements.GetDictionary("/Resources");
                if (resources != null)
                {
                    // Get external objects dictionary
                    PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
                    if (xObjects != null)
                    {
                        ICollection<PdfItem> items = xObjects.Elements.Values;
                        // Iterate references to external objects
                        foreach (PdfItem item in items)
                        {
                            PdfReference reference = item as PdfReference;
                            if (reference != null)
                            {
                                PdfDictionary xObject = reference.Value as PdfDictionary;
                                // Is external object an image?
                                if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                {
                                    ExportImage(xObject, ref imageCount, targeList);
                                }
                            }
                        }
                    }
                }
            }
            return targeList;
        }


        static void ExportImage(PdfDictionary image, ref int count, List<string> targeList)
        {

            Console.WriteLine("Clear");
            string filter = "";
            if (image.Elements.GetArray("/Filter") == null)
            {
                filter = image.Elements.GetValue("/Filter").ToString();
            }
            else
            {
                filter = image.Elements.GetArray("/Filter").First().ToString();
            }
            switch (filter)
            {
                case "/DCTDecode":
                    ExportJpegImage(image, ref count, targeList);
                    break;

                case "/FlateDecode":
                    ExportAsPngImage(image, ref count, targeList);
                    break;
            }

        }

        static void ExportJpegImage(PdfDictionary image, ref int count, List<string> targeList)
        {
            // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
            byte[] stream = image.Stream.Value;
            string target = @"D:\Projects\hisdocpro\temp\temp_" + count + ".jpeg";
            FileStream fs = new FileStream(target, FileMode.Create, FileAccess.Write);
            count++;
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(stream);
            bw.Close();
            targeList.Add(target);
        }

        static void ExportAsPngImage(PdfDictionary image, ref int count, List<string> targeList)
        {
            int width = image.Elements.GetInteger(PdfImage.Keys.Width);
            int height = image.Elements.GetInteger(PdfImage.Keys.Height);
            int bitsPerComponent = image.Elements.GetInteger(PdfImage.Keys.BitsPerComponent);
            
            throw new NotImplementedException();
            // TODO: You can put the code here that converts vom PDF internal image format to a Windows bitmap
            // and use GDI+ to save it in PNG format.
            // It is the work of a day or two for the most important formats. Take a look at the file
            // PdfSharp.Pdf.Advanced/PdfImage.cs to see how we create the PDF image formats.
            // We don't need that feature at the moment and therefore will not implement it.
            // If you write the code for exporting images I would be pleased to publish it in a future release
            // of PDFsharp.
        }
    }
}
