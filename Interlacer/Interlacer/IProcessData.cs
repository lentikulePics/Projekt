using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    public interface IProcessData
    {
        void SetUnits(Units units);
        Units GetUnits();
    }
}
