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
        /// obsahuje informace o pasovacích značkách
        /// </summary>
        private LineData lineData;
        /// <summary>
        /// pocet obrazku k prolozeni
        /// </summary>
        private int pictureCount;
        /// <summary>
        /// vysledny obrazek
        /// </summary>
        private Picture result = null;

        /// <summary>
        /// konstruktor, ktery z Listu cest k souborum vytvori List indexu
        /// </summary>
        /// <param name="pictures">seznam obrazku, bud nenactenych s nastavenou cestou k souboru nebo vytvorenych programove</param>
        /// <param name="interlacingData">data potrebna k prolozeni</param>
        /// <param name="lineData">data potrebna k pridani pasovacich znacek</param>
        public PictureContainer(List<Picture> pictures, InterlacingData interlacingData, LineData lineData)
        {
            pictureCount = pictures.Count;
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

        public void Interlace()
        {
            int pxWidth = (int)(interlacingData.GetWidth() * interlacingData.GetPictureResolution());
            int pxHeight = (int)(interlacingData.GetHeight() * interlacingData.GetPictureResolution());
            int lenses = (int)(interlacingData.GetWidth() * interlacingData.GetLenticuleDensity());
            for (int i = 0; i < indexList.Count; i++)
            {
                Picture currentPic = indexList[i].Key;
                List<int> indexes = indexList[i].Value;
                bool loadPic = !currentPic.IsCreated();
                if (loadPic)
                    currentPic.Load();
                if (result == null)
                    result = new Picture(lenses * pictureCount, currentPic.GetHeight());
                currentPic.Resize(lenses, currentPic.GetHeight(), interlacingData.GetInitialResizeFilter());
                for (int j = 0; j < indexes.Count; j++)
                {
                    for (int k = 0; k < lenses; k++)
                    {
                        int column = k * pictureCount + (pictureCount - 1 - indexes[j]);
                        result.CopyColumn(currentPic, k, column, 0);
                    }
                }
                if (loadPic)
                    currentPic.Destroy();
            }
            result.Resize(pxWidth, pxHeight, interlacingData.GetFinalResampleFilter());
        }

        public Picture GetResult()
        {
            return result;
        }
    }
}
