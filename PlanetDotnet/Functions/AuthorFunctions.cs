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
using PlanetDotnet.Services.Foundations.Authors;

namespace PlanetDotnet.Functions
{
    public class AuthorFunctions
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IAuthorService authorService;

        public AuthorFunctions(
            ILoggingBroker loggingBroker,
            IAuthorService authorService)
        {
            this.loggingBroker = loggingBroker;
            this.authorService = authorService;
        }

        [FunctionName("GetAllAuthors")]
        public async Task<IActionResult> GetAllAuthorsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "authors")] HttpRequest req)
        {
            try
            {
                this.loggingBroker.LogInformation("Started loading all authors.");

                var authors = await this.authorService.RetrieveAllAuthorsAsync();

                this.loggingBroker.LogInformation("Finished loading all authors.");
                return new OkObjectResult(authors);
            }
            catch
            {
                return new InternalServerErrorResult();
            }
        }
    }
}
