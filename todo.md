# TODO

- tree folders and move items
- rename dll files
- about & version control
- write OPML
- dark UI
- zoom level settings
- window form font settings
- read OPML
- refine styling
- download images @time
- minimize to tray
- ​



# ?

- vpn code
- ~~html sanitizer~~
- image cache
- ~~get image size without downloading~~
- get image dim without downloading
- ~~use system proxy settings~~
- ~~read~~ write opml
- ~~styling~~ -> css style with `HtmlPanel`
- ~~replace img tags with call to cache~~ -> event handler in `HtmlPanel`
- ~~extract linked content as attachments~~



## proxy in c##

```c#
WebClient wc = new WebClient();
wc.Proxy = WebRequest.DefaultWebProxy;
var s = wc.DownloadString("https://www.twitter.com");
```

## image length without downloading

```c#
HttpWebRequest req = (HttpWebRequest)WebRequest.Create(e.Src);
req.Method = "HEAD";
long len;
using (HttpWebResponse resp = (HttpWebResponse)(req.GetResponse()))
{
    len = resp.ContentLength;
}
```