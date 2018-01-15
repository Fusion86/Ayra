using System;

namespace Ayra.Core.Enums
{
    [Flags]
    public enum NSoftwareRegion
    {
        NONE,

        USA,
        EUR,
        JPN,
        CHN,
        KOR,
        TWN,

        ALL, // Region Free, probably
    }
}
