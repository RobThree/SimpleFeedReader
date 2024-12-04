using Microsoft.VisualStudio.TestTools.UnitTesting;
using N2.RssAtomFeedReader;
using System.Globalization;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace SimpleFeedReaderTests
{
    [TestClass]
    public partial class FeedReaderTests
    {
        [TestMethod]
        public void BasicRSSFeedTest()
        {
            var target = new FeedReader(true);
            var items = target.RetrieveFeed(@"TestFeeds/basic.rss").ToArray();

            Assert.AreEqual(2, items.Length);

            Assert.AreEqual("http://example.org/foo/bar/1", items[0].Uri.ToString());
            Assert.AreEqual("Title 1", items[0].Title);
            Assert.IsTrue(items[0].Summary!.StartsWith("Lorem ipsum dolor sit"));
            Assert.IsNull(items[0].Content);
            Assert.AreEqual("tag:example.org,1999:blog-123456789123456789123456789.post-987564321987654231", items[0].Id);
            Assert.AreEqual(DateTimeOffset.Parse("2014-04-16T13:57:35.0000000+02:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].PublishDate);
            Assert.AreEqual(DateTimeOffset.Parse("2014-04-16T13:57:35.0000000+02:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].LastUpdatedDate);

            Assert.IsNull(items[1].Title);
            Assert.IsNull(items[1].Summary);
            Assert.IsNull(items[1].Content);
            Assert.IsNull(items[1].Uri);
            Assert.IsNull(items[1].Id);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].PublishDate);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].LastUpdatedDate);

            Assert.IsTrue(items[0].GetContent()!.StartsWith("Lorem ipsum dolor sit"));
            Assert.IsTrue(items[0].GetSummary()!.StartsWith("Lorem ipsum dolor sit"));
        }

        [TestMethod]
        public void BasicRSSWithImageFeedTest()
        {
            var target = new FeedReader(true);
            var items = target.RetrieveFeed(@"TestFeeds/basic_image.rss").ToArray();

            Assert.AreEqual(2, items.Length);

            Assert.AreEqual("http://example.org/foo/bar/1", items[0].Uri.ToString());
            Assert.AreEqual("Title 1", items[0].Title);
            Assert.IsTrue(items[0].Summary!.StartsWith("Lorem ipsum dolor sit"));
            Assert.IsNull(items[0].Content);
            Assert.AreEqual("tag:example.org,1999:blog-123456789123456789123456789.post-987564321987654231", items[0].Id);
            Assert.AreEqual(DateTimeOffset.Parse("2014-04-16T13:57:35.0000000+02:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].PublishDate);
            Assert.AreEqual(DateTimeOffset.Parse("2014-04-16T13:57:35.0000000+02:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].LastUpdatedDate);

            Assert.IsNull(items[1].Title);
            Assert.IsNull(items[1].Summary);
            Assert.IsNull(items[1].Content);
            Assert.IsNull(items[1].Uri);
            Assert.IsNull(items[1].Id);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].PublishDate);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].LastUpdatedDate);

            Assert.IsTrue(items[0].GetContent()!.StartsWith("Lorem ipsum dolor sit"));
            Assert.IsTrue(items[0].GetSummary()!.StartsWith("Lorem ipsum dolor sit"));

            Assert.IsTrue(items[0].Images.Count() == 2);
            Assert.AreEqual(items[0].Images.ElementAt(0).ToString(), "http://example.org/foo/bar/123abc.png");
            Assert.AreEqual(items[0].Images.ElementAt(1).ToString(), "http://example.org/foo/bar/123abc_2.png");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ThrowsWhenRequiredFeedTest1()
        {
            var target = new FeedReader(true);
            _ = target.RetrieveFeed(@"TestFeeds/non_existing.rss");
        }

        //https://feeds.nos.nl/nosnieuwsalgemeen
        [TestMethod]
        public void OpenActualFeedTest1()
        {
            var target = new FeedReader(true);
            var items = target.RetrieveFeed(@"https://feeds.nos.nl/nosnieuwsalgemeen");
            Assert.IsTrue(items.Any());
            foreach (var item in items)
            {
                Assert.IsNotNull(item.Title);
                Assert.IsNotNull(item.GetContent());
                Assert.IsNotNull(item.GetSummary());
                Assert.IsNotNull(item.Uri);
                Assert.IsNotNull(item.Id);
                Assert.AreNotEqual(DateTimeOffset.MinValue, item.PublishDate);
                Assert.AreNotEqual(DateTimeOffset.MinValue, item.LastUpdatedDate);
                Console.WriteLine(item.Title);
                Console.WriteLine(item.GetContent());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void ThrowsWhenRequiredFeedTest2()
        {
            var target = new FeedReader(true);
            _ = target.RetrieveFeed(@"http://example.org/non_existing.rss");
        }

        [TestMethod]
        public void SuppressesExceptionsWhenRequiredFeedTest()
        {
            var target = new FeedReader(false);
            _ = target.RetrieveFeed(@"TestFeeds/non_existing.rss");
        }

        [TestMethod]
        public void DefaultNormalizationTest()
        {
            var target = new FeedReader(true);
            var items = target.RetrieveFeed(@"TestFeeds/decoding.rss").ToArray();

            var i0 = items[0];
            Assert.IsNull(i0.Summary);
            Assert.IsNull(i0.Title);

            var i1 = items[1];
            Assert.AreEqual("foo&bar", i1.Summary);
            Assert.AreEqual("foo&bar", i1.Title);

            var i2 = items[2];
            Assert.AreEqual(i2.Title, i2.Content);
        }

        [TestMethod]
        public void ExtendedNormalizerRSSFeedTest()
        {
            var target = new FeedReader(new ExtendedFeedItemNormalizer(), true);
            var items = target.RetrieveFeed(@"TestFeeds/basic.rss").ToArray();

            Assert.IsInstanceOfType(items[0], typeof(ExtendedFeedItem));
            Assert.AreEqual(1, ((ExtendedFeedItem)items[0]).Authors.Length);
            Assert.AreEqual("noreply1@example.org (John Doe 1)", ((ExtendedFeedItem)items[0]).Authors[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void DTDInjectionTest1()
        {
            var target = new FeedReader(true);  //We want to check the exception so don't suppress it
            _ = target.RetrieveFeed(@"TestFeeds/xml_injection1.rss");
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void DTDInjectionTest2()
        {
            var target = new FeedReader(true);  //We want to check the exception so don't suppress it
            _ = target.RetrieveFeed(@"TestFeeds/xml_injection2.rss");
        }

        [TestMethod]
        public void BasicAtomFeedTest()
        {
            var target = new FeedReader(true);
            var items = target.RetrieveFeed(@"TestFeeds/basic.atom").ToArray();

            Assert.AreEqual(2, items.Length);

            Assert.AreEqual("http://example.org/foo/bar/1", items[0].Uri.ToString());
            Assert.AreEqual("Summary1", items[0].Summary);
            Assert.AreEqual("Test1", items[0].Title);
            Assert.AreEqual("HTML content", items[0].Content);
            Assert.AreEqual("urn:uuid:0ea6c57b-4546-4264-8b96-13434c349d87", items[0].Id);

            Assert.AreEqual(DateTimeOffset.Parse("2013-03-13T13:37:31.0000000+00:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].PublishDate);
            Assert.AreEqual(DateTimeOffset.Parse("2014-04-16T13:57:35.0000000+00:00", CultureInfo.InvariantCulture, DateTimeStyles.None), items[0].LastUpdatedDate);

            Assert.AreEqual("http://example.org/foo/bar/2", items[1].Uri.ToString());
            Assert.AreEqual("Summary2", items[1].Summary);
            Assert.AreEqual("Test2", items[1].Title);
            Assert.AreEqual("Text content", items[1].Content);
            Assert.AreEqual("urn:uuid:d58672c4-f62e-483e-ab00-ff0940113e29", items[1].Id);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].PublishDate);
            Assert.AreEqual(DateTimeOffset.MinValue, items[1].LastUpdatedDate);
        }

        [TestMethod]
        public void BasicActualRSSFeedTest()
        {
            var target = new FeedReader(new GoogleFeedItemNormalizer(), true);
            var items = target.RetrieveFeed(@"TestFeeds/google_snapshot.rss").ToArray();

            Assert.AreEqual(10, items.Length);
            Assert.IsTrue(items[0].GetContent()!.StartsWith("(CNN) -- Rescue boats"));
            Assert.IsTrue(items[1].GetContent()!.StartsWith("Pro-Russian troops guard"));
            Assert.IsTrue(items[2].GetContent()!.StartsWith("(CNN) -- Former New York"));
            Assert.IsTrue(items[3].GetContent()!.StartsWith("Two blasts near the"));
            Assert.IsTrue(items[4].GetContent()!.StartsWith("A three-year-old boy"));
        }

        [TestMethod]
        public void BasicActualAtomFeedTest()
        {
            var target = new FeedReader(new GoogleFeedItemNormalizer(), true);
            var items = target.RetrieveFeed(@"TestFeeds/google_snapshot.atom").ToArray();

            Assert.AreEqual(10, items.Length);
            Assert.IsTrue(items[0].GetContent()!.StartsWith("(CNN) -- Rescue boats"));
            Assert.IsTrue(items[1].GetContent()!.StartsWith("Pro-Russian troops guard"));
            Assert.IsTrue(items[2].GetContent()!.StartsWith("(CNN) -- Former New York"));
            Assert.IsTrue(items[3].GetContent()!.StartsWith("Two blasts near the"));
            Assert.IsTrue(items[4].GetContent()!.StartsWith("A three-year-old boy"));
        }

        [TestMethod]
        public void BasicRSSCategoriesTest()
        {
            var target = new FeedReader();
            var items = target.RetrieveFeed(@"TestFeeds/categories.rss").ToArray();
            Assert.AreEqual(items[0].Categories.ElementAt(0), "NEWS");
            Assert.AreEqual(items[0].Categories.ElementAt(1), "TEST");
        }

        [TestMethod]
        public void BasicAtomCategoriesTest()
        {
            var target = new FeedReader();
            var items = target.RetrieveFeed(@"TestFeeds/categories.atom").ToArray();
            Assert.AreEqual(items[0].Categories.ElementAt(0), "a");
            Assert.AreEqual(items[1].Categories.ElementAt(0), "b");
            Assert.AreEqual(items[1].Categories.ElementAt(1), "c");
        }

        #region TestClasses

        private class ExtendedFeedItem : FeedItem
        {
            public string[] Authors { get; set; }

            public ExtendedFeedItem()
            {
                Authors = [];
            }

            public ExtendedFeedItem(FeedItem item)
                : base(item)
            {
                Authors = [];
            }
        }

        private class ExtendedFeedItemNormalizer : DefaultFeedItemNormalizer, IFeedItemNormalizer
        {
            public override FeedItem Normalize(SyndicationFeed feed, SyndicationItem item)
            {
                return new ExtendedFeedItem(base.Normalize(feed, item))
                {
                    Authors = item.Authors.Select(i => i.Name ?? i.Email).ToArray()
                };
            }
        }

        /// <summary>
        /// Simple sample class (not to be taken TOO seriously) to demonstrate extracting "interesting" stuff from a bunch of HTML crap in the feed
        /// </summary>
        private partial class GoogleFeedItemNormalizer : DefaultFeedItemNormalizer, IFeedItemNormalizer
        {
            /// <summary>
            /// Some people, when confronted with a problem, think  “I know, I'll use regular expressions.”
            /// Now they have two problems.     — Jamie Zawinski
            /// </summary>
            private static readonly Regex getlines = RegExGetRssLines();

            public override FeedItem Normalize(SyndicationFeed feed, SyndicationItem item)
            {
                if (item.Content != null)
                    item.Content = new TextSyndicationContent(GetLines(((TextSyndicationContent)item.Content).Text)[2]);
                if (item.Summary != null)
                    item.Summary = new TextSyndicationContent(GetLines(item.Summary.Text)[2]);
                return base.Normalize(feed, item);
            }

            private static string[] GetLines(string value)
            {
                return getlines.Matches(value).Cast<Match>().ToArray().Select(m => m.Value).ToArray();
            }

            [GeneratedRegex("<font(?:(?:\\s+color=\"(?:[#0-9])\")?|(?:\\s+size=\"-[12]\")?|(?:\\s+class=\"[a-zA-Z0-9]\")?)+>(.*?)</font>", RegexOptions.Compiled)]
            private static partial Regex RegExGetRssLines();
        }

        #endregion TestClasses
    }
}