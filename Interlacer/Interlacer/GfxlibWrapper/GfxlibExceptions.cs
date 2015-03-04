using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    class PictureLoadFailureException : Exception { }
    class PictureSaveFailureException : Exception { }
    class PictureCreationFailureException : Exception { }
    class PictureResizeFailureException : Exception { }
    class PictureAlreadyCreatedException : Exception { }
}
