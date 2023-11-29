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
using PlanetDotnet.Services.Foundations.Feeds;

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
            try
            {

                var memoryStream = new MemoryStream();
                using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
                {
                    Async = true,
                    Indent = true // Makes the output XML easier to read, optional
                });

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("rss");
                xmlWriter.WriteAttributeString("version", "2.0");

                xmlWriter.WriteStartElement("channel");

                // Write channel elements
                xmlWriter.WriteElementString("title", feed.Title?.Text ?? string.Empty);
                xmlWriter.WriteElementString("link", feed.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty);
                xmlWriter.WriteElementString("description", feed.Description?.Text ?? string.Empty);

                // Optional channel elements (add as needed)
                if (feed.Language != null)
                    xmlWriter.WriteElementString("language", feed.Language);

                if (feed.LastUpdatedTime != DateTimeOffset.MinValue)
                    xmlWriter.WriteElementString("lastBuildDate", feed.LastUpdatedTime.ToString("R")); // RFC-822 format

                // More optional elements like image, categories, etc. can be added here

                // Write items
                foreach (var item in feed.Items)
                {
                    xmlWriter.WriteStartElement("item");

                    xmlWriter.WriteElementString("title", item.Title?.Text ?? string.Empty);
                    xmlWriter.WriteElementString("link", item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty);
                    xmlWriter.WriteElementString("description", item.Summary?.Text ?? string.Empty);

                    // Other optional elements for each item (guid, pubDate, etc.)
                    if (item.Id != null)
                        xmlWriter.WriteElementString("guid", item.Id);

                    if (item.PublishDate != DateTimeOffset.MinValue)
                        xmlWriter.WriteElementString("pubDate", item.PublishDate.ToString("R")); // RFC-822 format

                    // Additional elements for each item can be added here

                    xmlWriter.WriteEndElement(); // item
                }

                xmlWriter.WriteEndElement(); // channel
                xmlWriter.WriteEndElement(); // rss

                await xmlWriter.FlushAsync();
                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
