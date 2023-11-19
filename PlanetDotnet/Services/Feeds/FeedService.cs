// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.DateTimes;
using PlanetDotnet.Brokers.Feeds;
using PlanetDotnet.Brokers.Loggings;

namespace PlanetDotnet.Services.Feeds
{
    internal partial class FeedService : IFeedService
    {
        private readonly IFeedBroker feedBroker;
        private readonly IAuthorBroker authorBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        private const string RssFeedTitleKey = "RssFeedTitle";
        private const string RssFeedDescriptionKey = "RssFeedDescription";
        private const string RssFeedUrlKey = "RssFeedUrl";
        private const string RssFeedImageUrlKey = "RssFeedImageUrl";

        public FeedService(
            IFeedBroker feedBroker,
            IAuthorBroker authorBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.feedBroker = feedBroker;
            this.authorBroker = authorBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<SyndicationFeed> CombineFeedsAsync()
        {
            var authors = await this.authorBroker.GetAllAuthorsAsync();

            var feedItems = new List<SyndicationItem>();

            foreach (var author in authors)
            {
                foreach (var feedUri in author.FeedUris)
                {
                    var feed = await this.feedBroker.ReadFeedAsync(feedUri.AbsoluteUri);

                    feedItems.AddRange(feed.Items);
                }
            }

            return CreateCombinedFeed(
                items: feedItems,
                lastUpdate: this.dateTimeBroker.GetCurrentDateTimeOffset());
        }

        private static SyndicationFeed CreateCombinedFeed(
            List<SyndicationItem> items,
            DateTimeOffset lastUpdate,
            string languageCode = "mixed")
        {
            var rssFeedTitle = Environment.GetEnvironmentVariable(
                variable: RssFeedTitleKey,
                target: EnvironmentVariableTarget.Process);

            var rssFeedDescription = Environment.GetEnvironmentVariable(
                 variable: RssFeedDescriptionKey,
                 target: EnvironmentVariableTarget.Process);

            var rssFeedUrl = Environment.GetEnvironmentVariable(
                variable: RssFeedUrlKey,
                target: EnvironmentVariableTarget.Process);

            var rssFeedImageUrl = Environment.GetEnvironmentVariable(
                variable: RssFeedImageUrlKey,
                target: EnvironmentVariableTarget.Process);

            var feed = new SyndicationFeed(
                title: rssFeedTitle,
                description: rssFeedDescription,
                feedAlternateLink: new Uri(rssFeedUrl),
                items: items);

            feed.ImageUrl = new Uri(rssFeedImageUrl);
            feed.Copyright = new TextSyndicationContent("The copyright for each post is retained by its author.");
            feed.Language = languageCode;
            feed.LastUpdatedTime = lastUpdate;

            return feed;
        }
    }
}
