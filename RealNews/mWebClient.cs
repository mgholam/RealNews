using System;
using System.Net;

namespace RealNews
{
    public class mWebClient : WebClient
    {
        public mWebClient()
        {
            Timeout = 10 * 1000;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// KLUDGE : https security for .net 4
            base.Encoding = System.Text.Encoding.UTF8;
            if (Settings.UseSytemProxy) // FEATURE : else define a proxy
                Proxy = WebRequest.DefaultWebProxy;
        }
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest request = base.GetWebRequest(uri);
            var http = request as HttpWebRequest;
            if (http != null)
            {
                http.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                http.ReadWriteTimeout = Timeout;
            }

            request.Timeout = Timeout;
            return request;
        }
    }
}
