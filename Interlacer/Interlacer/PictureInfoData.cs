using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    class PictureInfoData
    {
        private Dictionary<String, Picture> pictures = new Dictionary<String, Picture>();
        private Picture defaultImage;

        public Picture GetInfo(String path)
        {
            if (!pictures.ContainsKey(path))
            {
                Picture pic = new Picture(path);
                pic.Ping();
                pictures.Add(path, pic);
            }
            return pictures[path];
        }
    }
}
