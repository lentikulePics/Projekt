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
        /// <summary>
        /// List indexu - klicem je vzdy cesta k souboru, hodnotou je seznam pozic, na kterych se obrazek ma objevit
        /// </summary>
        private List<KeyValuePair<String, List<int>>> indexList = new List<KeyValuePair<String, List<int>>>();
        /// <summary>
        /// obsahuje informace o vystupnim obrazku
        /// </summary>
        private OutputPictureData outData;
        /// <summary>
        /// filter pro 1. fazi prokladani
        /// </summary>
        private FilterType initialResizeFilter;
        /// <summary>
        /// filter pro 2. fazi prokladani
        /// </summary>
        private FilterType finalResampleFilter;

        /// <summary>
        /// konstruktor, ktery z Listu cest k souborum vytvori List indexu
        /// </summary>
        /// <param name="paths">List cest k souborum</param>
        /// <param name="outData">informace o vystupnim obrazku</param>
        /// <param name="initialResizeFilter">filter pro 1. fazi prokladani</param>
        /// <param name="finalResampleFilter">filter pro 2. fazi prokladani</param>
        public PictureContainer(List<String> paths, OutputPictureData outData, FilterType initialResizeFilter, FilterType finalResampleFilter)
        {
            this.outData = outData;
            this.initialResizeFilter = initialResizeFilter;
            this.finalResampleFilter = finalResampleFilter;
            Dictionary<String, List<int>> indexTable = new Dictionary<String, List<int>>();
            for (int i = 0; i < paths.Count; i++)
            {
                if (indexTable.ContainsKey(paths[i]))
                {
                    indexTable[paths[i]].Add(i);
                }
                else
                {
                    indexTable.Add(paths[i], new List<int>{i});
                }
            }
            indexList = indexTable.ToList();
        }


        /*private List<Picture> pictures = new List<Picture>();

        public Picture Interlace(double inchWidth, double inchHeight, double dpi, double lpi,
            FilterType initialResizeFilter, FilterType finalResampleFilter, ProgressBar progressBar)
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
        }*/
    }
}
