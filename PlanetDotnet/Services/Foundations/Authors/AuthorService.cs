// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Hashes;
using PlanetDotnet.Brokers.Loggings;

namespace PlanetDotnet.Services.Foundations.Authors
{
    public partial class AuthorService : IAuthorService
    {
        private readonly IAuthorBroker authorBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IHashBroker hashBroker;

        public AuthorService(
            IAuthorBroker authorBroker,
            ILoggingBroker loggingBroker,
            IHashBroker hashBroker)
        {
            this.authorBroker = authorBroker;
            this.loggingBroker = loggingBroker;
            this.hashBroker = hashBroker;
        }

        public string GetGravatarImage(Author member)
        {
            var defaultImage = Environment.GetEnvironmentVariable(
                    variable: "DefaultGravatarImage",
                    target: EnvironmentVariableTarget.Process);

            const int size = 200;
            var hash = member?.GravatarHash;

            if (string.IsNullOrWhiteSpace(hash))
            {
                var email = member?.EmailAddress?.Trim()?
                    .ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(email))
                {
                    hash = this.hashBroker.Hash(value: email)
                        .ToLowerInvariant();
                }
            }

            defaultImage = HttpUtility.UrlEncode(defaultImage);
            return $"//www.gravatar.com/avatar/{hash}.jpg?s={size}&d={defaultImage}";
        }

        public ValueTask<IEnumerable<Author>> RetrieveAllAuthorsAsync() =>
            TryCatch(async () => await this.authorBroker.GetAllAuthorsAsync());
    }
}
