namespace CodeHollow.FeedReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Parser;

    /// <summary>
    /// The static FeedReader class which allows to read feeds from a given url. Use it to
    /// parse a feed from an url <see cref="Read(string)"/>, a file <see cref="ReadFromFile(string)"/>
    /// or a string <see cref="ReadFromString(string)"/>. If the feed url is not known, <see cref="ParseFeedUrlsFromHtml(string)"/>
    /// returns all feed links on a given page.
    /// </summary>
    /// <example>
    /// var links = FeedReader.ParseFeedUrlsFromHtml("https://codehollow.com");
    /// var firstLink = links.First();
    /// var feed = FeedReader.Read(firstLink.Url);
    /// Console.WriteLine(feed.Title);
    /// </example>
    public static class FeedReader
    {
        /// <summary>
        /// gets a url (with or without http) and returns the full url
        /// </summary>
        /// <param name="url">url with or without http</param>
        /// <returns>full url</returns>
        /// <example>GetUrl("codehollow.com"); => returns https://codehollow.com</example>
        public static string GetAbsoluteUrl(string url)
        {
            return new UriBuilder(url).ToString();
        }

        /// <summary>
        /// reads a feed from a file
        /// </summary>
        /// <param name="filePath">the path to the feed file</param>
        /// <returns>parsed feed</returns>
        public static Feed ReadFromFile(string filePath)
        {
            string feedContent = System.IO.File.ReadAllText(filePath);
            return ReadFromString(feedContent);
        }

        /// <summary>
        /// reads a feed from the <paramref name="feedContent" />
        /// </summary>
        /// <param name="feedContent">the feed content (xml)</param>
        /// <returns>parsed feed</returns>
        public static Feed ReadFromString(string feedContent)
        {
            return FeedParser.GetFeed(feedContent);
        }

        /// <summary>
        /// read the rss feed type from the type statement of an html link
        /// </summary>
        /// <param name="linkType">application/rss+xml or application/atom+xml or ...</param>
        /// <returns>the feed type</returns>
        private static FeedType GetFeedTypeFromLinkType(string linkType)
        {
            if (linkType.Contains("application/rss"))
                return FeedType.Rss;

            if (linkType.Contains("application/atom"))
                return FeedType.Atom;

            throw new InvalidFeedLinkException($"The link type '{linkType}' is not a valid feed link!");
        }
    }
}
