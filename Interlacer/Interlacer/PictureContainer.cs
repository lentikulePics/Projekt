using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;

namespace Interlacer
{
    class PictureContainer
    {
        private List<Picture> pictures = new List<Picture>();

        public void AddPicture(Picture picture)
        {
            pictures.Add(picture);
        }

        public Picture Interlace(double inchWidth, double inchHeight, double dpi, double lpi,
            Picture.FilterType initialResizeFilter, Picture.FilterType finalResampleFilter, ProgressBar progressBar)
        {
            int pxWidth = (int)(inchWidth * dpi);
            int pxHeight = (int)(inchHeight * dpi);
            int lenses = (int)(inchWidth * lpi);
            Picture result = null;
            progressBar.Maximum = pictures.Count * 3 + 1;
            progressBar.Step = 1;
            progressBar.Value = 0;
            for (int i = 0; i < pictures.Count; i++)
            {
                pictures[i].Load();
                if (i == 0)
                    result = new Picture(lenses * pictures.Count, pictures[i].GetHeight());
                progressBar.PerformStep();
                pictures[i].Resize(lenses, pictures[i].GetHeight(), initialResizeFilter);
                progressBar.PerformStep();
                for (int j = 0; j < lenses; j++)
                {
                    int column = j * pictures.Count + i;
                    result.CopyColumn(pictures[i], j, column, 0);
                }
                pictures[i].Destroy();
                progressBar.PerformStep();
                Application.DoEvents();
            }
            result.Resize(pxWidth, pxHeight, finalResampleFilter);
            progressBar.PerformStep();
            return result;
        }
    }
}
