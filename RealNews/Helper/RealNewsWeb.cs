using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace RealNews
{
    class RealNewsWeb : CoreWebServer
    {
        public RealNewsWeb(
            int HttpPort,
            ImageCache imgcache,
            Func<string> show
            ) : base(HttpPort, true, true, AuthenticationSchemes.Anonymous, "api", "main.html")
        {
            _imgcache = imgcache;
            _show = show;
        }
        private Func<string> _show;
        private ImageCache _imgcache;

        public override void InitializeCommandHandler(Dictionary<string, Handler> handler, Dictionary<string, string> apihelp)
        {
            handler.Add("show", ctx =>
            {
                OutPutContentType(ctx, ".html");
                WriteResponse(ctx, 200, _show());
            });

            handler.Add("image", ctx =>
            {
                string gstr = ctx.Request.Url.GetComponents(UriComponents.Query, UriFormat.Unescaped);
                byte[] o = Properties.Resources.notfound;
                if (_imgcache.Contains(gstr))
                {
                    o = _imgcache.Get(gstr);
                    if (o == null)
                        o = Properties.Resources.notfound;
                }
                WriteResponse(ctx, 200, o, false);
            });
        }

        public override void ServeFile(HttpListenerContext ctx, string path)
        {
            if (path == "style.css")
            {
                OutPutContentType(ctx, ".css");
                WriteResponse(ctx, 200, File.ReadAllText("configs\\style.css"));
            }
            else if (path == "api/star.png")
            {
                OutPutContentType(ctx, ".png");
                var ms = new MemoryStream();
                if (Settings.DarkMode)
                    Properties.Resources.star1.Save(ms, ImageFormat.Png);
                else
                    Properties.Resources.Star.Save(ms, ImageFormat.Png);
                WriteResponse(ctx, 200, ms.ToArray(), false);
            }
            else
                base.ServeFile(ctx, path);
        }
    }
}
