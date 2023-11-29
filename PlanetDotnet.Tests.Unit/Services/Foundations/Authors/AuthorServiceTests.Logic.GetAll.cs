// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanetDotnet.Authors.Models.Authors;
using Xunit;

namespace PlanetDotnet.Tests.Unit.Services.Foundations.Authors
{
    public partial class AuthorServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAuthorsAsync()
        {
            // given
            IEnumerable<Author> randomAuthors = CreateRandomAuthors();
            IEnumerable<Author> storageAuthors = randomAuthors;
            IEnumerable<Author> expectedAuthors = storageAuthors;

            this.authorBrokerMock.Setup(broker =>
                broker.GetAllAuthorsAsync())
                    .ReturnsAsync(expectedAuthors);

            // when
            var actualAuthors =
                await this.authorService.RetrieveAllAuthorsAsync();

            // then
            actualAuthors.Should().BeEquivalentTo(expectedAuthors);

            this.authorBrokerMock.Verify(broker =>
                broker.GetAllAuthorsAsync(),
                    Times.Once());

            this.authorBrokerMock.VerifyNoOtherCalls();
        }
    }
}
