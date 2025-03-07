using System.Net.Http;
using System.ServiceModel.Syndication;

namespace SimpleFeedReader;

/// <summary>
/// Options to configure the <see cref="FeedReader"/>.
/// </summary>
public class FeedReaderOptions
{
    /// <summary>
    /// The default normalizer to use when normalizing <see cref="SyndicationItem"/>s.
    /// </summary>
    public IFeedItemNormalizer DefaultNormalizer { get; set; } = new DefaultFeedItemNormalizer();
    /// <summary>
    /// When true, the <see cref="FeedReader"/> will throw on errors.
    /// </summary>
    public bool ThrowOnError { get; set; } = false;
    /// <summary>
    /// When you want to use a custom <see cref="HttpClient"/> you can set it through here.
    /// </summary>
    public HttpClient? HttpClient { get; set; } = null;
}