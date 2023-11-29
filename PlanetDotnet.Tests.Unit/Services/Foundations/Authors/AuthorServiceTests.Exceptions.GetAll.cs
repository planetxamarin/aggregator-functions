// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Reflection;
using FluentAssertions;
using Moq;
using PlanetDotnet.Authors.Models.Authors.Exceptions;
using Xunit;

namespace PlanetDotnet.Tests.Unit.Services.Foundations.Authors
{
    public partial class AuthorServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenAssemblyExceptionOccursAndLogIt()
        {
            // given
            TargetInvocationException targetInvocationException
                = GetTargetInvocationException();

            var failedStorageException =
                new FailedAuthorStorageException(targetInvocationException);

            var expectedAuthorDependencyException =
                new AuthorDependencyException(failedStorageException);

            this.authorBrokerMock.Setup(broker =>
                broker.GetAllAuthorsAsync())
                    .Throws(targetInvocationException);

            // when
            Action retrieveAllAuthorsAction = () =>
                this.authorService.RetrieveAllAuthorsAsync();

            AuthorDependencyException actualAuthorDependencyException =
                Assert.Throws<AuthorDependencyException>(retrieveAllAuthorsAction);

            // then
            actualAuthorDependencyException.Should()
                .BeEquivalentTo(expectedAuthorDependencyException);

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
    }
}
