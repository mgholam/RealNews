namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// The base object for all feed items
    /// </summary>
    public abstract class BaseFeedItem
    {
        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; set; } // title

        /// <summary>
        /// The "link" element
        /// </summary>
        public string Link { get; set; } // link
                                         /// <summary>
                                         /// The "enclosure" field
                                         /// </summary>
        public FeedItemEnclosure Enclosure { get; set; }

        /// <summary>
        /// Gets the underlying XElement in order to allow reading properties that are not available in the class itself
        /// </summary>
        public XElement Element { get; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }

        internal abstract FeedItem ToFeedItem();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        protected BaseFeedItem()
        {
            ExtraData = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFeedItem"/> class.
        /// Reads a base feed item based on the xml given in element
        /// </summary>
        /// <param name="item">feed item as xml</param>
        protected BaseFeedItem(XElement item)
        {
            this.Title = item.GetValue("title");
            this.Link = item.GetValue("link");
            this.Element = item;
        }
    }
}
