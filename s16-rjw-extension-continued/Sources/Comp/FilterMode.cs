using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s16_extension
{
    public enum FilterMode : byte
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals,
        Excludes,
    }
}
