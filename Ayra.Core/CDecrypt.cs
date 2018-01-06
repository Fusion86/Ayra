using Ayra.Core.Models;
using System;
using System.Diagnostics;

namespace Ayra.Core
{
    public static class CDecrypt
    {
        public static void DecryptContents(TMD tmd, string titleKey, string path)
        {
            Debug.WriteLine("[DecryptContents] Title version: " + tmd.Header.TitleVersion);
            Debug.WriteLine("[DecryptContents] TMD version: " + tmd.Header.Version);
            Debug.WriteLine("[DecryptContents] Content count: " + tmd.Header.NumContents);

            if (tmd.Header.Version != 1) throw new NotSupportedException();
        }
    }
}
