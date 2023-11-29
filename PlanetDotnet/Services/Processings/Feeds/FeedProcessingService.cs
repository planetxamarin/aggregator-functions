// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Brokers.Serializations;
using PlanetDotnet.Brokers.Storages;
using PlanetDotnet.Services.Foundations.Feeds;

namespace PlanetDotnet.Services.Processings.Feeds
{
    public class FeedProcessingService : IFeedProcessingService
    {
        private readonly IFeedService feedService;
        private readonly IStorageBroker storageBroker;
        private readonly IAuthorBroker authorBroker;
        private readonly ISerializationBroker serializationBroker;
        private readonly ILoggingBroker loggingBroker;

        public FeedProcessingService(
            IFeedService feedService,
            IStorageBroker storageBroker,
            IAuthorBroker authorBroker,
            ISerializationBroker serializationBroker,
            ILoggingBroker loggingBroker)
        {
            this.feedService = feedService;
            this.storageBroker = storageBroker;
            this.authorBroker = authorBroker;
            this.serializationBroker = serializationBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask ProcessFeedLoadingAsync()
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
                    this.loggingBroker.LogInformation($"Loading {language} combined author feed");
                    var feed = await feedService.LoadFeedAsync(null, language);
                    using var stream = await this.serializationBroker.SerializeFeedAsync(feed);
                    await this.storageBroker.UploadBlobAsync(language, stream);
                }
                catch (Exception ex)
                {
                    this.loggingBroker.LogError(ex, "error");
                }
            }

            CultureInfo.CurrentCulture = mainCulture;
        }
    }
}
