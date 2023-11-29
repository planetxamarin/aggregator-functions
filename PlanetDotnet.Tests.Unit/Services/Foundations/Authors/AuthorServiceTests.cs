// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using Moq;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Services.Foundations.Authors;
using Tynamix.ObjectFiller;

namespace PlanetDotnet.Tests.Unit.Services.Foundations.Authors
{
    public partial class AuthorServiceTests
    {
        private readonly Mock<IAuthorBroker> authorBrokerMock;
        private readonly IAuthorService authorService;

        public AuthorServiceTests()
        {
            this.authorBrokerMock = new Mock<IAuthorBroker>();

            this.authorService = new AuthorService(
                authorBroker: this.authorBrokerMock.Object);
        }

        private static IEnumerable<Author> CreateRandomAuthors() =>
             CreateAuthorFiller().Create(count: GetRandomNumber());

        private static Author CreateRandomAuthor() =>
            CreateAuthorFiller().Create();

        private static Filler<Author> CreateAuthorFiller() => new();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 15).GetValue();
    }
}
