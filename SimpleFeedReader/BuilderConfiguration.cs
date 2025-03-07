using Microsoft.Extensions.DependencyInjection;

namespace SimpleFeedReader;

/// <summary>
/// Extension methods for configuring the feed reader service.
/// </summary>
public static class BuilderConfiguration
{
    /// <summary>
    /// Add the feed reader service to your application.
    /// </summary>
    /// <param name="services">The service collection to add the feed reader to.</param>
    /// <param name="options">The options to configure the feed reader.</param>
    public static IServiceCollection AddFeedReader(this IServiceCollection services, Func<FeedReaderOptions> options)
    {
        var setup = options?.Invoke() ?? new FeedReaderOptions();

        services.AddSingleton<IFeedReader>(new FeedReader(setup));
        return services;
    }
}
