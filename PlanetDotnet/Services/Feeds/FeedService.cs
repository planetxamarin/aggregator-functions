// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.DateTimes;
using PlanetDotnet.Brokers.Feeds;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Brokers.Serializations;
using PlanetDotnet.Brokers.Storages;

namespace PlanetDotnet.Services.Feeds
{
    internal partial class FeedService : IFeedService
    {
        private readonly IFeedBroker feedBroker;
        private readonly IAuthorBroker authorBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly ISerializationBroker serializationBroker;
        private readonly IStorageBroker storageBroker;

        private const string RssFeedTitleKey = "RssFeedTitle";
        private const string RssFeedDescriptionKey = "RssFeedDescription";
        private const string RssFeedUrlKey = "RssFeedUrl";
        private const string RssFeedImageUrlKey = "RssFeedImageUrl";

        public FeedService(
            IFeedBroker feedBroker,
            IAuthorBroker authorBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker,
            ISerializationBroker serializationBroker,
            IStorageBroker storageBroker)
        {
            this.feedBroker = feedBroker;
            this.authorBroker = authorBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
            this.serializationBroker = serializationBroker;
            this.storageBroker = storageBroker;
        }

        public async ValueTask<SyndicationFeed> GetMixedFeedAsync()
        {
            var authors = await this.authorBroker.GetAllAuthorsAsync();

            return await GetCombinedFeedsAsync(authors, language: "mixed");
        }

        public async ValueTask LoadFeedAsync()
        {
            var authors = await this.authorBroker.GetAllAuthorsAsync();

            this.loggingBroker.LogInformation($"Found {authors.Count()} author(s) globally...");

            var languages = authors.Select(author => author.FeedLanguageCode).Distinct().ToList();

            this.loggingBroker.LogInformation($"Available languages: {string.Join(",", languages)}.");

            foreach (var language in languages)
            {
                var languageAuthors = authors.Where(author =>
                    author.FeedLanguageCode == language);

                this.loggingBroker.LogInformation($"{authors.Count()} author(s) are writing in '{language}'...");

                var feed = await GetCombinedFeedsAsync(languageAuthors, language);

                var content = await this.serializationBroker.SerializeFeedAsync(feed);

                this.loggingBroker.LogInformation($"Blob for '{language}' is uploading...");

                await this.storageBroker.UploadBlobAsync(language, content);

                this.loggingBroker.LogInformation($"Blob for '{language}' feed was uploaded successfully.");
            }
        }

        public async ValueTask<SyndicationFeed> RetrieveFeedAsync(string language)
        {
            var content = await this.storageBroker.ReadBlobAsync(language);

            return this.serializationBroker.DeserializeFeed(content);
        }

        private async ValueTask<SyndicationFeed> GetCombinedFeedsAsync(
            IEnumerable<Author> authors,
            string language)
        {
            var feedItems = new List<SyndicationItem>();

            foreach (var author in authors)
            {
                foreach (var feedUri in author.FeedUris)
                {
                    try
                    {
                        var feed = await this.feedBroker.ReadFeedAsync(feedUri.AbsoluteUri);

                        feedItems.AddRange(feed.Items);
                    }
                    catch (Exception ex)
                    {
                        this.loggingBroker.LogError(ex);
                    }
                }
            }

            return CreateFeedInstance(
                items: feedItems,
                lastUpdate: this.dateTimeBroker.GetCurrentDateTimeOffset(),
                constributers: authors,
                languageCode: language);
        }

        private static SyndicationFeed CreateFeedInstance(
            List<SyndicationItem> items,
            DateTimeOffset lastUpdate,
            IEnumerable<Author> constributers,
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

            foreach (var constributer in constributers)
            {
                var item = new SyndicationPerson(
                    email: constributer.EmailAddress,
                    name: $"{constributer.FirstName} {constributer.LastName}",
                    uri: constributer.WebSite.ToString());

                feed.Contributors.Add(item);
            }

            return feed;
        }

    }
}
