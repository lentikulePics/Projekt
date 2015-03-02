using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GfxlibWrapper.GfxlibExceptions;

namespace GfxlibWrapper
{
    unsafe class Picture
    {
        /// <summary>
        /// sirka v pixelech
        /// </summary>
        private int width = 0;
        /// <summary>
        /// vyska v pixelech
        /// </summary>
        private int height = 0;
        /// <summary>
        /// pole obsahujici informace o jednotlivych pixelech
        /// </summary>
        private Int64* pixelData = null;
        /// <summary>
        /// nazev souboru pro nacteni
        /// </summary>
        private String filename = null;
        /// <summary>
        /// ukazatel na objekt C++ tridy Magick::Image
        /// </summary>
        private void* imagePtr = null;

        /// <summary>
        /// trida reprezentujici typ filteru pouziteho pro resize
        /// </summary>
        public class FilterType
        {
            public static readonly FilterType None = new FilterType("Nearest neighbour", 1);
            public static readonly FilterType Triangle = new FilterType("Triangle", 3);
            public static readonly FilterType Cubic = new FilterType("Cubic", 10);
            public static readonly FilterType Lanczos = new FilterType("Lanczos", 22);

            private String name;
            public readonly int filterNum;

            private FilterType(String name, int value)
            {
                this.name = name;
                this.filterNum = value;
            }

            public override string ToString()
            {
                return name;
            }
        }

        /// <summary>
        /// prevede objekt tridy String na pole znaku a prida terminator
        /// </summary>
        /// <param name="str">objekt tridy String k prevedeni</param>
        /// <returns>pole charu ukoncene terminatorem</returns>
        private char[] stringToCharArray(String str)
        {
            return (str + '\0').ToCharArray();
        }

        /// <summary>
        /// nastavi pametove limity knihovny Magick++
        /// </summary>
        static Picture()
        {
            GfxlibCommunicator.setResourceLimits();
        }

        /// <summary>
        /// vytvori bitmapu o zadane velikosti
        /// </summary>
        /// <param name="width">sirka v pixelech</param>
        /// <param name="height">vyska v pixelech</param>
        public Picture(int width, int height)
        {
            this.width = width;
            this.height = height;
            try
            {
                imagePtr = GfxlibCommunicator.createImage(width, height);
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);
            }
            catch (SEHException)
            {
                Destroy();
                throw new PictureCreationFailureException();
            }
        }

        /// <summary>
        /// nastavi jmeno vstupniho souboru
        /// </summary>
        /// <param name="filename">jmeno vstupniho souboru</param>
        public Picture(String filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// zjisti z obrazku sirku a vysku v pixelech bez jeho kompletniho nacteni
        /// </summary>
        public void Ping()
        {
            if (imagePtr != null)
                throw new PictureAlreadyCreatedException();
            try
            {
                fixed (int* wPtr = &width, hPtr = &height)
                {
                    GfxlibCommunicator.getImageSizeWithoutLoading(stringToCharArray(filename), wPtr, hPtr);
                }
            }
            catch (SEHException)
            {
                throw new PictureLoadFailureException();
            }
        }

        /// <summary>
        /// nacte bitmapu ze zadaneho souboru
        /// </summary>
        public void Load()
        {
            if (imagePtr != null)
                throw new PictureAlreadyCreatedException();
            try
            {
                imagePtr = GfxlibCommunicator.loadImage(stringToCharArray(filename));
                width = GfxlibCommunicator.getImageWidth(imagePtr);
                height = GfxlibCommunicator.getImageHeight(imagePtr);
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();
                if (errorCode == (int)GfxlibErrors.PictureLoadFailure)
                    throw new PictureLoadFailureException();
                else
                {
                    Destroy();
                    throw new PictureCreationFailureException();
                }
            }
        }

        /// <summary>
        /// getter sirky bitmapy
        /// </summary>
        /// <returns>sirka bitmapy v pixelech</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetWidth()
        {
            return width;
        }

        /// <summary>
        /// getter vysky v pixelech
        /// </summary>
        /// <returns>vyska bitmapy v pixelech</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHeight()
        {
            return height;
        }

        /// <summary>
        /// ulozi bitmapu
        /// </summary>
        /// <param name="saveFilename">nazev vystupniho souboru</param>
        public void Save(String saveFilename)
        {
            try
            {
                GfxlibCommunicator.saveImage(imagePtr, stringToCharArray(saveFilename));
            }
            catch (SEHException)
            {
                throw new PictureSaveFailureException();
            }
        }

        /// <summary>
        /// nastavi hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <param name="pxValue">hodnota pixelu ve formatu ORGB, kde O je pruhlednost (opacity),
        /// cim je vetsi, tim je pixel pruhlednejsi, hodnota je 8 bytová - napr. 0x0000FFFF00000000 je cervena</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, Int64 pxValue)
        {
            pixelData[y * width + x] = pxValue;
        }

        /// <summary>
        /// nastavi hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <param name="red">hodnota cerveneho kanalu (0 - 65535)</param>
        /// <param name="green">hodnota zeleneho kanalu (0 - 65535)</param>
        /// <param name="blue">hodnota modreho kanalu (0 - 65535)</param>
        public void SetPixel(int x, int y, UInt16 red, UInt16 green, UInt16 blue)
        {
            UInt16* pxPtr = (UInt16*)&pixelData[y * width + x];
            pxPtr[0] = blue;
            pxPtr[1] = green;
            pxPtr[2] = red;
        }

        /// <summary>
        /// vrati hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <returns>hodnota pixelu na danych souradnicich</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int64 GetPixel(int x, int y)
        {
            return pixelData[y * width + x];
        }
        
        /// <summary>
        /// zmeni velikost bitmapy
        /// </summary>
        /// <param name="newWidth">nova sirka</param>
        /// <param name="newHeight">nova vyska</param>
        /// <param name="filterType">typ filteru</param>
        public void Resize(int newWidth, int newHeight, FilterType filterType)
        {
            try
            {
                GfxlibCommunicator.resizeImage(imagePtr, newWidth, newHeight, filterType.filterNum);
                width = newWidth;
                height = newHeight;
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();
                if (errorCode == (int)GfxlibErrors.PictureResizeFilure)
                    throw new PictureResizeFailureException();
                else
                    throw new PictureCreationFailureException();
            }

            /*void* oldImagePtr = imagePtr;
            imagePtr = createImage(newWidth, newHeight);
            Int64* newPixelData = getPixelDataPtr(imagePtr);
            double xRatio = (double)width / newWidth;
            double yRatio = (double)height / newHeight;
            for (int j = 0; j < newHeight; j++)
            {
                int newY = j * newWidth;
                int y = (int)(j * yRatio) * width;
                for (int i = 0; i < newWidth; i++)
                    newPixelData[newY + i] = pixelData[y + (int)(i * xRatio)];
            }
            width = newWidth;
            height = newHeight;
            pixelData = newPixelData;
            deleteImage(oldImagePtr);*/
        }

        /// <summary>
        /// znici obrazek a dealokuje pamet, kterou vyuziva
        /// </summary>
        public void Destroy()
        {
            if (imagePtr != null)
                GfxlibCommunicator.deleteImage(imagePtr);
            imagePtr = null;
            pixelData = null;
            width = 0;
            height = 0;
        }

        /// <summary>
        /// zkopiruje sloupec pixelu z jinoho obrazku do tohoto
        /// </summary>
        /// <param name="source">zdrojovy obrazek</param>
        /// <param name="sourceColumn">index zdrojoveho sloupce</param>
        /// <param name="destColumn">index ciloveho sloupce tohoto obrazku</param>
        /// <param name="destYBegin">index pocatecni y souradnice, od ktere se zacne sloupec kopirovat do tohoto obrazku</param>
        public void CopyColumn(Picture source, int sourceColumn, int destColumn, int destYBegin)
        {
            for (int i = 0; i < source.height; i++)
                SetPixel(destColumn, i + destYBegin, source.GetPixel(sourceColumn, i));
        }
    }
}
