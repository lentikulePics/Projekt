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

    class PictureCreationFailureException : Exception
    {
        public readonly int width;
        public readonly int height;

        public PictureCreationFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    class PictureResizeFailureException : Exception 
    {
        public readonly int width;
        public readonly int height;

        public PictureResizeFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    class PictureAlreadyCreatedException : Exception { }

    class PictureClipFailureException : Exception
    {
        public readonly int width;
        public readonly int height;

        public PictureClipFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
