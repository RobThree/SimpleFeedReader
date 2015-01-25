using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace SimpleFeedReader
{
    /// <summary>
    /// Retrieves <see cref="SyndicationFeed"/>s and normalizes the items from the feed into <see cref="FeedItem"/>s.
    /// </summary>
    public class FeedReader
    {
        /// <summary>
        /// Gets the default FeedItemNormalizer the <see cref="FeedReader"/> will use when normalizing 
        /// <see cref="SyndicationItem"/>s.
        /// </summary>
        public IFeedItemNormalizer DefaultNormalizer { get; private set; }

        /// <summary>
        /// Gets wether the FeedReader will throw on exceptions or suppress exceptions and return empty results on
        /// errors.
        /// </summary>
        public bool ThrowOnError { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        public FeedReader()
            : this(new DefaultFeedItemNormalizer()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="throwOnError">
        /// When true, the <see cref="FeedReader"/> will throw on errors, when false the <see cref="FeedReader"/> will 
        /// suppress exceptions and return empty results.
        /// </param>
        public FeedReader(bool throwOnError)
            : this(new DefaultFeedItemNormalizer(), throwOnError) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="defaultFeedItemNormalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="SyndicationItem"/>s.
        /// </param>
        public FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer)
            : this(defaultFeedItemNormalizer, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="defaultFeedItemNormalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="SyndicationItem"/>s.
        /// </param>
        /// <param name="throwOnError">
        /// When true, the <see cref="FeedReader"/> will throw on errors, when false the <see cref="FeedReader"/> will 
        /// suppress exceptions and return empty results.
        /// </param>
        public FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer, bool throwOnError)
        {
            if (defaultFeedItemNormalizer == null)
                throw new ArgumentNullException("defaultFeedItemNormalizer");

            this.DefaultNormalizer = defaultFeedItemNormalizer;
            this.ThrowOnError = throwOnError;
        }

        /// <summary>
        /// Retrieves the specified feeds.
        /// </summary>
        /// <param name="uris">The uri's of the feeds to retrieve.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeeds(IEnumerable<string> uris)
        {
            return this.RetrieveFeeds(uris, this.DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feeds.
        /// </summary>
        /// <param name="uris">The uri's of the feeds to retrieve.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeeds(IEnumerable<string> uris, IFeedItemNormalizer normalizer)
        {
            List<FeedItem> items = new List<FeedItem>();
            foreach (var u in uris)
                items.AddRange(RetrieveFeed(u, normalizer));
            return items;
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="uri">The uri of the feed to retrieve.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(string uri)
        {
            return this.RetrieveFeed(uri, this.DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="uri">The uri of the feed to retrieve.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(string uri, IFeedItemNormalizer normalizer)
        {
            try
            {
                return this.RetrieveFeed(XmlReader.Create(uri), normalizer);
            }
            catch
            {
                if (this.ThrowOnError)
                    throw;
            }
            return Enumerable.Empty<FeedItem>();
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(XmlReader xmlReader)
        {
            return this.RetrieveFeed(xmlReader, this.DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(XmlReader xmlReader, IFeedItemNormalizer normalizer)
        {
            if (normalizer == null)
                throw new ArgumentNullException("normalizer");

            var items = new List<FeedItem>();
            try
            {
                var feed = SyndicationFeed.Load(xmlReader);
                foreach (var item in feed.Items)
                    items.Add(normalizer.Normalize(feed, item));
            }
            catch
            {
                if (this.ThrowOnError)
                    throw;
            }
            return items;
        }
    }
}
