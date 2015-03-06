#ifndef _GFXLIB_H
#define _GFXLIB_H
#include <iostream>

extern "C"
{
	/*definuje typ podle velikosti barevneho kanalu (INT32 - 8-bitovy kanal / INT64 - 16-bitovy kanal)
	pri zmene je potreba prepsat hodnotu makra MAGICKCORE_QUANTUM_DEPTH v souboru magick-baseconfig.h*/
	typedef INT32 INTPX;

	__declspec(dllexport) void setResourceLimits();
	__declspec(dllexport) void* createImage(int w, int h);
	__declspec(dllexport) void deleteImage(void*);
	__declspec(dllexport) INTPX* getPixelDataPtr(void*);
	__declspec(dllexport) void* loadImage(char*);
	__declspec(dllexport) void* pingImage(char*);
	__declspec(dllexport) void saveImage(void*, char*);
	__declspec(dllexport) int getImageWidth(void*);
	__declspec(dllexport) int getImageHeight(void*);
	__declspec(dllexport) double getImageXDpi(void*);
	__declspec(dllexport) double getImageYDpi(void*);
	__declspec(dllexport) void setImageDpi(void*, double, double);
	__declspec(dllexport) void resizeImage(void*, int, int, int);
}

#endif