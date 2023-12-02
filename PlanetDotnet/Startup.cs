// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.DateTimes;
using PlanetDotnet.Brokers.Feeds;
using PlanetDotnet.Brokers.Hashes;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Brokers.Serializations;
using PlanetDotnet.Brokers.Storages;
using PlanetDotnet.Services.Foundations.Authors;
using PlanetDotnet.Services.Foundations.Feeds;
using PlanetDotnet.Services.Processings.Feeds;
using PlanetDotnet.Services.Processings.Previews;

[assembly: FunctionsStartup(typeof(PlanetDotnet.Startup))]
namespace PlanetDotnet
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<ILogger<LoggingBroker>, Logger<LoggingBroker>>();
            builder.Services.AddSingleton<ILoggingBroker, LoggingBroker>();
            builder.Services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
            builder.Services.AddSingleton<ISerializationBroker, SerializationBroker>();
            builder.Services.AddSingleton<IStorageBroker, StorageBroker>();
            builder.Services.AddSingleton<IAuthorBroker, AuthorBroker>();
            builder.Services.AddSingleton<IFeedBroker, FeedBroker>();
            builder.Services.AddSingleton<IHashBroker, HashBroker>();
            builder.Services.AddSingleton<IFeedService, FeedService>();
            builder.Services.AddSingleton<IAuthorService, AuthorService>();
            builder.Services.AddSingleton<IPreviewProcessingService, PreviewProcessingService>();
            builder.Services.AddSingleton<IFeedProcessingService, FeedProcessingService>();
        }
    }
}
