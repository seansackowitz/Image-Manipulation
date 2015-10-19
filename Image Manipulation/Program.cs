using System;
using System.IO;
using System.Diagnostics;
using Image_Manipulation.Image_Effect;

namespace Image_Manipulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Image path to open:");
            string InputPath = "c:\\1.png";//Console.ReadLine();
            Console.WriteLine("Image path to save:");
            string OutputPath = "c:\\2.png";//Console.ReadLine();
            Console.WriteLine("Image path to save:");
            string OutputPathT = "c:\\3.png";//Console.ReadLine();
            Console.WriteLine("accuracy:");
            float accuracy = float.Parse(Console.ReadLine());
            Console.WriteLine("Thread Count:");
            int threadCount = 8;// int.Parse(Console.ReadLine());
            Image imageSource;
            Image product;
            Image productT;
            Stopwatch s = new Stopwatch();
            s.Start();

            using (FileStream openStream = new FileStream(InputPath, FileMode.Open)) { imageSource = Image.FromStream(openStream, threadCount); }
            Console.WriteLine("Threading Image Loaded in {0}", s.ElapsedMilliseconds);

            s.Restart();
            product = Edge.MarkEdge(imageSource, accuracy, new Color(255, 0, 0, 0));
            Console.WriteLine("Image Thrown in {0}", s.ElapsedMilliseconds);
            imageSource.Dispose();

            //s.Restart();
            //productT = Pixelate.DoPixelate(imageSource, radius, radius, threadCount);
            //Console.WriteLine("Threading Image Thrown in {0}", s.ElapsedMilliseconds);

            s.Restart();
            using (FileStream saveStream = new FileStream(OutputPath, FileMode.Create)) { product.SaveToFile(saveStream, threadCount); }
            Console.WriteLine("Image Saved in {0}", s.ElapsedMilliseconds);
            product.Dispose();

            //s.Restart();
            //using (FileStream saveStream = new FileStream(OutputPathT, FileMode.Create)) { productT.SaveToFile(saveStream, threadCount); }
            //Console.WriteLine("Threading Image Saved in {0}", s.ElapsedMilliseconds);
            //productT.Dispose();

            Console.WriteLine("Restart? (Y/N)");
            if (Console.ReadKey().Key == ConsoleKey.Y)
                Main(args);
        }
    }
}
