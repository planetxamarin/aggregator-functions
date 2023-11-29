// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Authors.Models.Authors.Exceptions
{
    public class AuthorServiceException : Exception
    {
        public AuthorServiceException(Exception innerException)
            : base(message: "Author service error occurred, contact support.", innerException) { }
    }
}
