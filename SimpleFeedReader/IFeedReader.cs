using System.Xml;

namespace SimpleFeedReader;

/// <summary>
/// 
/// </summary>
public interface IFeedReader
{
    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="uri">The uri of the feed to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="uri">The uri of the feed to retrieve.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the specified feeds.
    /// </summary>
    /// <param name="uris">The uri's of the feeds to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    /// <remarks>This is a convenience method.</remarks>
    Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the specified feeds.
    /// </summary>
    /// <param name="uris">The uri's of the feeds to retrieve.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    /// <remarks>This is a convenience method.</remarks>
    Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default);
}