// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Extensions
{
    internal static class ExceptionExtensions
    {
        public static TException WithData<TException>(
            this TException exception,
            string key,
            object value)
            where TException : Exception
        {
            exception.Data[key] = value;
            return exception;
        }
    }
}
