#  ![Logo](https://raw.githubusercontent.com/RobThree/SimpleFeedReader/master/Gfx/icon.png) SimpleFeedReader

Easy to use, simple, Syndication feed reader (Atom / RSS). Available as [Nuget Package](https://www.nuget.org/packages/SimpleFeedReader/).

## Usage

```c#
var reader = new FeedReader();
var items = reader.RetrieveFeed("http://www.nytimes.com/services/xml/rss/nyt/International.xml");

foreach (var i in items)
    Console.WriteLine($"{i.Date.ToString("g")}\t{i.Title}");
````
Output:

```
4/16/2014  4:27 AM     Growth Rose 7.4% in First Quarter, China Reports
4/16/2014 12:29 AM     Milan Court Gives Berlusconi a Year of Community Service
4/15/2014 12:34 PM     Desalination Plant Said to Be Planned for Thirsty Beijing
4/15/2014  7:24 PM     After Prank by Dutch Girl on Twitter, Real Trouble
4/15/2014  4:33 PM     Afghanistan Says NATO Airstrike in East Killed Civilians
4/16/2014 12:49 AM     Iran Escalates Dispute Over U.N. Envoy
...
````

## Notes

This is a fork of [SimpleFeedReader](https://github.com/RobThree/SimpleFeedReader) by [RobIII](https://github.com/RobThree). The original project is no longer maintained. It is converted
to Net8.0 and .NET Standard 2.0.

By default the `FeedReader` suppresses exceptions (since feeds tend to go down occasionally, they contain invalid XML from time-to-time and have all other sorts of problems). However, you can tell the `FeedReader` to throw exceptions simply by setting the `throwOnError` argument of the `FeedReader`'s constructor to true.

The `FeedReader` also accepts an optional `FeedNormalizer` (needs to implement the `IFeedItemNormalizer` interface). This "normalizer" can transform or otherwise affect the way [`SyndicationItem`](http://msdn.microsoft.com/en-us/library/system.servicemodel.syndication.syndicationitem.aspx)s are transformed into `FeedItem`s. The `FeedItem` is the basic object retrieved from feeds and, for simplicity, contains only a few simple properties and methods. It has `Title`, `Summary`, `Content`, `Uri` , `PublishDate`, `LastUpdatedDate`, `Images` and `Categories` properties and that's about it. The default `DefaultFeedItemNormalizer` strips and decodes any HTML in the `Title`, `Summary`, `Content` to (try to) reliably return plain text only. The `Date` property will be populated with whatever the `SyndicationItem`'s latest date is: the `PublishDate` or `LastUpdatedTime`.

You can implement your own `IFeedItemNormalizer` (see the [UnitTest project](https://github.com/RobThree/SimpleFeedReader/tree/master/SimpleFeedReaderTests) for examples) to handle 'normalization' differently to your desire. The `FeedReader` has some convienience methods like `RetrieveFeeds()` that retrieve more than one feed.

The project is aimed at easy, don't-make-me-think, retrieval of syndication feeds' entries. It is by no means intended as full-fledged feedreader. It is, however, easily extensible for your purposes (again, see the [UnitTest project](https://github.com/RobThree/SimpleFeedReader/tree/master/SimpleFeedReaderTests) for examples; the `ExtendedFeedItem` and `ExtendedFeedItemNormalizer` are nice concrete examples of this idea).


[![Build status](https://github.com/gjkaal/SimpleFeedReader/actions/workflows/dotnet.yml)](https://github.com/gjkaal/SimpleFeedReader/actions/workflows/dotnet.yml/badge.svg) 
<a href="https://www.nuget.org/packages/N2.FeedReader"><img src="http://img.shields.io/nuget/v/N2.FeedReader.svg?style=flat-square" alt="NuGet version" height="18"></a>
