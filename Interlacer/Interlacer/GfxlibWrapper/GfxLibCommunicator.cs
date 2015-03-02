using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
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
        public static extern Int64* getPixelDataPtr(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* loadImage(char[] filename);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void saveImage(void* ptr, char[] filename);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageWidth(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageHeight(void* ptr);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void resizeImage(void* ptr, int w, int h, int filterType);

        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getImageSizeWithoutLoading(char[] filename, int* width, int* height);
    }
}
