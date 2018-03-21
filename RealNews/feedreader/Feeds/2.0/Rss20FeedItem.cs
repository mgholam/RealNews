namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// RSS 2.0 feed item accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class Rss20FeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "description" field of the feed item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "author" field of the feed item
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The "comments" field of the feed item
        /// </summary>
        public string Comments { get; set; }



        /// <summary>
        /// The "guid" field
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string PublishingDateString { get; set; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTime? PublishingDate { get; set; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource Source { get; set; }

        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// The "content:encoded" field
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCore DC { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss20FeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedItem"/> class.
        /// Reads a new feed item element based on the given xml item
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss20FeedItem(XElement item)
            : base(item)
        {
            this.Comments = item.GetValue("comments");
            this.Author = item.GetValue("author");
            this.Enclosure = new FeedItemEnclosure(item.GetElement("enclosure"));
            this.PublishingDateString = item.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);
            this.DC = new DublinCore(item);
            this.Source = new FeedItemSource(item.GetElement("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Guid = item.GetValue("guid");
            this.Description = item.GetValue("description");
            this.Content = item.GetValue("content:encoded")?.HtmlDecode();
            if (this.ExtraData == null)
                this.ExtraData = new Dictionary<string, string>();
            foreach (var i in item.DescendantNodes())
            {
                var x = i as XElement;
                if (x != null)
                {
                    if (this.ExtraData.ContainsKey(x.Name.LocalName))
                    {
                        this.ExtraData[x.Name.LocalName] += ", " + x.Value;
                    }
                    else
                    {
                        if (x.Name.LocalName == "content")
                        {
                            var w = x.GetAttribute("width");
                            if (w != null)
                            {
                                var s = x.GetAttribute("url");
                                if (s != null)
                                {
                                    if (this.ExtraData.ContainsKey("image" + w.Value) == false)
                                        this.ExtraData.Add("image" + w.Value, "<img src='" + s.Value + "' >");
                                }
                            }
                        }
                        else
                            this.ExtraData.Add(x.Name.LocalName, x.Value);
                    }
                }

            }
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            FeedItem fi = new FeedItem(this)
            {
                Author = this.Author,
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Description,
                Id = this.Guid,
                PublishingDate = this.PublishingDate,
                PublishingDateString = this.PublishingDateString
            };
            return fi;
        }
    }
}
