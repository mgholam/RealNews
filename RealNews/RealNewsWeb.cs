using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RealNews
{
    class RealNewsWeb : CoreWebServer
    {
        public RealNewsWeb(
            int HttpPort

            ) : base(HttpPort, true, true, AuthenticationSchemes.Anonymous, "api", "main.html")
        {
        }

        public override void InitializeCommandHandler(Dictionary<string, Handler> handler, Dictionary<string, string> apihelp)
        {
            handler.Add("hello", ctx =>
            {
                WriteResponse(ctx, 200, "hello");
            });

            handler.Add("image", ctx =>
            {
                string gstr = ctx.Request.Url.GetComponents(UriComponents.Query, UriFormat.Unescaped);
                var b = Properties.Resources.notfound;
                WriteResponse(ctx, 200, b, false);
                //WriteResponse(ctx, 404, "");
            });
        }

        public override void ServeFile(HttpListenerContext ctx, string path)
        {
            if (path == "style.css") {
                OutPutContentType(ctx, ".css");
                WriteResponse(ctx, 200, File.ReadAllText("configs\\style.css"));
            }
            else
                base.ServeFile(ctx, path);
        }
    }
}
