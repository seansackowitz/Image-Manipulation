using System;
using System.Collections.Generic;
using System.Threading;

namespace Image_Manipulation.Image_Effect
{
    public static class Throw
    {
        public static Image DoThrow(Image img, int radius)
        {
            Image returnImage = new Image(img.Width, img.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    returnImage.Pixels[x, y] = img.Pixels[Random.RandomInt(0 < x - radius ? x - radius : 0, img.Width > x + radius + 1 ? x + radius + 1 : img.Width),
                                                          Random.RandomInt(0 < y - radius ? y - radius : 0, img.Height > y + radius + 1 ? y + radius + 1 : img.Height)];
                }
            }

            return returnImage;
        }

        public static Image DoThrow(Image img, int radius, int threadCount)
        {
            List<Thread> AllThreads = new List<Thread>();
            Image returnImage = new Image(img.Width, img.Height);

            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(delegate(object thread)
                {
                    int num = (int)thread;
                    int baseWidthOfChunk = img.Width / threadCount;
                    int widthOfChunk = img.Width - (num * baseWidthOfChunk) < baseWidthOfChunk ? img.Width - (num * baseWidthOfChunk) : baseWidthOfChunk;
                    for (int x = num * baseWidthOfChunk; x < (num * baseWidthOfChunk) + widthOfChunk; x++)
                    {
                        for (int y = 0; y < img.Height; y++)
                        {
                            returnImage.Pixels[x, y] = img.Pixels[Random.RandomInt(0 < x - radius ? x - radius : 0, img.Width > x + radius + 1 ? x + radius + 1 : img.Width),
                                                                  Random.RandomInt(0 < y - radius ? y - radius : 0, img.Height > y + radius + 1 ? y + radius + 1 : img.Height)];
                        }
                    }
                });
                int threadNum = i;
                AllThreads.Add(t);
                t.Start(threadNum);
                t.Join();
            }

            bool threadsActive;
            do
            {
                threadsActive = false;
                foreach (Thread t in AllThreads)
                    if (t.IsAlive)
                        threadsActive = true;
            } while (threadsActive);
            return returnImage;
        }
    }
}