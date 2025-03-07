using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Xml;

namespace SimpleFeedReader;

/// <summary>
/// Retrieves <see cref="SyndicationFeed"/>s and normalizes the items from the feed into <see cref="FeedItem"/>s.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FeedReader"/> class.
/// </remarks>
/// <param name="defaultFeedItemNormalizer">
/// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="SyndicationItem"/>s.
/// </param>
/// <param name="throwOnError">
/// When true, the <see cref="FeedReader"/> will throw on errors, when false the <see cref="FeedReader"/> will 
/// suppress exceptions and return empty results.
/// </param>
public class FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer, bool throwOnError)
{
    private static readonly HttpClient _httpclient = new();

    /// <summary>
    /// Gets the default FeedItemNormalizer the <see cref="FeedReader"/> will use when normalizing 
    /// <see cref="SyndicationItem"/>s.
    /// </summary>
    public IFeedItemNormalizer DefaultNormalizer { get; private set; } = defaultFeedItemNormalizer ?? throw new ArgumentNullException(nameof(defaultFeedItemNormalizer));

    /// <summary>
    /// Gets wether the FeedReader will throw on exceptions or suppress exceptions and return empty results on
    /// errors.
    /// </summary>
    public bool ThrowOnError { get; private set; } = throwOnError;

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
    /// Retrieves the specified feeds.
    /// </summary>
    /// <param name="uris">The uri's of the feeds to retrieve.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    /// <remarks>This is a convenience method.</remarks>
    public Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris)
        => RetrieveFeedsAsync(uris, DefaultNormalizer);

    /// <summary>
    /// Retrieves the specified feeds.
    /// </summary>
    /// <param name="uris">The uri's of the feeds to retrieve.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    /// <remarks>This is a convenience method.</remarks>
    public async Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris, IFeedItemNormalizer normalizer)
    {
        var tasks = uris.Select(uri => RetrieveFeedAsync(uri, normalizer));
        await Task.WhenAll(tasks);

        return tasks.SelectMany(tasks => tasks.Result);
    }

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="uri">The uri of the feed to retrieve.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri)
        => RetrieveFeedAsync(uri, DefaultNormalizer);

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="uri">The uri of the feed to retrieve.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    public async Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri, IFeedItemNormalizer normalizer)
    {
        try
        {
            using var reader = await GetXmlReaderAsync(uri);
            return await RetrieveFeedAsync(reader, normalizer);
        }
        catch
        {
            if (ThrowOnError)
            {
                throw;
            }
        }
        return [];
    }

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader) =>
        RetrieveFeedAsync(xmlReader, DefaultNormalizer);

    /// <summary>
    /// Retrieves the specified feed.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
    /// <param name="normalizer">
    /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
    /// </param>
    /// <returns>
    /// Returns a task that resolves to an <see cref="IEnumerable{FeedItem}"/> of retrieved <see cref="FeedItem"/>s.
    /// </returns>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader, IFeedItemNormalizer normalizer)
    {
        if (xmlReader == null)
        {
            throw new ArgumentNullException(nameof(xmlReader));
        }

        if (normalizer == null)
        {
            throw new ArgumentNullException(nameof(normalizer));
        }

        try
        {
            var feed = SyndicationFeed.Load(xmlReader);
            return Task.FromResult(feed.Items.Select(item => normalizer.Normalize(feed, item)));
        }
        catch
        {
            if (ThrowOnError)
            {
                throw;
            }
        }
        return Task.FromResult(Enumerable.Empty<FeedItem>());
    }

    private static async Task<XmlReader> GetXmlReaderAsync(string uri)
    {
        if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
        {
            var response = await _httpclient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return XmlReader.Create(stream);
        }
        else if (File.Exists(uri))
        {
            var stream = File.OpenRead(uri);
            return XmlReader.Create(stream);
        }
        throw new FileNotFoundException($"The file '{uri}' was not found.");
    }
}
