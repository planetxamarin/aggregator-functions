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
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Services.Feeds;

[assembly: FunctionsStartup(typeof(PlanetDotnet.Startup))]
namespace PlanetDotnet
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ILogger, Logger<LoggingBroker>>();
            builder.Services.AddScoped<ILoggingBroker, LoggingBroker>();
            builder.Services.AddScoped<IDateTimeBroker, DateTimeBroker>();
            builder.Services.AddScoped<IAuthorBroker, AuthorBroker>();
            builder.Services.AddScoped<IFeedBroker, FeedBroker>();
            builder.Services.AddScoped<IFeedService, FeedService>();
        }
    }
}
