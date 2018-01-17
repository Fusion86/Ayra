using Ayra.Core.Models.N3DS;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace Ayra.Core.Classes
{
    public class NUSClientN3DS : NUSClient
    {
        protected override string nusBaseUrl => "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        protected override string nusUserAgent => "wii libnup/1.0";
        protected override Type TMDType => typeof(TMD);

        protected override async Task DownloadContent(dynamic tmd, string outDir, int i)
        {
            throw new NotImplementedException();
        }
    }
}
