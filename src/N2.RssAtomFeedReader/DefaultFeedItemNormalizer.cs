using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace N2.RssAtomFeedReader
{
    /// <summary>
    /// The <see cref="DefaultFeedItemNormalizer"/> normalizes <see cref="FeedItem.Title"/>, 
    /// <see cref="FeedItem.Content"/> and <see cref="FeedItem.Summary"/> of <see cref="FeedItem"/>s to the point where
    /// they no longer contain any HTML, redundant whitespace, un-normalized unicode chars and other control chars like
    /// tabs, newlines or backspaces. The <see cref="FeedItem"/>'s <see cref="FeedItem.Date"/> property will contain
    /// whichever date is latest; the <see cref="FeedItem.PublishDate"/> or <see cref="FeedItem.LastUpdatedDate"/>.
    /// </summary>
    /// <remarks>
    /// You can implement a normalizer yourself by implementing the <see cref="IFeedItemNormalizer"/> interface.
    /// </remarks>
    public partial class DefaultFeedItemNormalizer : IFeedItemNormalizer
    {
        private static readonly Regex htmlRegex = RegExXmlElement();    //@"<(.|\n)*?>"
        private static readonly Regex controlCodesRegex = RegExControlCodes();
        private static readonly Regex whiteSpaceRegex = RegExWhiteSpace();

        /// <summary>
        /// Normalizes a SyndicationItem into a FeedItem.
        /// </summary>
        /// <param name="feed">The <see cref="SyndicationFeed"/> on which the item was retrieved.</param>
        /// <param name="item">A <see cref="SyndicationItem"/> to normalize into a <see cref="FeedItem"/>.</param>
        /// <returns>Returns a normalized <see cref="FeedItem"/>.</returns>
        public virtual FeedItem Normalize(SyndicationFeed feed, SyndicationItem item)
        {
            var alternatelink = item.Links.FirstOrDefault(l => l.RelationshipType == null || l.RelationshipType.Equals("alternate", StringComparison.OrdinalIgnoreCase));

            Uri itemuri = null;
            if (alternatelink == null && !Uri.TryCreate(item.Id, UriKind.Absolute, out Uri parsed))
            {
                itemuri = parsed;
            }
            else
            {
                itemuri = alternatelink.GetAbsoluteUri();
            }

            return new FeedItem
            {
                Id = string.IsNullOrEmpty(item.Id) ? null : item.Id.Trim(),
                Title = item.Title == null ? null : Normalize(item.Title.Text),
                Content = item.Content == null ? null : Normalize(((TextSyndicationContent)item.Content).Text),
                Summary = item.Summary == null ? null : Normalize(item.Summary.Text),
                PublishDate = item.PublishDate,
                LastUpdatedDate = item.LastUpdatedTime == DateTimeOffset.MinValue ? item.PublishDate : item.LastUpdatedTime,
                Uri = itemuri,
                Images = GetFeedItemImages(item),
                Categories = item.Categories.Select(c => c.Name)
            };
        }

        private static IEnumerable<Uri> GetFeedItemImages(SyndicationItem item)
        {
            return item.ElementExtensions
                .Where(p => p.OuterName.Equals("image"))
                .Select(p => new Uri(p.GetObject<XElement>().Value));
        }

        private static string Normalize(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = HtmlDecode(value);
                if (string.IsNullOrEmpty(value))
                    return value;

                value = StripHTML(value);
                value = StripDoubleOrMoreWhiteSpace(RemoveControlChars(value));
                value = value.Normalize().Trim();
            }
            return value;
        }

        private static string RemoveControlChars(string value)
        {
            return controlCodesRegex.Replace(value, " ");
        }

        private static string StripDoubleOrMoreWhiteSpace(string value)
        {
            return whiteSpaceRegex.Replace(value, " ");
        }

        private static string StripHTML(string value)
        {
            return htmlRegex.Replace(value, " ");
        }

        private static string HtmlDecode(string value, int threshold = 5)
        {
            int c = 0;
            string newvalue = WebUtility.HtmlDecode(value);
            while (!newvalue.Equals(value) && c < threshold)    //Keep decoding (if a string is double/triple/... encoded; we want the original)
            {
                c++;
                value = newvalue;
                newvalue = WebUtility.HtmlDecode(value);
            }
            if (c >= threshold) //Decoding threshold exceeded?
                return null;

            return newvalue;
        }

#if NETSTANDARD
        private static Regex RegExXmlElement()
        {
            return new Regex(@"<[^>]*>", RegexOptions.Compiled);
        }
        private static Regex RegExControlCodes()
        {
            return new Regex(@"[\x00-\x1F\x7f]", RegexOptions.Compiled);
        }
        private static Regex RegExWhiteSpace()
        {
            return new Regex(@"\s{2,}", RegexOptions.Compiled);
        }

#else
        [GeneratedRegex(@"<[^>]*>", RegexOptions.Compiled)]
        private static partial Regex RegExXmlElement();
        [GeneratedRegex(@"[\x00-\x1F\x7f]", RegexOptions.Compiled)]
        private static partial Regex RegExControlCodes();
        [GeneratedRegex(@"\s{2,}", RegexOptions.Compiled)]
        private static partial Regex RegExWhiteSpace();
#endif

    }
}
