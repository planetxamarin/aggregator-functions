// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Extensions;
using PlanetDotnet.Models.Feeds.Exceptions;
using Polly;
using Polly.Retry;

namespace PlanetDotnet.Services.CombinedFeeds
{
    public class CombinedFeedService : ICombinedFeedService
    {
        private readonly HttpClient httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IAuthorBroker authorBroker;
        private readonly ILoggingBroker loggingBroker;

        private const string RssFeedTitleKey = "RssFeedTitle";
        private const string RssFeedDescriptionKey = "RssFeedDescription";
        private const string RssFeedUrlKey = "RssFeedUrl";
        private const string RssFeedImageUrlKey = "RssFeedImageUrl";

        public CombinedFeedService(
            IAuthorBroker authorBroker,
            ILoggingBroker loggingBroker,
            HttpClient httpClient)
        {
            this.httpClient = httpClient;
            EnsureHttpClient();

            _retryPolicy ??= Policy.Handle<FailedFeedException>()
                .WaitAndRetryAsync(2, retry => TimeSpan.FromSeconds(retry * Math.Pow(1.2, retry)));

            this.authorBroker = authorBroker;
            this.loggingBroker = loggingBroker;
        }

        private void EnsureHttpClient()
        {
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("PlanetDotnet", $"{GetType().Assembly.GetName().Version}"));

            this.httpClient.Timeout = TimeSpan.FromSeconds(15);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        }

        public async Task<SyndicationFeed> LoadFeed(int? numberOfItems, string languageCode = "mixed")
        {
            var authors = await this.authorBroker.GetAllAuthorsAsync();

            IEnumerable<Author> languageAuthors;
            if (languageCode == null || languageCode == "mixed") // use all tamarins
            {
                languageAuthors = authors;
            }
            else
            {
                languageAuthors = authors.Where(t => t.FeedLanguageCode == languageCode);
            }

            var feedTasks = languageAuthors.SelectMany(t => TryReadFeeds(t)).ToArray();

            this.loggingBroker.LogInformation($"Loading feed for language: {languageCode} for {feedTasks.Length} authors");

            var syndicationItems = await Task.WhenAll(feedTasks).ConfigureAwait(false);
            var combinedFeed = GetCombinedFeed(syndicationItems.SelectMany(f => f), languageCode, languageAuthors, numberOfItems);
            return combinedFeed;
        }

        private IEnumerable<Task<IEnumerable<SyndicationItem>>> TryReadFeeds(Author tamarin)
        {
            return tamarin.FeedUris.Select(uri => TryReadFeed(tamarin, uri.AbsoluteUri));
        }

        private async Task<IEnumerable<SyndicationItem>> TryReadFeed(Author tamarin, string feedUri)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(context => ReadFeed(feedUri), new Context(feedUri)).ConfigureAwait(false);
            }
            catch (FailedFeedException ex)
            {
                this.loggingBroker.LogError(ex, $"{tamarin.FirstName} {tamarin.LastName}'s feed of {ex.Data["FeedUri"]} failed to load.");
            }

            return Array.Empty<SyndicationItem>();
        }

        private async Task<IEnumerable<SyndicationItem>> ReadFeed(string feedUri)
        {
            HttpResponseMessage response;
            try
            {
                this.loggingBroker.LogInformation($"Loading feed {feedUri}");

                response = await httpClient.GetAsync(feedUri).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    using var feedStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using var reader = XmlReader.Create(feedStream);
                    var feed = SyndicationFeed.Load(reader);
                    var filteredItems = feed.Items;

                    return filteredItems;
                }
            }
            catch (HttpRequestException hex)
            {
                throw new FailedFeedException("Loading remote syndication feed failed", hex)
                    .WithData("FeedUri", feedUri);
            }
            catch (WebException ex)
            {
                throw new FailedFeedException("Loading remote syndication feed timed out", ex)
                    .WithData("FeedUri", feedUri);
            }
            catch (XmlException ex)
            {
                throw new FailedFeedException("Failed parsing remote syndication feed", ex)
                    .WithData("FeedUri", feedUri);
            }
            catch (TaskCanceledException ex)
            {
                throw new FailedFeedException("Reading feed timed out", ex)
                    .WithData("FeedUri", feedUri);
            }
            catch (OperationCanceledException opcex)
            {
                throw new FailedFeedException("Reading feed timed out", opcex)
                    .WithData("FeedUri", feedUri);
            }

            throw new FailedFeedException("Loading remote syndication feed failed.")
                .WithData("FeedUri", feedUri)
                .WithData("HttpStatusCode", (int)response.StatusCode);
        }

        private SyndicationFeed GetCombinedFeed(IEnumerable<SyndicationItem> items, string languageCode,
            IEnumerable<Author> authors, int? numberOfItems)
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

            var orderedItems = items
                .Where(item =>
                    GetMaxTime(item) <= DateTimeOffset.UtcNow)
                .OrderByDescending(item => GetMaxTime(item));

            var feed = new SyndicationFeed(
               rssFeedTitle,
               rssFeedDescription,
                new Uri(rssFeedUrl),
                numberOfItems.HasValue ? orderedItems.Take(numberOfItems.Value) : orderedItems)
            {
                ImageUrl = new Uri(rssFeedImageUrl),
                Copyright = new TextSyndicationContent("The copyright for each post is retained by its author."),
                Language = languageCode,
                LastUpdatedTime = DateTimeOffset.UtcNow
            };

            foreach (var author in authors)
            {
                feed.Contributors.Add(new SyndicationPerson(
                    author.EmailAddress, $"{author.FirstName} {author.LastName}", author.WebSite.ToString()));
            }

            return feed;
        }

        private static DateTimeOffset GetMaxTime(SyndicationItem item)
        {
            try
            {
                return new[] { item.PublishDate.UtcDateTime, item.LastUpdatedTime.UtcDateTime }.Max();
            }
            catch
            {
                return item.PublishDate.UtcDateTime;
            }
        }
    }
}