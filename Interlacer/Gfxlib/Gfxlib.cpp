#include "stdafx.h"

#pragma warning(disable:4251)  //ignorovani varovani o exportu STL trid - tyto tridy mimo toto DLL nebudou pouzity

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
	Magick::ResourceLimits::disk(0);
#else
	Magick::ResourceLimits::memory(4294967296);
	Magick::ResourceLimits::disk(0);
#endif
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
		img = new Magick::Image;
		img->read(filename);
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

void* pingImage(char* filename)
{
	Magick::Image* img = nullptr;
	try
	{
		img = new Magick::Image;
		img->ping(filename);
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

double getImageXDpi(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->xResolution();
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();
	switch (resType)
	{
		case MagickCore::ResolutionType::PixelsPerInchResolution:
			return resolution;
		case MagickCore::ResolutionType::PixelsPerCentimeterResolution:
			return (resolution * 2.54);
		default:
			return 0;
	}
}

double getImageYDpi(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->yResolution();
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();
	switch (resType)
	{
	case MagickCore::ResolutionType::PixelsPerInchResolution:
		return resolution;
	case MagickCore::ResolutionType::PixelsPerCentimeterResolution:
		return (resolution * 2.54);
	default:
		return 0;
	}
}

void setImageDpi(void* ptr, double xDpi, double yDpi)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	imagePtr->resolutionUnits(MagickCore::ResolutionType::PixelsPerInchResolution);
	imagePtr->image()->x_resolution = xDpi;
	imagePtr->image()->y_resolution = yDpi;
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