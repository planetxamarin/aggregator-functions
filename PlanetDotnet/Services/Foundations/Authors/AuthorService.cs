// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;

namespace PlanetDotnet.Services.Foundations.Authors
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorBroker authorBroker;

        public AuthorService(IAuthorBroker authorBroker) =>
            this.authorBroker = authorBroker;

        public async ValueTask<IEnumerable<Author>> RetrieveAllAuthorsAsync() =>
            await this.authorBroker.GetAllAuthorsAsync();
    }
}
