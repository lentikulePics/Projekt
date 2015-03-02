#ifndef _GFXLIB_H
#define _GFXLIB_H
#include <iostream>

extern "C"
{
	__declspec(dllexport) void setResourceLimits();
	__declspec(dllexport) void* createImage(int w, int h);
	__declspec(dllexport) void deleteImage(void*);
	__declspec(dllexport) INT64* getPixelDataPtr(void*);
	__declspec(dllexport) void* loadImage(char*);
	__declspec(dllexport) void saveImage(void*, char*);
	__declspec(dllexport) int getImageWidth(void*);
	__declspec(dllexport) int getImageHeight(void*);
	__declspec(dllexport) void resizeImage(void*, int, int, int);
	__declspec(dllexport) void getImageSizeWithoutLoading(char*, int*, int*);
}

#endif