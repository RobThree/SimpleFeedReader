using System.ServiceModel.Syndication;

namespace SimpleFeedReader;

/// <summary>
/// Represents an item from a <see cref="SyndicationFeed"/>.
/// </summary>
public record FeedItem
{
    /// <summary>
    /// The Id of the <see cref="FeedItem"/>.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// The Title of the <see cref="FeedItem"/>.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The Content of the <see cref="FeedItem"/>.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// The Summary of the <see cref="FeedItem"/>.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// The Uri of the <see cref="FeedItem"/>.
    /// </summary>
    public Uri? Uri { get; init; }

    /// <summary>
    /// The images of the <see cref="FeedItem"/>.
    /// </summary>
    public IEnumerable<Uri>? Images { get; init; }

    /// <summary>
    /// The vategories of the <see cref="FeedItem"/>.
    /// </summary>
    public IEnumerable<string>? Categories { get; init; }

    /// <summary>
    /// The publication date of the <see cref="FeedItem"/>.
    /// </summary>
    public DateTimeOffset? PublishDate { get; init; }

    /// <summary>
    /// The date when the feeditem was last updated <see cref="FeedItem"/>.
    /// </summary>
    public DateTimeOffset? LastUpdatedDate { get; init; }

    /// <summary>
    /// Initializes a new <see cref="FeedItem"/>.
    /// </summary>
    public FeedItem()
    {
        Images = [];
        Categories = [];
    }

    /// <summary>
    /// Returns content, if any, otherwise returns the summary as content.
    /// </summary>
    /// <returns>Returns content, if any, otherwise returns the summary as content.</returns>
    /// <remarks>This method is intended as conveinience-method.</remarks>
    public string? GetContent() => !string.IsNullOrEmpty(Content) ? Content : Summary;

    /// <summary>
    /// Returns the summary, if any, otherwise returns the content as the summary.
    /// </summary>
    /// <returns>Returns the summary, if any, otherwise returns the content as the summary.</returns>
    /// <remarks>This method is intended as conveinience-method.</remarks>
    public string? GetSummary() => !string.IsNullOrEmpty(Summary) ? Summary : Content;
}
