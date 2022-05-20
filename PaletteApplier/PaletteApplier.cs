using System;
using System.Collections.Generic;
using System.Drawing;

namespace PaletteApplier
{
    class PaletteApplier
    {
        private Image ColorPalette;
        private Image ColorlessImage;
        private Image ColorfulImage;

        public int CalculateColorfulImage()
        {
            if (ColorlessImage == null || ColorPalette == null)
            {
                return 1;
            }
            Bitmap t = new Bitmap(ColorlessImage.Width, ColorlessImage.Height);
            Bitmap cl = ColorlessImage as Bitmap;
            for (int i = 0; i < t.Height; i++)
            {
                for (int j = 0; j < t.Width; j++) //the alpha param of the pixel is its x coordinate on palette
                {
                    Color c = cl.GetPixel(j, i);
                    byte alp = c.A;
                    Bitmap pal = ColorPalette as Bitmap;
                    Color n = pal.GetPixel(alp, 0);
                    t.SetPixel(j, i, n);
                }
            }
            this.ColorfulImage = t;
            return 0;
        }

        public int SetColorfulImage(Image img)
        {
            this.ColorfulImage = img;
            return 0;
        }

        public Image GetColorfulImage()
        {
            if (this.ColorfulImage == null)
            {
                int r = CalculateColorfulImage();
                if (r == 1)
                {
                    return null;
                }
            }
            return this.ColorfulImage;
        }

        public int SetPalette(Image palette)
        {
            Bitmap bm = palette as Bitmap;
            if (bm.Width != 256)
            {
                return 1;
            }
            this.ColorPalette = palette;
            return 0;
        }

        public Image GetPalette()
        {
            if (this.ColorPalette == null)
            {
                int r = CalculatePalette();
                if (r != 0)
                {
                    return null;
                }
            }
            return this.ColorPalette;
        }

        public int CalculatePalette()
        {
            if (this.ColorfulImage == null)
            {
                return 1;
            }
            List<Color> list = FindDifferentColors(ColorfulImage);
            if (list.Count > 256)
            {
                return 2;
            }
            Bitmap bm = new Bitmap(list.Count, 1);
            for (int i = 0; i < list.Count; i++)
            {
                bm.SetPixel(i, 0, list[i]);
            }
            for (int i = list.Count; i < 256; i++)
            {
                Color c = Color.FromArgb(0, 255, 255, 255);
                bm.SetPixel(i, 0, c);
            }
            this.ColorPalette = bm;
            return 0;
        }

        public List<Color> FindDifferentColors(Image img)
        {
            List<Color> res = new List<Color>();
            Bitmap bm = img as Bitmap;
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    Color c = bm.GetPixel(i, j);
                    if (!ColorInside(res, c))
                    {
                        res.Add(c);
                    }
                }
            }
            return res;
        }

        public bool ColorInside(List<Color> list, Color c)
        {
            foreach (Color cl in list)
            {
                if (ColorsEqual(c, cl))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ColorsEqual(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
        }

        public int SetColorlessImage(Image img) 
        {
            Bitmap bm = img as Bitmap;
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    Color c = bm.GetPixel(i, j);
                }
            }
            this.ColorlessImage = img;
            return 0;
        }

        public int CalculateColorlessImage(bool flag)
        {
            Image palette = GetPalette();
            Image colored = GetColorfulImage();
            if (palette == null || colored == null)
            {
                return 1;
            }
            Bitmap col = colored as Bitmap;
            Bitmap bm = new Bitmap(col.Width, col.Height);
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    Color c = col.GetPixel(i, j);
                    if (c.A < 5) //if the pixel is transparent, its first (transparent) color on palette
                    {
                        bm.SetPixel(i, j, Color.FromArgb(0, 255, 255, 255));
                        continue;
                    }
                    int a = GetAlpha(c, palette);
                    if (a == -1)
                    {
                        a = GetLikeliestColor(c);
                    }
                    Color cl = Color.FromArgb(a, 255, 255, 255);
                    bm.SetPixel(i, j, cl);
                }
            }
            this.ColorlessImage = bm;
            return 0;
        }

        public int GetIndex(List<Color> list, Color c)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (ColorsEqual(list[i], c))
                    return i;
            }
            return -1;
        }

        public int GetAlpha(Color c, Image palette)
        {
            Bitmap pal = palette as Bitmap;
            for (int i = 0; i < 256; i++)
            {
                if (ColorsEqual(pal.GetPixel(i, 0), c))
                {
                    return i;
                }
            }
            return -1;
        }

        public Image GetColorlessImage()
        {
            if (this.ColorlessImage == null)
            {
                int r = CalculateColorlessImage(true);
                if (r != 0)
                {
                    return null;
                }
            }
            return this.ColorlessImage;
        }

        public int GetLikeliestColor(Color c) //trying to find the color on the palette that looks almos the same like this
        {
            int mindiff = 1000000;
            int index = -1;
            Bitmap pal = this.ColorPalette as Bitmap;
            for (int i = 0; i < 256; i++)
            {
                Color t = pal.GetPixel(i, 0);
                int difA = Math.Abs(t.A - c.A);
                int difR = Math.Abs(t.R - c.R);
                int difG = Math.Abs(t.G - c.G);
                int difB = Math.Abs(t.B - c.B);
                int dif = difA * difA + difR * difR + difG * difG
                    + difB * difB;
                if (dif < mindiff)
                {
                    mindiff = dif;
                    index = i;
                }
            }
            return index;
        }
    }
}
