// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Storages;
using PlanetDotnet.Services.CombinedFeeds;

namespace PlanetDotnet
{
    public class LoadFeedsFunction
    {
        private readonly ICombinedFeedService feedService;
        private readonly IStorageBroker storageBroker;
        private readonly IAuthorBroker authorBroker;

        public LoadFeedsFunction(
            ICombinedFeedService feedService,
            IStorageBroker storageBroker,
            IAuthorBroker authorBroker)
        {
            this.feedService = feedService;
            this.storageBroker = storageBroker;
            this.authorBroker = authorBroker;
        }

        [FunctionName("LoadFeedsFunction")]
        public async Task Run(
            [TimerTrigger("0 0 */1 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"Load feeds Timer trigger function executed at: {DateTime.Now}");

            try
            {
                await this.storageBroker.InitializeAsync();

                var authors = await this.authorBroker.GetAllAuthorsAsync();

                var languages = authors.Select(author => author.FeedLanguageCode).Distinct().ToList();

                var mainCulture = CultureInfo.CurrentCulture;

                foreach (var language in languages)
                {
                    try
                    {
                        CultureInfo.CurrentCulture = new CultureInfo(language);
                        log.LogInformation($"Loading {language} combined author feed");
                        var feed = await feedService.LoadFeed(null, language);
                        using var stream = await SerializeFeed(feed);
                        await this.storageBroker.UploadBlobAsync(language, stream);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "error");
                    }
                }

                CultureInfo.CurrentCulture = mainCulture;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "error");
            }
            log.LogInformation($"Load feeds Finished at: {DateTime.Now}");
        }

        private static async Task<Stream> SerializeFeed(SyndicationFeed feed)
        {
            FixInvalidLastUpdatedTime(feed);

            var memoryStream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
            {
                Async = true
            });

            var rssFormatter = new Rss20FeedFormatter(feed);
            rssFormatter.WriteTo(xmlWriter);
            await xmlWriter.FlushAsync();

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        private static void FixInvalidLastUpdatedTime(SyndicationFeed feed)
        {
            foreach (var item in feed.Items)
            {
                // Check if both PublishDate and LastUpdatedTime are invalid
                if (item.PublishDate == DateTimeOffset.MinValue && item.LastUpdatedTime == DateTimeOffset.MinValue)
                {
                    item.PublishDate = DateTimeOffset.UtcNow;
                    item.LastUpdatedTime = DateTimeOffset.UtcNow;
                }
                else
                {
                    // If only PublishDate is invalid, set it to LastUpdatedTime
                    if (item.PublishDate == DateTimeOffset.MinValue)
                    {
                        item.PublishDate = item.LastUpdatedTime;
                    }

                    // If only LastUpdatedTime is invalid, set it to PublishDate
                    if (item.LastUpdatedTime == DateTimeOffset.MinValue)
                    {
                        item.LastUpdatedTime = item.PublishDate;
                    }
                }
            }
        }

    }

}
