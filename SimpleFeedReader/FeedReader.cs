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
public class FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer, bool throwOnError) : IFeedReader
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

    /// <inheritdoc/>
    public Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris, CancellationToken cancellationToken = default)
        => RetrieveFeedsAsync(uris, DefaultNormalizer, cancellationToken);


    /// <inheritdoc/>
    public async Task<IEnumerable<FeedItem>> RetrieveFeedsAsync(IEnumerable<string> uris, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default)
    {
        var tasks = uris.Select(uri => RetrieveFeedAsync(uri, normalizer, cancellationToken));
        await Task.WhenAll(tasks);

        return tasks.SelectMany(tasks => tasks.Result);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri, CancellationToken cancellationToken = default)
        => RetrieveFeedAsync(uri, DefaultNormalizer, cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<FeedItem>> RetrieveFeedAsync(string uri, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default)
    {
        try
        {
            using var reader = await GetXmlReaderAsync(uri, cancellationToken);
            return await RetrieveFeedAsync(reader, normalizer, cancellationToken);
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

    /// <inheritdoc/>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader, CancellationToken cancellationToken = default) =>
        RetrieveFeedAsync(xmlReader, DefaultNormalizer, cancellationToken);

    /// <inheritdoc/>
    public Task<IEnumerable<FeedItem>> RetrieveFeedAsync(XmlReader xmlReader, IFeedItemNormalizer normalizer, CancellationToken cancellationToken = default)
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

    private static async Task<XmlReader> GetXmlReaderAsync(string uri, CancellationToken cancellationToken = default)
    {
        if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
        {
            var response = await _httpclient.GetAsync(uri, cancellationToken);
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
