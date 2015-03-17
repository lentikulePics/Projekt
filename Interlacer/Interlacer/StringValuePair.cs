using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    class StringValuePair<ValueType>
    {
        public readonly String name;
        public readonly ValueType value;

        StringValuePair(String name, ValueType value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
