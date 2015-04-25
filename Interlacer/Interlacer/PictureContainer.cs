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
    public class PictureContainer
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
        /// <summary>y
        /// vyska samotneho obrazku bez ramecku pred poslednim resizem
        /// </summary>
        private int preResamplePictureHeight = -1;
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
        /// instance progress baru, ktery bude pouzit pro zobrazeni postupu prokladani
        /// </summary>
        private ProgressBar progressBar;

        /// <summary>
        /// konstruktor, ktery z Listu cest k souborum vytvori List indexu
        /// </summary>
        /// <param name="pictures">seznam obrazku, bud nenactenych s nastavenou cestou k souboru nebo vytvorenych programove</param>
        /// <param name="interlacingData">data potrebna k prolozeni</param>
        /// <param name="lineData">data potrebna k pridani pasovacich znacek</param>
        /// <param name="progressBar">instance progress baru, ktery bude pouzit pro zobrazeni postupu prokladani</param>
        public PictureContainer(List<Picture> pictures, InterlacingData interlacingData, LineData lineData, ProgressBar progressBar = null)
        {
            pictureCount = pictures.Count;
            this.interlacingData = interlacingData;
            this.lineData = lineData;
            this.progressBar = progressBar;
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

        private void resetProgressBar()
        {
            if (progressBar != null)
            {
                progressBar.Maximum = pictureCount + 3;
                progressBar.Step = 1;
                progressBar.Value = 0;
            }
        }

        private void makeProgressBarStep()
        {
            if (progressBar != null)
            {
                progressBar.PerformStep();
                Application.DoEvents();
                progressBar.Refresh();
            }
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

        /// <summary>
        /// Vrátí prázdný obrázek, pokud jsou nastaveny pasovací značky připočte jejich šířku či výšku
        /// </summary>
        /// <returns>Vrátí prázdný obrázek, pokud jsou nastaveny pasovací značky připočte jejich šířku či výšku</returns>
        private Picture getPictureForMarkToLine()
        {
  
            int addWidth = 0;
            int addHeight = 0;
            if (lineData == null)
                return new Picture(preResamplePictureWidth, preResamplePictureHeight);
            
            if (lineData.GetLeft())
                addWidth = getAddWidthForLineAndIndent();
            if (lineData.GetRight())
                addWidth = addWidth + getAddWidthForLineAndIndent();
            if (lineData.GetTop())
                addHeight = getAddHeightForLineAndIndent();
            if (lineData.GetBottom())
                addHeight = addHeight + getAddHeightForLineAndIndent();

            return new Picture(preResamplePictureWidth + addWidth, preResamplePictureHeight + addHeight);
        }

        /// <summary>
        ///  Vrací šířku rámečku s odsazením v px pro přidání před finálním resizem vydělený poměrem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineAndIndentRatio(double ratio)
        {
            double widthFL = lineData.GetFrameInchWidth() + lineData.GetInchIndent();
            double widthFLpixel = widthFL * interlacingData.GetDPI();
            return (int)(widthFLpixel / ratio);
        }

        /// <summary>
        ///  Vrací šířku rámečku bez odsazením v px pro pridani pred finalnim resizem vydeleny pomerem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineRatio(double ratio)
        {
            double widthFL = lineData.GetFrameInchWidth();
            double widthFLpixel = widthFL * interlacingData.GetDPI();
            return (int)(widthFLpixel / ratio);
        }


        /// <summary>
        ///  Vrací šířku postraního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLineAndIndent()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineAndIndentRatio(ratio);
        }

        /// <summary>
        ///  Vrací šířku postraního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLineAndIndentLeft()
        {
            if (lineData.GetLeft())
                return this.getAddWidthForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        /// Vrací výšku horního nebo spodního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineAndIndent()
        {
            double ratio = outputPictureHeight / (double)(preResamplePictureHeight);
            return getAddSizeForLineAndIndentRatio(ratio);
        }

        /// <summary>
        ///  Vrací výšku horního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineAndIndentTop()
        {
            if (lineData.GetTop())
                return this.getAddHeightForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        ///  Vrací výšku čar horního rámečku bez osazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineTop()
        {
            double ratio = outputPictureHeight / (double)(preResamplePictureHeight);
            return getAddSizeForLineRatio(ratio);
        }
        /// <summary>
        ///  Vrací šířku čar na stranách bez odsazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLine()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineRatio(ratio);
        }

        public void Interlace()
        {
            switch (interlacingData.GetDirection())
            {
                case Direction.Vertical:
                    InterlaceV();
                    break;
                case Direction.Horizontal:
                    InterlaceH();
                    break;
            }
            double pictureResolution = interlacingData.GetPictureResolution();
            switch (interlacingData.GetResolutionUnits())
            {
                case Units.In:
                    result.SetDpi(pictureResolution, pictureResolution);
                    break;
                case Units.Cm:
                    result.SetDpcm(pictureResolution, pictureResolution);
                    break;
                case Units.Mm:
                    result.SetDpcm(pictureResolution * 10, pictureResolution * 10);
                    break;
            }
        }

        private void InterlaceV()
        {
            if (inputPictureHeight < 0 || inputPictureWidth < 0)
                throw new InvalidOperationException("CheckPictures was not called");
            int lenses = (int)(interlacingData.GetInchWidth() * interlacingData.GetLPI());
            double lensesError = (interlacingData.GetInchWidth() * interlacingData.GetLPI() - lenses) * pictureCount;
            double errorRatio = lensesError / ((lenses * pictureCount) + lensesError);
            outputPictureWidth = interlacingData.GetInchWidth() * interlacingData.GetDPI();
            double finalError = outputPictureWidth * errorRatio;
            outputPictureWidth -= finalError;
            outputPictureHeight = interlacingData.GetInchHeight() * interlacingData.GetDPI();
            preResamplePictureWidth = lenses * pictureCount;
            preResamplePictureHeight = inputPictureHeight;
            resetProgressBar();
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
                currentPic.Resize(lenses, preResamplePictureHeight, interlacingData.GetInitialResizeFilter());
                for (int j = 0; j < indexes.Count; j++)
                {
                    for (int k = 0; k < lenses; k++)
                    {
                        int column = k * pictureCount + (pictureCount - 1 - indexes[j]) + this.getAddWidthForLineAndIndentLeft();
                        result.CopyColumn(currentPic, k, column, this.getAddHeightForLineAndIndentTop());
                    }
                    makeProgressBarStep();
                }
                if (loadPic)
                    currentPic.Destroy();
            }
            drawLines();
            makeProgressBarStep();
            ResizeResultV();
            makeProgressBarStep();
        }

        private void InterlaceH()
        {
            if (inputPictureHeight < 0 || inputPictureWidth < 0)
                throw new InvalidOperationException("CheckPictures was not called");
            int lenses = (int)(interlacingData.GetInchHeight() * interlacingData.GetLPI());
            double lensesError = (interlacingData.GetInchHeight() * interlacingData.GetLPI() - lenses) * pictureCount;
            double errorRatio = lensesError / ((lenses * pictureCount) + lensesError);
            outputPictureHeight = interlacingData.GetInchHeight() * interlacingData.GetDPI();
            double finalError = outputPictureHeight * errorRatio;
            outputPictureHeight -= finalError;
            outputPictureWidth = interlacingData.GetInchWidth() * interlacingData.GetDPI();
            preResamplePictureHeight = lenses * pictureCount;
            preResamplePictureWidth = inputPictureWidth;
            resetProgressBar();
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
                currentPic.Resize(preResamplePictureWidth, lenses, interlacingData.GetInitialResizeFilter());
                for (int j = 0; j < indexes.Count; j++)
                {
                    for (int k = 0; k < lenses; k++)
                    {
                        int row = k * pictureCount + (pictureCount - 1 - indexes[j]) + this.getAddHeightForLineAndIndentTop();
                        result.CopyRow(currentPic, k, row, this.getAddWidthForLineAndIndentLeft());
                    }
                    makeProgressBarStep();
                }
                if (loadPic)
                    currentPic.Destroy();
            }
            drawLines();
            makeProgressBarStep();
            ResizeResultH();
            makeProgressBarStep();
        }

        /// <summary>
        /// Zjistí zda na indexu sloupečku může být vykreslena pasovací značka
        /// </summary>
        /// <param name="index">index sloupečku obrázku</param>
        /// <returns>Vratí true pokud na daném indexu může být vykreslena značka</returns>
        private bool getCanBeLineV(int index)
        {
            if (!lineData.GetCenterPosition())
            {
                if (((index + 2*pictureCount - getAddWidthForLineAndIndentLeft() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                {                   
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (((index + 2*pictureCount - (pictureCount / 2 - lineData.GetLineThickness() / 2)  - getAddWidthForLineAndIndentLeft() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Zjistí zda na indexu řádku může být vykreslena pasovací značka
        /// </summary>
        /// <param name="index">index řádku obrázku</param>
        /// <returns>Vratí true pokud na daném indexu může být vykreslena značka</returns>
        private bool getCanBeLineH(int index)
        {
            if (!lineData.GetCenterPosition())
            {
                if (((index + 2*pictureCount - getAddHeightForLineAndIndentTop() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (((index + 2*pictureCount - (pictureCount / 2 - lineData.GetLineThickness() / 2) - getAddHeightForLineAndIndentTop() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Vykreslí vertikální pasovací značky na levé straně obrázku
        /// </summary>
        private void drawLinesLeftV()
        {
            int colorValue;
            if (lineData.GetLeft())
            {
           
                for (int i = 0; i < this.getAddWidthForLineAndIndent(); i++)    // začínám kreslit od zleva do prava
                {
                    if (i < this.getAddWidthForLine())
                    {
                        if (getCanBeLineV(i))
                            colorValue = lineData.GetLineColor().ToArgb();
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();
                    }
                    else
                        colorValue = Color.White.ToArgb();
                    for (int j = 0; j < result.GetHeight(); j++) {
                        result.SetPixel(i, j, colorValue);
                    }
                        
                       
                }
            } 
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky na pravé straně obrázku
        /// </summary>
        private void drawLinesRightV()
        {
            int colorValue = 0;
            if (lineData.GetRight())
            {
                for (int i = result.GetWidth()-1; i >= (preResamplePictureWidth + this.getAddWidthForLineAndIndentLeft()); i--) // začínám kreslit z prava do leva
                {
                    if (i >= (result.GetWidth() - this.getAddWidthForLine()))
                    {
                        if (getCanBeLineV(i))
                            colorValue = lineData.GetLineColor().ToArgb();
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();
                    }
                    else
                        colorValue = Color.White.ToArgb();
                    for (int j = 0; j < result.GetHeight(); j++)
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky nahoře obrázku
        /// </summary>
        private void drawLinesTopV(){
            int colorValue = 0;
            if (lineData.GetTop())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud jsou na levé straně už pasovací značky nekreslím je znova
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud jsou na pravé straně už pasovací značky nekreslím je znova

                for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)    // začínám kreslit z leva do prava
                {
                    if (getCanBeLineV(i))
                        colorValue = lineData.GetLineColor().ToArgb();
                    else
                        colorValue = lineData.GetBackgroundColor().ToArgb();
                    for (int j = 0; j <= getAddHeightForLineTop(); j++)
                        result.SetPixel(i, j, colorValue);
                    colorValue = Color.White.ToArgb();
                    for (int j = getAddHeightForLineTop()+1; j < getAddHeightForLineAndIndent(); j++)
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky v dolní části obrázku
        /// </summary>
        private void drawLinesBottomV()
        {
            int colorValue = 0;
            if (lineData.GetBottom())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud jsou na levé straně už pasovací značky nekreslím je znova
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud jsou na pravé straně už pasovací značky nekreslím je znova
                for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)
                {
                    if (getCanBeLineV(i))
                        colorValue = lineData.GetLineColor().ToArgb();
                    else
                        colorValue = lineData.GetBackgroundColor().ToArgb();
                    for (int j = result.GetHeight() - 1; j >= (result.GetHeight() - getAddHeightForLineTop()); j--)
                        result.SetPixel(i, j, colorValue);
                    colorValue = Color.White.ToArgb();
                    for (int j = (result.GetHeight() - getAddHeightForLineTop()-1); j >= (result.GetHeight() - getAddHeightForLineAndIndent()); j--)
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky na levé straně obrázku
        /// </summary>
        private void drawLinesLeftH()
        {
            int colorValue;
            if (lineData.GetLeft())
            {

                for (int i = 0; i < result.GetHeight(); i++)
                {
                    for (int j = 0; j < this.getAddWidthForLineAndIndent(); j++) // kreslí se z leva do prava
                    {
                        if (j < this.getAddWidthForLine()) {
                            if (getCanBeLineH(i))
                                colorValue = lineData.GetLineColor().ToArgb();
                            else
                                colorValue = lineData.GetBackgroundColor().ToArgb();
                            }
                        else
                            colorValue = Color.White.ToArgb();
                        result.SetPixel(j, i, colorValue);
                    }  
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky na pravé straně obrázku
        /// </summary>
        private void drawLinesRightH()
        {
            int colorValue = 0;
            if (lineData.GetRight())
            {
                for (int i = 0; i < result.GetHeight(); i++)
                {
                    for (int j = result.GetWidth()-1; j >= (preResamplePictureWidth + this.getAddWidthForLineAndIndentLeft()); j--) // kreslí se z prava do leva
                    {
                        if (j >= (result.GetWidth() - this.getAddWidthForLine()))
                        {
                            if (getCanBeLineH(i))
                                colorValue = lineData.GetLineColor().ToArgb();
                            else
                                colorValue = lineData.GetBackgroundColor().ToArgb();
                        }
                        else
                            colorValue = Color.White.ToArgb();
                        result.SetPixel(j, i, colorValue);
                    }
                        
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky v horní části obrázku
        /// </summary>
        private void drawLinesTopH()
        {
            int colorValue = 0;
            if (lineData.GetTop())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud je na levé straně pasovací značky nekreslím znova
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud je na pravé straně pasovací značky nekreslím znova
                for (int i = 0; i < getAddHeightForLineAndIndentTop(); i++)
                {
                     if (i < getAddHeightForLineTop())
                     {
                         if (getCanBeLineH(i))
                             colorValue = lineData.GetLineColor().ToArgb();
                         else
                             colorValue = lineData.GetBackgroundColor().ToArgb();
                     }
                     else
                         colorValue = Color.White.ToArgb();
                    
                     for (int j = pomLeft; j < result.GetWidth() - pomRight; j++){
                          result.SetPixel(j, i, colorValue);
                     }
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky v dolní části obrázku
        /// </summary>
        private void drawLinesBottomH()
        {
            int colorValue = 0;
            if (lineData.GetBottom())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud je na levé straně pasovací značky nekreslím znova
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud je na pravé straně pasovací značky nekreslím znova
                for (int i = result.GetHeight() - 1; i >= (result.GetHeight() - getAddHeightForLineAndIndent()); i--)
                {
                    if (i > (result.GetHeight() - getAddHeightForLineTop())) {
                        if (getCanBeLineH(i))
                            colorValue = lineData.GetLineColor().ToArgb();
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();
                    }
                    else
                        colorValue = Color.White.ToArgb();
                    for (int j = pomLeft; j < result.GetWidth() - pomRight; j++)
                        result.SetPixel(j, i, colorValue);
                }
            }
        }

        /// <summary>
        /// Vykreslí pasovací značky, pokud někde mají být, zavoláním dalších metod
        /// </summary>
        private void drawLines()
        {
            if (lineData != null)
            {
                switch (interlacingData.GetDirection())
                {
                    case Direction.Vertical:
                        drawLinesLeftV();
                        drawLinesRightV();
                        drawLinesTopV();
                        drawLinesBottomV();
                        break;
                    case Direction.Horizontal:
                        drawLinesLeftH();
                        drawLinesRightH();
                        drawLinesTopH();
                        drawLinesBottomH();
                        break;
                }
       
                
            }
        }

        private void ResizeResultV()
        {
            int outputHeight = (int)outputPictureHeight;
            if (lineData != null)
            {
                if (lineData.GetTop())
                    outputHeight += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
                if (lineData.GetBottom())
                    outputHeight += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
            }
            int originalWidth = (int)(interlacingData.GetInchWidth() * interlacingData.GetLPI()) * pictureCount;
            double widthRatio = outputPictureWidth / originalWidth;
            int outputWidth = (int)(result.GetWidth() * widthRatio);
            result.Resize(outputWidth, result.GetHeight(), interlacingData.GetFinalResampleFilter());
            makeProgressBarStep();
            result.Resize(outputWidth, outputHeight,
                interlacingData.GetInitialResizeFilter() == FilterType.None ? FilterType.None : FilterType.Triangle);
        }

        private void ResizeResultH()
        {
            int outputWidth = (int)outputPictureWidth;
            if (lineData != null)
            {
                if (lineData.GetLeft())
                    outputWidth += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
                if (lineData.GetRight())
                    outputWidth += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
            }
            int originalHeight = (int)(interlacingData.GetInchHeight() * interlacingData.GetLPI()) * pictureCount;
            double heightRatio = outputPictureHeight / originalHeight;
            int outputHeight = (int)(result.GetHeight() * heightRatio);
            result.Resize(result.GetWidth(), outputHeight, interlacingData.GetFinalResampleFilter());
            makeProgressBarStep();
            result.Resize(outputWidth, outputHeight,
                interlacingData.GetInitialResizeFilter() == FilterType.None ? FilterType.None : FilterType.Triangle);
        }

        public Picture GetResult()
        {
            return result;
        }
    }
}
