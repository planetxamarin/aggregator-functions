// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Loggings;

namespace PlanetDotnet.Services.Foundations.Authors
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorBroker authorBroker;
        private readonly ILoggingBroker loggingBroker;

        public AuthorService(
            IAuthorBroker authorBroker,
            ILoggingBroker loggingBroker)
        {
            this.authorBroker = authorBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<IEnumerable<Author>> RetrieveAllAuthorsAsync() =>
            await this.authorBroker.GetAllAuthorsAsync();
    }
}
