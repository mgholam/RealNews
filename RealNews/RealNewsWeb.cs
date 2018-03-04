using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RealNews
{
    class RealNewsWeb : CoreWebServer
    {
        public RealNewsWeb(
            int HttpPort,
            RaptorDB.Common.IKeyStoreHF imgcache,
            Func<string> show
            ) : base(HttpPort, true, true, AuthenticationSchemes.Anonymous, "api", "main.html")
        {
            _imgcache = imgcache;
            _show = show;
        }
        private Func<string> _show;
        private RaptorDB.Common.IKeyStoreHF _imgcache;

        public override void InitializeCommandHandler(Dictionary<string, Handler> handler, Dictionary<string, string> apihelp)
        {
            handler.Add("hello", ctx =>
            {
                WriteResponse(ctx, 200, "hello");
            });

            handler.Add("show", ctx =>
            {
                OutPutContentType(ctx, ".html");
                WriteResponse(ctx, 200, _show());
            });

            handler.Add("image", ctx =>
            {
                string gstr = ctx.Request.Url.GetComponents(UriComponents.Query, UriFormat.Unescaped);
                var o = _imgcache.GetObjectHF(gstr) as ImgCache;
                byte[] b = null;
                if (o == null)
                    b = Properties.Resources.notfound;
                else
                    b = o.data;
                WriteResponse(ctx, 200, b, false);
                //WriteResponse(ctx, 404, "");
            });
        }

        public override void ServeFile(HttpListenerContext ctx, string path)
        {
            if (path == "style.css")
            {
                OutPutContentType(ctx, ".css");
                WriteResponse(ctx, 200, File.ReadAllText("configs\\style.css"));
            }
            else
                base.ServeFile(ctx, path);
        }
    }
}
