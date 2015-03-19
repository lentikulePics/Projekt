#include "stdafx.h"

#include "Gfxlib.h"
#include "GfxlibErrors.h"
#include <iostream>
#include <fstream>
#include <string>
#include <Magick++.h>

void writeError(std::string str)
{
	std::ofstream file("MagickError.txt");
	file << str;
	file.close();
}

void setResourceLimits()
{
#if defined _WIN64
	Magick::ResourceLimits::memory(68719476736);
#else
	Magick::ResourceLimits::memory(4294967296);
#endif
	Magick::ResourceLimits::disk(0);
}

void* createImage(int w, int h)
{
	std::string wStr = std::to_string(w);
	std::string hStr = std::to_string(h);
	std::string sizeStr = wStr + std::string("x") + hStr;
	Magick::Image* img = nullptr;
	try
	{
		img = new Magick::Image;
		img->size(sizeStr);
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		delete img;
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	catch (...)
	{
		delete img;
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	return (void*)img;
}

void deleteImage(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	delete imagePtr;
}

INTPX* getPixelDataPtr(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	INTPX* pxPtr = nullptr;
	try
	{
		pxPtr = (INTPX*)imagePtr->getPixels(0, 0, imagePtr->columns(), imagePtr->rows());
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	catch (...)
	{
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	return pxPtr;
}

void* loadImage(char* filename)
{
	Magick::Image* img = nullptr;
	try
	{
		std::ifstream file(filename, std::ios::binary);
		file.seekg(0, std::ios::end);
		long length = (long)file.tellg();
		file.seekg(0, std::ios::beg);
		char* data = new char[length];
		file.read(data, length);
		file.close();

		img = new Magick::Image;
		img->read(Magick::Blob(data, length));
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		delete img;
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	catch (...)
	{
		delete img;
		writeError("...");
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	return (void*)img;
}

void* pingImage(char* filename)
{
	Magick::Image* img = nullptr;
	try
	{
		std::ifstream file(filename);
		file.seekg(0, std::ios::end);
		long length = (long)file.tellg();
		file.seekg(0, std::ios::beg);
		char* data = new char[length];
		file.read(data, length);
		file.close();
		writeError(std::to_string(length));
		img = new Magick::Image;
		img->ping(Magick::Blob(data, length));
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		delete img;
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	catch (...)
	{
		delete img;
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	return (void*)img;
}

void saveImage(void* ptr, char* filename)
{
	try
	{
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();
		imagePtr->write(filename);
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureSaveFailure, 0, 0, 0);
	}
	catch (...)
	{
		RaiseException(GfxlibErrors::PictureSaveFailure, 0, 0, 0);
	}
}

int getImageWidth(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	return (int)imagePtr->columns();
}

int getImageHeight(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	return (int)imagePtr->rows();
}

double getImageXResolution(void* ptr, int* unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->xResolution();
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();
	switch (resType)
	{
		case MagickCore::ResolutionType::PixelsPerInchResolution:
			*unitType = 1;
			return resolution;
		case MagickCore::ResolutionType::PixelsPerCentimeterResolution:
			*unitType = 2;
			return resolution;
		default:
			return 0;
	}
}

double getImageYResolution(void* ptr, int* unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->yResolution();
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();
	switch (resType)
	{
	case MagickCore::ResolutionType::PixelsPerInchResolution:
		*unitType = 1;
		return resolution;
	case MagickCore::ResolutionType::PixelsPerCentimeterResolution:
		*unitType = 2;
		return resolution;
	default:
		return 0;
	}
}

void setImageResolution(void* ptr, double xRes, double yRes, int unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	imagePtr->resolutionUnits(unitType == 1 ?
		MagickCore::ResolutionType::PixelsPerInchResolution : MagickCore::ResolutionType::PixelsPerCentimeterResolution);
	imagePtr->image()->x_resolution = xRes;
	imagePtr->image()->y_resolution = yRes;
}

void resizeImage(void* ptr, int w, int h, int filterType)
{
	try
	{
		std::string wStr = std::to_string(w);
		std::string hStr = std::to_string(h);
		std::string sizeStr = wStr + std::string("x") + hStr + std::string("!");
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();
		imagePtr->filterType((MagickCore::FilterTypes)filterType);
		imagePtr->resize(sizeStr);
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureResizeFilure, 0, 0, 0);
	}
	catch (...)
	{
		RaiseException(GfxlibErrors::PictureResizeFilure, 0, 0, 0);
	}
}

void clipImage(void* ptr, int x, int y, int w, int h)
{
	try
	{
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();
		imagePtr->crop(Magick::Geometry(w, h, x, y));
	}
	catch (Magick::Error & er)
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureClipFailure, 0, 0, 0);
	}
	catch (...)
	{
		RaiseException(GfxlibErrors::PictureClipFailure, 0, 0, 0);
	}
}