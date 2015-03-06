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
        private List<KeyValuePair<Picture, List<int>>> indexList = new List<KeyValuePair<Picture, List<int>>>();
        /// <summary>
        /// obsahuje informace o vystupnim obrazku
        /// </summary>
        private InterlacingData interlacingData;

        /// <summary>
        /// konstruktor, ktery z Listu cest k souborum vytvori List indexu
        /// </summary>
        /// <param name="pictures">seznam obrazku, bud nenactenych s nastavenou cestou k souboru nebo vytvorenych programove</param>
        /// <param name="interlacingData">data potrebna k prolozeni</param>
        public PictureContainer(List<Picture> pictures, InterlacingData interlacingData)
        {
            this.interlacingData = interlacingData;
            Dictionary<Picture, List<int>> indexTable = new Dictionary<Picture, List<int>>();
            for (int i = 0; i < pictures.Count; i++)
            {
                if (indexTable.ContainsKey(pictures[i]))
                {
                    indexTable[pictures[i]].Add(i);
                }
                else
                {
                    indexTable.Add(pictures[i], new List<int>{i});
                }
            }
            indexList = indexTable.ToList();
        }

        public void show()
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                String str = "";
                for (int j = 0; j < indexList[i].Value.Count; j++)
                {
                    str += indexList[i].Value[j] + ", ";
                }
                System.Windows.Forms.MessageBox.Show(indexList[i].Key.ToString() + ": " + str);
            }
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
