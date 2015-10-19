using System.Collections.Generic;
using System.Threading;

namespace Image_Manipulation.Image_Effect
{
    public static class Pixelate
    {
        public static Image DoPixelate(Image img, int xRegion, int yRegion, int threadCount)
        {
            List<Thread> AllThreads = new List<Thread>();
            Image returnImage = new Image(img.Width, img.Height);
            
            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(PixelizeChunk));
                int threadChunkWidth = (int)((((float)img.Width / xRegion) / threadCount) * xRegion);
                t.Start(new ChunkObject(img, returnImage, i * threadChunkWidth, 0, xRegion, yRegion, (i + 1) * threadChunkWidth) as object);
                AllThreads.Add(t);
            }

            bool threadsActive;
            do {
                threadsActive = false;
                foreach (Thread t in AllThreads)
                {
                    if (t.IsAlive) { threadsActive = true; }
                }
            } while (threadsActive);

            return returnImage;
        }

        public static Image DoPixelate(Image img, int xRegion, int yRegion)
        {
            Image returnImage = new Image(img.Width, img.Height);

            PixelizeChunk(new ChunkObject(img, returnImage, 0, 0, xRegion, yRegion, img.Width));

            return returnImage;
        }

        /// <summary>
        /// Used for threading
        /// </summary>
        static void PixelizeChunk(object o)
        {
            ChunkObject obj = (ChunkObject)o;
            for (int b = 0; obj.chunkPosX < obj.threadStopPosX; obj.chunkPosX += obj.chunkSizeX)
            {
                for (obj.chunkPosY = 0; obj.chunkPosY < obj.source.Height; obj.chunkPosY += obj.chunkSizeY)
                {
                    int colAvgXSize = obj.source.Width - obj.chunkPosX < obj.chunkSizeX ? obj.source.Width - obj.chunkPosX : obj.chunkSizeX;
                    int colAvgYSize = obj.source.Height - obj.chunkPosY < obj.chunkSizeY ? obj.source.Height - obj.chunkPosY : obj.chunkSizeY;
                    Color[,] colAvg = new Color[colAvgXSize, colAvgYSize];

                    //Add all pixels to colAvg to find average color from the block of pixels
                    for (int i = 0; i < colAvgXSize; i++)
                        for (int j = 0; j < colAvgYSize; j++)
                            colAvg[i, j] = obj.source.Pixels[obj.chunkPosX + i, obj.chunkPosY + j];

                    //Find the average color
                    Color avg = AverageColor(colAvg);

                    //Assign all the pixels in the final picture in the block to the average color
                    for (int i = 0; i < colAvgXSize; i++)
                        for (int j = 0; j < colAvgYSize; j++)
                            obj.product.Pixels[i + obj.chunkPosX, j + obj.chunkPosY] = avg;
                }
            }
        }

        /// <summary>
        /// Single Thread
        /// </summary>
        static void PixelizeChunk(ChunkObject obj)
        {
            for (int b = 0; obj.chunkPosX < obj.threadStopPosX; obj.chunkPosX += obj.chunkSizeX)
            {
                for (obj.chunkPosY = 0; obj.chunkPosY < obj.source.Height; obj.chunkPosY += obj.chunkSizeY)
                {
                    int colAvgXSize = obj.source.Width - obj.chunkPosX < obj.chunkSizeX ? obj.source.Width - obj.chunkPosX : obj.chunkSizeX;
                    int colAvgYSize = obj.source.Height - obj.chunkPosY < obj.chunkSizeY ? obj.source.Height - obj.chunkPosY : obj.chunkSizeY;
                    Color[,] colAvg = new Color[colAvgXSize, colAvgYSize];

                    //Add all pixels to colAvg to find average color from the block of pixels
                    for (int i = 0; i < colAvgXSize; i++)
                        for (int j = 0; j < colAvgYSize; j++)
                            colAvg[i, j] = obj.source.Pixels[obj.chunkPosX + i, obj.chunkPosY + j];

                    //Find the average color
                    Color avg = AverageColor(colAvg);

                    //Assign all the pixels in the final picture in the block to the average color
                    for (int i = 0; i < colAvgXSize; i++)
                        for (int j = 0; j < colAvgYSize; j++)
                            obj.product.Pixels[i + obj.chunkPosX, j + obj.chunkPosY] = avg;
                }
            }
        }

        static Color AverageColor(Color[,] colors)
        {
            int alpha = 0, red = 0, green = 0, blue = 0;
            int i = 0;
            foreach (Color col in colors)
            {
                i++;
                alpha += col.A;
                red += col.R;
                green += col.G;
                blue += col.B;
            }

            return new Color(alpha / i, red / i, green / i, blue / i);
        }
    }

    class ChunkObject
    {
        public Image source, product;
        public int chunkPosX, chunkPosY, chunkSizeX, chunkSizeY;
        public int threadStopPosX;

        public ChunkObject(Image s, Image p, int posX, int posY, int sizeX, int sizeY, int stopPosX)
        {
            source = s;
            product = p;
            chunkPosX = posX;
            chunkPosY = posY;
            chunkSizeX = sizeX;
            chunkSizeY = sizeY;
            threadStopPosX = stopPosX;
        }
        public ChunkObject(Image s, Image p, int posX, int posY, int sizeX, int sizeY)
        {
            source = s;
            product = p;
            chunkPosX = posX;
            chunkPosY = posY;
            chunkSizeX = sizeX;
            chunkSizeY = sizeY;
            threadStopPosX = 0;
        }
        public ChunkObject(Image s, Image p, int posX, int sizeX, int sizeY)
        {
            source = s;
            product = p;
            chunkPosX = posX;
            chunkPosY = 0;
            chunkSizeX = sizeX;
            chunkSizeY = sizeY;
            threadStopPosX = 0;
        }
    }
}