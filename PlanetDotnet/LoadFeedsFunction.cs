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
                
            }
            catch (Exception ex)
            {
                log.LogError(ex, "error");
            }
            log.LogInformation($"Load feeds Finished at: {DateTime.Now}");
        }
    }
}
