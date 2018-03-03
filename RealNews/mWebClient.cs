using System;
using System.Net;

namespace RealNews
{
    public class mWebClient : WebClient
    {
        public mWebClient()
        {
            Timeout = 10 * 1000;
        }
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }
}
