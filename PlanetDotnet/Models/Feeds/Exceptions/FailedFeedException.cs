// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Models.Feeds.Exceptions
{
    public class FailedFeedException : Exception
    {
        public FailedFeedException(string message)
            : base(message)
        { }

        public FailedFeedException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
