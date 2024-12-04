using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

[assembly: AssemblyVersion("8.0.9")]

namespace N2.RssAtomFeedReader;

public static class BuilderConfiguration
{
    /// <summary>
    /// Add the feed reader service to your application.
    /// </summary>
    public static IServiceCollection AddFeedReader(this IServiceCollection services, Func<FeedReaderOptions> options)
    {
        var setup = options?.Invoke() ?? new FeedReaderOptions();

        services.AddSingleton<IFeedReader>(new FeedReader(setup));
        return services;
    }
}