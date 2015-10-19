using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Manipulation.Image_Effect
{
    public static class Edge
    {
        public static Image MarkEdge(Image img, float accuracy, Color edgeColor)
        {
            Image returnImage = new Image(img.Width, img.Height);
            // 1 2 3
            // 4 5 6
            // 7 8 9
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    //1, 4, 7
                    if (x - 1 > -1)
                    {
                        //4
                        if (img.Pixels[x - 1, y].Compare(img.Pixels[x, y]) > accuracy)
                        {
                            returnImage.Pixels[x - 1, y] = edgeColor;
                            returnImage.Pixels[x, y] = edgeColor;
                        }
                        //1
                        if (y - 1 > -1)
                            if (img.Pixels[x - 1, y - 1].Compare(img.Pixels[x, y]) > accuracy)
                            {
                                returnImage.Pixels[x - 1, y - 1] = edgeColor;
                                returnImage.Pixels[x, y] = edgeColor;
                            }
                        //7
                        if (y + 1 < img.Height)
                            if (img.Pixels[x - 1, y + 1].Compare(img.Pixels[x, y]) > accuracy)
                            {
                                returnImage.Pixels[x - 1, y + 1] = edgeColor;
                                returnImage.Pixels[x, y] = edgeColor;
                            }
                    }

                    //3, 6, 9
                    if (x + 1 < img.Width)
                    {
                        //4
                        if (img.Pixels[x + 1, y].Compare(img.Pixels[x, y]) > accuracy)
                        {
                            returnImage.Pixels[x + 1, y] = edgeColor;
                            returnImage.Pixels[x, y] = edgeColor;
                        }
                        //1
                        if (y - 1 > -1)
                            if (img.Pixels[x + 1, y - 1].Compare(img.Pixels[x, y]) > accuracy)
                            {
                                returnImage.Pixels[x + 1, y - 1] = edgeColor;
                                returnImage.Pixels[x, y] = edgeColor;
                            }
                        //7
                        if (y + 1 < img.Height)
                            if (img.Pixels[x + 1, y + 1].Compare(img.Pixels[x, y]) > accuracy)
                            {
                                returnImage.Pixels[x + 1, y + 1] = edgeColor;
                                returnImage.Pixels[x, y] = edgeColor;
                            }
                    }
                    //2
                    if (y - 1 > -1)
                        if (img.Pixels[x, y - 1].Compare(img.Pixels[x, y]) > accuracy)
                        {
                            returnImage.Pixels[x, y - 1] = edgeColor;
                            returnImage.Pixels[x, y] = edgeColor;
                        }
                    //8
                    if (y + 1 < img.Height)
                        if (img.Pixels[x, y + 1].Compare(img.Pixels[x, y]) > accuracy)
                        {
                            returnImage.Pixels[x, y + 1] = edgeColor;
                            returnImage.Pixels[x, y] = edgeColor;
                        }
                }
            }

            return returnImage;
        }
    }
}
