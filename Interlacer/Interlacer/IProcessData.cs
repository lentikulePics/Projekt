using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    interface IProcessData
    {
        void SetUnits(Units units);
        Units GetUnits();
    }
        
}
