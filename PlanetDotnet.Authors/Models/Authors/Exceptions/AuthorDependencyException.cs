// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Authors.Models.Authors.Exceptions
{
    public class AuthorDependencyException : Exception
    {
        public AuthorDependencyException(Exception innerException) :
            base(message: "Author dependency error occurred, contact support.", innerException)
        { }
    }
}
