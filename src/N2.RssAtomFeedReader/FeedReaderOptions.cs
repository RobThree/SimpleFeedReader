using System.Collections.Generic;

namespace N2.RssAtomFeedReader;

/// <summary>
/// Options to initialize a feed reader.
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
    /// A default set of feeds that can be retrieved using the key.
    /// </summary>
    public Dictionary<string, string> Feeds { get; set; } = [];
}