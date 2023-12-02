// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Moq;
using Newtonsoft.Json;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Authors;
using PlanetDotnet.Brokers.Hashes;
using PlanetDotnet.Brokers.Loggings;
using PlanetDotnet.Services.Foundations.Authors;
using Tynamix.ObjectFiller;
using Xunit;

namespace PlanetDotnet.Tests.Unit.Services.Foundations.Authors
{
    public partial class AuthorServiceTests
    {
        private readonly Mock<IAuthorBroker> authorBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IHashBroker> hashBrokerMock;
        private readonly IAuthorService authorService;

        public AuthorServiceTests()
        {
            this.authorBrokerMock = new Mock<IAuthorBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.hashBrokerMock = new Mock<IHashBroker>();

            this.authorService = new AuthorService(
                authorBroker: this.authorBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                hashBroker: this.hashBrokerMock.Object);
        }

        private static IEnumerable<Author> CreateRandomAuthors() =>
             CreateAuthorFiller().Create(count: GetRandomNumber());

        private static Author CreateRandomAuthor() =>
            CreateAuthorFiller().Create();

        private static Filler<Author> CreateAuthorFiller() => new();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 15).GetValue();

        private static Expression<Func<Exception, bool>> SameExceptionAs(Exception expectedException) =>
            actualException => actualException.Message == expectedException.Message;

        private static TargetInvocationException GetTargetInvocationException() =>
            (TargetInvocationException)FormatterServices.GetUninitializedObject(typeof(TargetInvocationException));

        private static Exception GetException() =>
            (Exception)FormatterServices.GetUninitializedObject(typeof(Exception));

        private static string GetRandomString() => new MnemonicString().GetValue();

        public static TheoryData DependencyExceptions()
        {
            string exceptionMessage = GetRandomString();

            var exception = new Exception(exceptionMessage);

            var targetInvocationException =
                new TargetInvocationException(exception);

            var argumentNullException =
                new ArgumentNullException();

            var invalidOperationException =
                new InvalidOperationException(exceptionMessage);

            var aggregateException =
                new AggregateException();

            var operationCanceledException =
                new OperationCanceledException();

            var fileNotFoundException =
                new FileNotFoundException(exceptionMessage);

            var directoryNotFoundException =
                new DirectoryNotFoundException(exceptionMessage);

            var ioException =
                new IOException(exceptionMessage);

            var jsonSerializationException =
                new JsonSerializationException();

            var jsonReaderException =
                new JsonReaderException();


            return new TheoryData<Exception>
            {
                targetInvocationException,
                argumentNullException,
                invalidOperationException,
                aggregateException,
                operationCanceledException,
                fileNotFoundException,
                directoryNotFoundException,
                ioException,
                jsonSerializationException,
                jsonReaderException
            };
        }
    }
}
