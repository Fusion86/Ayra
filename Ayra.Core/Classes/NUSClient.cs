namespace Ayra.Core.Classes
{
    public abstract class NUSClient
    {
        protected string nusBaseUrl;
        protected string nusUserAgent;

        protected NUSWebClient GetNewNUSWebClient()
        {
            return new NUSWebClient { UserAgent = nusUserAgent };
        }
    }
}
