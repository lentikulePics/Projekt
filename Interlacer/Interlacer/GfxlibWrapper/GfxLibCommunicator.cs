using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    //definuje typ podle velikosti barevneho kanalu (Int32 - 8-bitovy kanal / Int64 - 16-bitovy kanal)
    using IntPx = Int32;

    /// <summary>
    /// staticka trida obsahujici funkce pro komunikaci s grafickou knihovnou prostrednictvim DLL
    /// </summary>
    static unsafe class GfxlibCommunicator
    {
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setResourceLimits();

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* createImage(int w, int h);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void deleteImage(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPx* getPixelDataPtr(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* loadImage(char[] filename);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* pingImage(char[] filename);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void saveImage(void* ptr, char[] filename);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageWidth(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getImageXDpi(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getImageYDpi(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setImageDpi(void* ptr, double xDpi, double yDpi);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageHeight(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void resizeImage(void* ptr, int w, int h, int filterType);
    }
}
