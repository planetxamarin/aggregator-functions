// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using PlanetDotnet.Authors.Models.Authors.Exceptions;
using Xunit;

namespace PlanetDotnet.Tests.Unit.Services.Foundations.Authors
{
    public partial class AuthorServiceTests
    {

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAndLogIt(
            Exception dependencyException)
        {
            // given 
            var failedStorageException =
                new FailedAuthorStorageException(dependencyException);

            var expectedAuthorDependencyException =
                new AuthorDependencyException(failedStorageException);

            this.authorBrokerMock.Setup(broker =>
                broker.GetAllAuthorsAsync())
                    .Throws(dependencyException);

            // when
            var retrieveAllAuthorsTask =
                  this.authorService.RetrieveAllAuthorsAsync();

            // then
            await Assert.ThrowsAsync<AuthorDependencyException>(() =>
                retrieveAllAuthorsTask.AsTask());

            this.authorBrokerMock.Verify(broker =>
                broker.GetAllAuthorsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAuthorDependencyException))),
                        Times.Once);

            this.authorBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var expectedAuthorServiceException =
                new AuthorServiceException(serviceException);

            this.authorBrokerMock.Setup(broker =>
                broker.GetAllAuthorsAsync())
                    .ThrowsAsync(serviceException);

            // when
            var retrievedAuthorTask =
                this.authorService.RetrieveAllAuthorsAsync();

            // then
            await Assert.ThrowsAsync<AuthorServiceException>(() =>
                retrievedAuthorTask.AsTask());

            this.authorBrokerMock.Verify(broker =>
                broker.GetAllAuthorsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAuthorServiceException))),
                        Times.Once);

            this.authorBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
