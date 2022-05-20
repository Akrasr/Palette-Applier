using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteApplier
{
    class Drawer
    {
        private Graphics Graph;

        public Drawer() { }

        public Drawer(Graphics g)
        {
            this.Graph = g;
        }

        public void DrawImage(Image img, int sector)
        {
            int wid = (int)(Graph.VisibleClipBounds.Width);
            int hei = (int)(Graph.VisibleClipBounds.Height) / 3;
            float mas = GetMaschtab(img);
            int imagewid = (int)((float)img.Width * mas);
            int imagehei = (int)((float)img.Height * mas);
            int x = (wid - imagewid) / 2;
            int y = (hei - imagehei) / 2 + hei * sector;
            Graph.DrawImage(img, x, y, imagewid, imagehei);
        }

        public float GetMaschtab(Image img)
        {
            int wid = (int)(Graph.VisibleClipBounds.Width);
            int hei = (int)(Graph.VisibleClipBounds.Height) / 3;
            float wex = (float)wid / (float)img.Width;
            float hex = (float)hei / (float)img.Height;
            return wex < hex ? wex : hex;
        }
    }
}
