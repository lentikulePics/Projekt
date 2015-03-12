using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// <summary>y
        /// sirka samotneho obrazku bez ramecku pred poslednim resizem
        /// </summary>
        private int preResamplePictureWidth = -1;
        /// <summary>
        /// sirka vystupniho obrzazku bez ramecku
        /// </summary>
        private double outputPictureWidth = -1.0;
        /// <summary>
        /// vyska vystupniho obrzazku bez ramecku
        /// </summary>
        private double outputPictureHeight = -1.0;
        /// <summary>
        /// sirka vstupnich obrazku
        /// </summary>
        private int inputPictureWidth = -1;
        /// <summary>
        /// vyska vstupnich obrazku
        /// </summary>
        private int inputPictureHeight = -1;

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
            this.lineData = lineData;
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

        private void adjustPictureSize(Picture picture)
        {
            if (picture.GetHeight() > inputPictureHeight || picture.GetWidth() > inputPictureWidth)
                picture.Clip(0, 0, inputPictureWidth, inputPictureHeight);
        }

        public bool CheckPictures()
        {
            bool sameSizes = true;
            for (int i = 0; i < indexList.Count; i++)
            {
                Picture pic = indexList[i].Key;
                if (!pic.IsCreated())
                    pic.Ping();
                if (i == 0)
                {
                    inputPictureHeight = pic.GetHeight();
                    inputPictureWidth = pic.GetWidth();
                }
                else if (pic.GetHeight() != indexList[i - 1].Key.GetHeight() ||
                         pic.GetWidth() != indexList[i - 1].Key.GetWidth())
                {
                    sameSizes = false;
                    if (pic.GetWidth() < inputPictureWidth)
                        inputPictureWidth = pic.GetWidth();
                    if (pic.GetHeight() < inputPictureHeight)
                        inputPictureHeight = pic.GetHeight();
                }
            }
            return sameSizes;
        }

        private Picture getPictureForMarkToLine()
        {
  
            int addWidth = 0;
            int addHeight = 0;
            if (lineData == null)
                return new Picture(preResamplePictureWidth, inputPictureHeight);
            
            if (lineData.GetLeft())
                addWidth = getAddWidthForLineAndIndent();
            if (lineData.GetRight())
                addWidth = addWidth + getAddWidthForLineAndIndent();
            if (lineData.GetTop())
                addHeight = getAddHeightForLineAndIndent();
            if (lineData.GetBottom())
                addHeight = addHeight + getAddHeightForLineAndIndent();

            return new Picture(preResamplePictureWidth + addWidth, inputPictureHeight + addHeight);
        }

        /// <summary>
        ///  vrací šířku rámečku s odsazením v px pro pridani pred finalnim resizem vydeleny pomerem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineAndIndentRadio(double ratio)
        {
            double widthFL = lineData.GetFrameWidth() + lineData.GetIndent();
            double widthFLpixel = widthFL * interlacingData.GetPictureResolution();
            return (int)(widthFLpixel / ratio);
        }

        /// <summary>
        ///  vrací šířku rámečku bez odsazením v px pro pridani pred finalnim resizem vydeleny pomerem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineRadio(double ratio)
        {
            double widthFL = lineData.GetFrameWidth();
            double widthFLpixel = widthFL * interlacingData.GetPictureResolution();
            return (int)(widthFLpixel / ratio);
        }


        /// <summary>
        ///  vrací šířku postraního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        public int getAddWidthForLineAndIndent()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineAndIndentRadio(ratio);
        }

        /// <summary>
        ///  vrací šířku postraního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        public int getAddWidthForLineAndIndentLeft()
        {
            if (lineData.GetLeft())
                return this.getAddWidthForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        ///  vrací výšku horního nebo spodního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        public int getAddHeightForLineAndIndent()
        {
             double ratio = outputPictureHeight / (double)(inputPictureHeight);
             return getAddSizeForLineAndIndentRadio(ratio);
        }

        /// <summary>
        ///  vrací výšku horního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        public int getAddHeightForLineAndIndentTop()
        {
            if (lineData.GetTop())
                return this.getAddHeightForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        ///  vrací výšku čar horního rámečku bez osazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        public int getAddHeightForLineTop()
        {
            double ratio = outputPictureHeight / (double)(inputPictureHeight);
            return getAddSizeForLineRadio(ratio);
        }
        /// <summary>
        ///  vrací šířku čar na stranách bez odsazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        public int getAddWidthForLine()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineRadio(ratio);
        }



        public void Interlace()
        {
            if (inputPictureHeight < 0 || inputPictureWidth < 0)
                throw new InvalidOperationException("CheckPictures was not called");
            this.outputPictureWidth = interlacingData.GetWidth() * interlacingData.GetPictureResolution();
            this.outputPictureHeight = interlacingData.GetHeight() * interlacingData.GetPictureResolution();
            //System.Windows.Forms.MessageBox.Show("" + pxHeight);
            int lenses = (int)(interlacingData.GetWidth() * interlacingData.GetLenticuleDensity());
            this.preResamplePictureWidth = lenses * pictureCount;

            for (int i = 0; i < indexList.Count; i++)
            {
                Picture currentPic = indexList[i].Key;
                List<int> indexes = indexList[i].Value;
                bool loadPic = !currentPic.IsCreated();
                if (loadPic)
                    currentPic.Load();
                adjustPictureSize(currentPic);
                if (result == null)
                    result = getPictureForMarkToLine();

                currentPic.Resize(lenses, currentPic.GetHeight(), interlacingData.GetInitialResizeFilter());
                for (int j = 0; j < indexes.Count; j++)
                {
                    for (int k = 0; k < lenses; k++)
                    {
                        int column = k * pictureCount + (pictureCount - 1 - indexes[j]) + this.getAddWidthForLineAndIndentLeft();
                        result.CopyColumn(currentPic, k, column, this.getAddHeightForLineAndIndentTop());
                    }
                }
                if (loadPic)
                    currentPic.Destroy();
            }
            drawLine();
            result.Save("mezi.tif");
            ResizeResult((int)outputPictureWidth, (int)outputPictureHeight);
        }

        private bool getCanBeLine(int index)
        {
            if (!lineData.GetCenterPosition())
            {
                if (((index + pictureCount - getAddWidthForLineAndIndentLeft() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                {                   
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if ((index % pictureCount) == (pictureCount/2 - lineData.GetLineThickness()/2))
                    return true;
                else
                    return false;
            }
        }

        private void drawLineLeft()
        {
            int colorValue;

            if (lineData.GetLeft())
            {
                for (int i = 0; i < this.getAddWidthForLineAndIndent(); i++)
                {
                    if (i < this.getAddWidthForLine() && getCanBeLine(i))
                    {
                        colorValue = lineData.GetLineColor().ToArgb();
                    }
                    else
                    {
                        colorValue = lineData.GetBackgroundColor().ToArgb();
                    }

                    for (int j = 0; j < result.GetHeight(); j++)
                    {
                        result.SetPixel(i, j, colorValue);
                    }

                }
            } 
        }
        private void drawLineRight()
        {
            int colorValue = 0;

            if (lineData.GetRight())
            {
                for (int i = result.GetWidth(); i >= (preResamplePictureWidth + this.getAddWidthForLineAndIndentLeft()); i--)
                {
                    if (i > (result.GetWidth() - this.getAddWidthForLine()) && getCanBeLine(i))
                    {
                        colorValue = lineData.GetLineColor().ToArgb();
                    }
                    else
                    {
                        colorValue = lineData.GetBackgroundColor().ToArgb();
                    }

                    for (int j = 0; j < result.GetHeight(); j++)
                    {
                        result.SetPixel(i, j, colorValue);
                    }

                }
            }
        }

        private void drawLineTop(){
            
            int colorValue = 0;

            if (lineData.GetTop())
                {
                    int pomLeft = 0;
                    int pomRight = 0;
                    if (lineData.GetLeft())
                    {
                        pomLeft = this.getAddWidthForLine();
                    }
                     if (lineData.GetRight())
                    {
                        pomRight = this.getAddWidthForLine();
                    }
                     for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)
                    {
                        if (getCanBeLine(i))
                        {
                            colorValue = lineData.GetLineColor().ToArgb();
                        }
                        else
                        {
                            colorValue = lineData.GetBackgroundColor().ToArgb();
                        }

                        for (int j = 0; j < getAddHeightForLineTop(); j++)
                        {
                            result.SetPixel(i, j, colorValue);
                        }

                        colorValue = lineData.GetBackgroundColor().ToArgb();

                        for (int j = getAddHeightForLineTop(); j < getAddHeightForLineAndIndent(); j++)
                        {
                            result.SetPixel(i, j, colorValue);
                        }

                    }
                }
        }

        private void drawLineBottom()
        {
            int colorValue = 0;

            if (lineData.GetBottom())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                {
                    pomLeft = this.getAddWidthForLine();
                }
                if (lineData.GetRight())
                {
                    pomRight = this.getAddWidthForLine();
                }

                for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)
                {
                    if (getCanBeLine(i))
                    {
                        colorValue = lineData.GetLineColor().ToArgb();
                    }
                    else
                    {
                        colorValue = lineData.GetBackgroundColor().ToArgb();
                    }

                    for (int j = result.GetHeight() - 1; j > (result.GetHeight() - getAddHeightForLineTop()); j--)
                    {
                        result.SetPixel(i, j, colorValue);
                    }

                    colorValue = lineData.GetBackgroundColor().ToArgb();

                    for (int j = (result.GetHeight() - getAddHeightForLineTop()); j >= (result.GetHeight() - getAddHeightForLineAndIndent()); j--)
                    {
                        result.SetPixel(i, j, colorValue);
                    }

                }
            }
        }

        private void drawLine()
        {
            if (lineData != null)
            {
                drawLineLeft();
                drawLineRight();
                drawLineTop();
                drawLineBottom();
            }
        }

        public void ResizeResult(int pxWidth, int pxHeight)
        {
            if (lineData == null)
                result.Resize(pxWidth, pxHeight, interlacingData.GetFinalResampleFilter());
            else
            {

                if (lineData.GetTop())
                {
                    pxHeight += (int)((lineData.GetFrameWidth() + lineData.GetIndent()) * interlacingData.GetPictureResolution());
                }

                if (lineData.GetBottom())
                {
                    pxHeight += (int)((lineData.GetFrameWidth() + lineData.GetIndent()) * interlacingData.GetPictureResolution());
                }

                if (lineData.GetLeft())
                {
                    pxWidth += (int)((lineData.GetFrameWidth() + lineData.GetIndent()) * interlacingData.GetPictureResolution());
                }

                if (lineData.GetRight())
                {
                    pxWidth += (int)((lineData.GetFrameWidth() + lineData.GetIndent())* interlacingData.GetPictureResolution());
                }

                result.Resize(pxWidth, pxHeight, interlacingData.GetFinalResampleFilter());
                //System.Windows.Forms.MessageBox.Show("" + lineData.GetFrameWidth() * interlacingData.GetPictureResolution());
                
            }
        }

        public Picture GetResult()
        {
            return result;
        }
    }
}
