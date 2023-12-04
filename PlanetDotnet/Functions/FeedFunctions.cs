// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanetDotnet.Services.Processings.Feeds;

namespace PlanetDotnet.Functions
{
    public class FeedFunctions
    {
        private readonly IFeedProcessingService feedProcessingService;

        public FeedFunctions(IFeedProcessingService feedProcessingService) =>
            this.feedProcessingService = feedProcessingService;

        [FunctionName("LoadFeedsFunction")]
        public async Task Run(
            [TimerTrigger("0 0 */1 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Load feeds Timer trigger function executed at: {DateTime.Now}");

                await feedProcessingService.ProcessFeedLoadingAsync();

                log.LogInformation($"Load feeds Finished at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Loading feeds could'nt be processed.");
            }
        }
    }
}
