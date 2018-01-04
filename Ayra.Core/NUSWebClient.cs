using System;
using System.Net;

namespace Ayra.Core
{
    public class NUSWebClient : WebClient
    {
        private readonly string _userAgent;

        public NUSWebClient(string userAgent)
        {
            _userAgent = userAgent;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.UserAgent = _userAgent;

            return request;
        }
    }
}
