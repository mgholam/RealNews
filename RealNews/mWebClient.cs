using System;
using System.Net;

namespace RealNews
{
    public class mWebClient : WebClient
    {
        public mWebClient()
        {
            Timeout = 10 * 1000;
            this.Encoding = System.Text.Encoding.UTF8;
            if (Settings.UseSytemProxy)
                Proxy = WebRequest.DefaultWebProxy;
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
