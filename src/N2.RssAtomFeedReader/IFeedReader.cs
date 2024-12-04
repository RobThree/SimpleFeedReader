using System.Collections.Generic;

namespace N2.RssAtomFeedReader;

public interface IFeedReader
{
    /// <summary>
    /// Retrieves a feed from the specified URI.
    /// </summary>
    /// <param name="uri">A valid uri specification</param>
    /// <returns>A list of feed items.</returns>
    IEnumerable<FeedItem> RetrieveFeed(string uri);
}