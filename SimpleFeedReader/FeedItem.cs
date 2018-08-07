using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;

namespace SimpleFeedReader
{
    /// <summary>
    /// Represents an item from a <see cref="SyndicationFeed"/>.
    /// </summary>
    public class FeedItem
    {
        /// <summary>
        /// The Id of the <see cref="FeedItem"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Title of the <see cref="FeedItem"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Content of the <see cref="FeedItem"/>.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The Summary of the <see cref="FeedItem"/>.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The Uri of the <see cref="FeedItem"/>.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The images of the <see cref="FeedItem"/>.
        /// </summary>
        public IEnumerable<Uri> Images { get; set; }

        /// <summary>
        /// The vategories of the <see cref="FeedItem"/>.
        /// </summary>
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// The Date of the <see cref="FeedItem"/>.
        /// </summary>
        [Obsolete("Split into PublishDate and LastUpdatedDate")]
        public DateTimeOffset Date { get { return new[] { PublishDate, LastUpdatedDate }.Max(); } }

        /// <summary>
        /// The publication date of the <see cref="FeedItem"/>.
        /// </summary>
        public DateTimeOffset PublishDate { get; set; }

        /// <summary>
        /// The date when the feeditem was last updated <see cref="FeedItem"/>.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Initializes a new <see cref="FeedItem"/>.
        /// </summary>
        public FeedItem()
        {
            Images = new List<Uri>();
            Categories = new List<string>();
        }

        /// <summary>
        /// Initializes a new <see cref="FeedItem"/> by copying the passed item's properties into the new instance.
        /// </summary>
        /// <param name="item">The <see cref="FeedItem"/> to copy.</param>
        /// <remarks>This is a copy-constructor.</remarks>
        public FeedItem(FeedItem item)
            : this()
        {
            Title = item.Title;
            Content = item.Content;
            Summary = item.Summary;
            Uri = item.Uri;
            PublishDate = item.PublishDate;
            LastUpdatedDate = item.LastUpdatedDate;
            Images = item.Images;
            Categories = item.Categories;
        }

        /// <summary>
        /// Returns content, if any, otherwise returns the summary as content.
        /// </summary>
        /// <returns>Returns content, if any, otherwise returns the summary as content.</returns>
        /// <remarks>This method is intended as conveinience-method.</remarks>
        public string GetContent()
        {
            return !string.IsNullOrEmpty(Content) ? Content : Summary;
        }

        /// <summary>
        /// Returns the summary, if any, otherwise returns the content as the summary.
        /// </summary>
        /// <returns>Returns the summary, if any, otherwise returns the content as the summary.</returns>
        /// <remarks>This method is intended as conveinience-method.</remarks>
        public string GetSummary()
        {
            return !string.IsNullOrEmpty(Summary) ? Summary : Content;
        }
    }
}
