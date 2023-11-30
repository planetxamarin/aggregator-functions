// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Services.Foundations.Previews;

namespace PlanetDotnet.Functions
{
    public class PreviewFunctions
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IPreviewService previewService;

        public PreviewFunctions(
            ILoggingBroker loggingBroker,
            IPreviewService previewService)
        {
            this.loggingBroker = loggingBroker;
            this.previewService = previewService;
        }

        [FunctionName("GetAllPreviews")]
        public async Task<IActionResult> GetAllPreviewsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "previews")] HttpRequest req)
        {
            try
            {
                this.loggingBroker.LogInformation("Started loading preview items.");

                var previews = await this.previewService.RetrieveAllPreviewsAsync("en");

                this.loggingBroker.LogInformation("Finished loading preview items.");
                return new OkObjectResult(previews);
            }
            catch
            {
                return new InternalServerErrorResult();
            }
        }
    }
}
