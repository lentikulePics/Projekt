using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    class PictureLoadFailureException : Exception
    {
        public readonly String filename;

        public PictureLoadFailureException(String filename)
        {
            this.filename = filename;
        }
    }

    class PictureSaveFailureException : Exception
    {
        public readonly String filename;

        public PictureSaveFailureException(String filename)
        {
            this.filename = filename;
        }
    }

    abstract class PictureProcessException : Exception { }

    class PictureCreationFailureException : PictureProcessException
    {
        public readonly int width;
        public readonly int height;

        public PictureCreationFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    class PictureResizeFailureException : PictureProcessException 
    {
        public readonly int width;
        public readonly int height;

        public PictureResizeFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    class PictureClipFailureException : PictureProcessException
    {
        public readonly int width;
        public readonly int height;

        public PictureClipFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    class PictureWrongFormatException : Exception
    {
        public readonly String filename;

        public PictureWrongFormatException(String filename)
        {
            this.filename = filename;
        }
    }

    class PictureAlreadyCreatedException : Exception { }
}
