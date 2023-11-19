// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Authors.Services;
using PlanetDotnet.Infrastructure;
using PlanetDotnet.Services.Feeds;

namespace PlanetDotnet
{
    public class LoadFeedsFunction
    {
        private readonly IFeedService feedService;

        public LoadFeedsFunction(IFeedService feedService) =>
            this.feedService = feedService;

        [FunctionName("LoadFeedsFunction")]
        public async Task Run(
            [TimerTrigger("0 0 */1 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"Load feeds Timer trigger function executed at: {DateTime.Now}");

            await this.feedService.LoadFeedAsync();

            log.LogInformation($"Load feeds Finished at: {DateTime.Now}");
        }
    }
}
