using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Image_Manipulation
{
    public class Image
    {
        public Color[,] Pixels;
        public int Width { get { return Pixels.GetLength(0); } }
        public int Height { get { return Pixels.GetLength(1); } }
        public int Depth { get; private set; }

        public static Image FromStream(Stream file)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(file);
            Image img = new Image(bmp);
            bmp.Dispose();
            return img;
        }

        public static Image FromStream(Stream file, int threadCount)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(file);
            Image img = new Image(bmp, threadCount);
            bmp.Dispose();
            return img;
        }

        Image(System.Drawing.Bitmap bmp)
        {
            Pixels = new Color[bmp.Size.Width, bmp.Size.Height];
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
            int PixelCount = Width * Height;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int step = Depth / 8;
            byte[] rgbValues = new byte[PixelCount * step];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, rgbValues.Length);
            
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    switch (Depth)
                    {
                        case 32:
                            Pixels[x, y].B = rgbValues[((x + y * Width) * 4)];
                            Pixels[x, y].G = rgbValues[((x + y * Width) * 4) + 1];
                            Pixels[x, y].R = rgbValues[((x + y * Width) * 4) + 2];
                            Pixels[x, y].A = rgbValues[((x + y * Width) * 4) + 3];
                            break;
                        case 24:
                            Pixels[x, y].B = rgbValues[((x + y * Width) * 3)];
                            Pixels[x, y].G = rgbValues[((x + y * Width) * 3) + 1];
                            Pixels[x, y].R = rgbValues[((x + y * Width) * 3) + 2];
                            Pixels[x, y].A = 255;
                            break;
                        case 8:
                            Pixels[x, y].B = rgbValues[x + y * Width];
                            Pixels[x, y].G = rgbValues[x + y * Width];
                            Pixels[x, y].R = rgbValues[x + y * Width];
                            Pixels[x, y].A = 255;
                            break;
                    }
                }
            }
            
            bmp.UnlockBits(bmpData);
        }

        Image(System.Drawing.Bitmap bmp, int threadCount)
        {
            Pixels = new Color[bmp.Size.Width, bmp.Size.Height];
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
            int PixelCount = Width * Height;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int step = Depth / 8;
            byte[] rgbValues = new byte[PixelCount * step];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, rgbValues.Length);

            List<Thread> AllThreads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(delegate (object threadNum)
                {
                    int num = (int)threadNum;
                    int baseWidthOfChunk = Width / threadCount;
                    int widthOfChunk = Width - (num * baseWidthOfChunk) < baseWidthOfChunk ? Width - (num * baseWidthOfChunk) : baseWidthOfChunk;
                    for (int x = num * baseWidthOfChunk; x < (num * baseWidthOfChunk) + widthOfChunk; x++)
                    {
                        for (int y = 0; y < Height; y++)
                            switch (Depth)
                            {
                                case 32:
                                    Pixels[x, y].B = rgbValues[((x + y * Width) * 4)];
                                    Pixels[x, y].G = rgbValues[((x + y * Width) * 4) + 1];
                                    Pixels[x, y].R = rgbValues[((x + y * Width) * 4) + 2];
                                    Pixels[x, y].A = rgbValues[((x + y * Width) * 4) + 3];
                                    break;
                                case 24:
                                    Pixels[x, y].B = rgbValues[((x + y * Width) * 3)];
                                    Pixels[x, y].G = rgbValues[((x + y * Width) * 3) + 1];
                                    Pixels[x, y].R = rgbValues[((x + y * Width) * 3) + 2];
                                    Pixels[x, y].A = 255;
                                    break;
                                case 8:
                                    Pixels[x, y].B = rgbValues[x + y * Width];
                                    Pixels[x, y].G = rgbValues[x + y * Width];
                                    Pixels[x, y].R = rgbValues[x + y * Width];
                                    Pixels[x, y].A = 255;
                                    break;
                            }
                    }
                });
                AllThreads.Add(t);
                int threadNumber = i;
                t.Start(threadNumber);
            }

            bool threadsActive;
            do
            {
                threadsActive = false;
                foreach (Thread t in AllThreads)
                {
                    if (t.IsAlive) { threadsActive = true; }
                }
            } while (threadsActive);

            bmp.UnlockBits(bmpData);
        }

        public Image(int width, int height) { Pixels = new Color[width, height]; }

        public void SaveToFile(Stream file)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, Width, Height);
            Depth = 32;
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int PixelCount = Width * Height;
            int step = Depth / 8;
            byte[] rgbValues = new byte[PixelCount * step];
            IntPtr ptr = bmpData.Scan0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    rgbValues[((x + y * Width) * 4)] = (byte)Pixels[x, y].B;
                    rgbValues[((x + y * Width) * 4) + 1] = (byte)Pixels[x, y].G;
                    rgbValues[((x + y * Width) * 4) + 2] = (byte)Pixels[x, y].R;
                    rgbValues[((x + y * Width) * 4) + 3] = (byte)Pixels[x, y].A;
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
            bmp.UnlockBits(bmpData);

            bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
        }

        public void SaveToFile(Stream file, int threadCount)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, Width, Height);
            Depth = 32;
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int PixelCount = Width * Height;
            int step = Depth / 8;
            byte[] rgbValues = new byte[PixelCount * step];
            IntPtr ptr = bmpData.Scan0;
            List<Thread> AllThreads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(delegate (object threadNum)
                {
                    int num = (int)threadNum;
                    int baseWidthOfChunk = Width / threadCount;
                    int widthOfChunk = Width - (num * baseWidthOfChunk) < baseWidthOfChunk ? Width - (num * baseWidthOfChunk) : baseWidthOfChunk;
                    for (int x = num * baseWidthOfChunk; x < (num * baseWidthOfChunk) + widthOfChunk; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            rgbValues[((x + y * Width) * 4)] = (byte)Pixels[x, y].B;
                            rgbValues[((x + y * Width) * 4) + 1] = (byte)Pixels[x, y].G;
                            rgbValues[((x + y * Width) * 4) + 2] = (byte)Pixels[x, y].R;
                            rgbValues[((x + y * Width) * 4) + 3] = (byte)Pixels[x, y].A;
                        }
                    }
                });
                AllThreads.Add(t);
                int threadNumber = i;
                t.Start(threadNumber);
            }

            bool threadsActive;
            do
            {
                threadsActive = false;
                foreach (Thread t in AllThreads)
                {
                    if (t.IsAlive) { threadsActive = true; }
                }
            } while (threadsActive);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
            bmp.UnlockBits(bmpData);

            bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
        }

        public void Dispose()
        {
            Pixels = null;
            GC.Collect();
        }
    }
}
