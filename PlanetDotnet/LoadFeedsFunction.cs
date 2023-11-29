// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Serializations;
using PlanetDotnet.Brokers.Storages;
using PlanetDotnet.Services.Foundations.Feeds;

namespace PlanetDotnet
{
    public class LoadFeedsFunction
    {
        private readonly IFeedService feedService;
        private readonly IStorageBroker storageBroker;
        private readonly IAuthorBroker authorBroker;
        private readonly ISerializationBroker serializationBroker;

        public LoadFeedsFunction(
            IFeedService feedService,
            IStorageBroker storageBroker,
            IAuthorBroker authorBroker,
            ISerializationBroker serializationBroker)
        {
            this.feedService = feedService;
            this.storageBroker = storageBroker;
            this.authorBroker = authorBroker;
            this.serializationBroker = serializationBroker;
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
                        var feed = await feedService.LoadFeedAsync(null, language);
                        using var stream = await this.serializationBroker.SerializeFeedAsync(feed);
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
    }
}
