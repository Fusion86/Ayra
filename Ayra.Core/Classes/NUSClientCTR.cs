﻿using Ayra.Core.Models.CTR;
using System;

namespace Ayra.Core.Classes
{
    public class NUSClientCTR : NUSClient
    {
        protected override string nusBaseUrl => "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";
        protected override string nusUserAgent => "wii libnup/1.0";
        protected override Type TMDType => typeof(TMD);
    }
}
