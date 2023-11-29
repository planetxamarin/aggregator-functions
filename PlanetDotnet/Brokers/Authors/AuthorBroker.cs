// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Authors.Services;

namespace PlanetDotnet.Brokers.Authors
{
    public class AuthorBroker : IAuthorBroker
    {
        public async ValueTask<IEnumerable<Author>> GetAllAuthorsAsync() =>
           await AuthorService.GetAllAuthors();
    }
}
