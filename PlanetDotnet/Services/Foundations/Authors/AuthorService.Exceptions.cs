// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            catch (ArgumentNullException argumentNullException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(argumentNullException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(invalidOperationException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (AggregateException aggregateException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(aggregateException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(operationCanceledException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(fileNotFoundException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(directoryNotFoundException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (IOException ioException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(ioException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (JsonSerializationException jsonSerializationException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(jsonSerializationException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
            }
            catch (JsonReaderException jsonReaderException)
            {
                var failedAuthorStorageException =
                    new FailedAuthorStorageException(jsonReaderException);

                throw CreateAndLogDependencyException(failedAuthorStorageException);
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
