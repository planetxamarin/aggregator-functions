// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Authors.Models.Authors.Exceptions
{
    public class FailedAuthorStorageException : Exception
    {
        public FailedAuthorStorageException(Exception innerException)
            : base("Failed authors storage error occurred, contact support.", innerException)
        { }
    }
}
