using Ayra.Core.Helpers;
using Ayra.Core.Models.WUP;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Ayra.Core.Classes
{
    public class NUSClientWiiU : NUSClient
    {
        protected override string nusBaseUrl => "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        protected override string nusUserAgent => "wii libnup/1.0";
        protected override Type TMDType => typeof(TMD);
    }
}
