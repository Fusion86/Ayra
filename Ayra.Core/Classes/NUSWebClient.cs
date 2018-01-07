using System;
using System.Net;

namespace Ayra.Core.Classes
{
    public class NUSWebClient : WebClient
    {
        public string UserAgent = "";

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.UserAgent = UserAgent;

            return request;
        }
    }
}
