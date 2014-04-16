using System.ServiceModel.Syndication;

namespace SimpleFeedReader
{
    /// <summary>
    /// Provides the base FeedItemNormalizer interface for <see cref="FeedItem"/>s.
    /// </summary>
    public interface IFeedItemNormalizer
    {
        /// <summary>
        /// Normalizes a <see cref="SyndicationItem"/> into a <see cref="FeedItem"/>.
        /// </summary>
        /// <param name="feed">The <see cref="SyndicationFeed"/> on which the item was retrieved.</param>
        /// <param name="item">The <see cref="SyndicationItem"/> to normalize.</param>
        /// <returns>Returns a normalized <see cref="FeedItem"/>.</returns>
        FeedItem Normalize(SyndicationFeed feed, SyndicationItem item);
    }
}
