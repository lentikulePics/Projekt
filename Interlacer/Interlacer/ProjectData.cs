using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    class ProjectData
    {
        private InterlacingData interlacingData = new InterlacingData();
        private LineData lineData = new LineData();

        public InterlacingData GetInterlacingData()
        {
            return interlacingData;
        }

        public LineData GetLineData()
        {
            return lineData;
        }
    }
}
