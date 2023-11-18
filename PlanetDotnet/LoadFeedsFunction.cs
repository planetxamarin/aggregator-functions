// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Authors.Services;
using PlanetDotnet.Infrastructure;

namespace PlanetDotnet
{
    public static class LoadFeedsFunction
    {
        [FunctionName("LoadFeedsFunction")]
        public static async Task Run(
            [TimerTrigger("0 0 */1 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var rssFeedTitle = GetEnvironmentVariable("RssFeedTitle");
            var rssFeedDescription = GetEnvironmentVariable("RssFeedDescription");
            var rssFeedUrl = GetEnvironmentVariable("RssFeedUrl");
            var rssFeedImageUrl = GetEnvironmentVariable("RssFeedImageUrl");

            var authors = await AuthorService.GetAllAuthors();
            var languages = authors.Select(author => author.FeedLanguageCode).Distinct().ToList();
            languages.Add("mixed");
            var feedSource =
                new CombinedFeedSource(
                    authors,
                    log,
                    rssFeedTitle,
                    rssFeedDescription,
                    rssFeedUrl,
                    rssFeedImageUrl);

            var blobConnectString = GetEnvironmentVariable("FeedBlobStorage");
            var container = new BlobContainerClient(blobConnectString, "feeds");
            await container.CreateIfNotExistsAsync();
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            foreach (var language in languages)
            {
                try
                {
                    log.LogInformation($"Loading {language} combined author feed");
                    var feed = await feedSource.LoadFeed(null, language);
                    using var stream = await SerializeFeed(feed);
                    await UploadBlob(container, stream, language, log);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, ex.Message);
                }
            }
        }

        private static async Task UploadBlob(BlobContainerClient container, Stream feedStream, string language, ILogger log)
        {
            var feedName = $"feed.{language}.rss";
            var blob = container.GetBlobClient(feedName);
            await blob.UploadAsync(feedStream, overwrite: true);

            log.LogInformation($"Uploaded {feedName} to {blob.Uri}");
        }

        private static async Task<Stream> SerializeFeed(SyndicationFeed feed)
        {
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

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
