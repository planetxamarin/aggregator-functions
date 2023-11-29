// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Authors.Models.Authors.Exceptions;

namespace PlanetDotnet.Services.Foundations.Authors
{
    public partial class AuthorService
    {
        private delegate ValueTask<IEnumerable<Author>> ReturningAuthorsFunction();

        private async ValueTask<IEnumerable<Author>> TryCatch(ReturningAuthorsFunction returningAuthorsFunction)
        {
            try
            {
                return await returningAuthorsFunction();
            }
            catch (TargetInvocationException targetInvocationException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(targetInvocationException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
        }

        private AuthorDependencyException CreateAndLogDependencyException(
            Exception exception)
        {
            var authorDependencyException = 
                new AuthorDependencyException(exception);
            
            this.loggingBroker.LogCritical(authorDependencyException);

            return authorDependencyException;
        }
    }
}
