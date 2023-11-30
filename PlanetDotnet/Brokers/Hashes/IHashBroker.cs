// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

namespace PlanetDotnet.Brokers.Hashes
{
    public interface IHashBroker
    {
        string Hash(string value);
    }
}
