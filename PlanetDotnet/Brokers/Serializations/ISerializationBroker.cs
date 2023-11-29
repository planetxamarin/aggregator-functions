// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.IO;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace PlanetDotnet.Brokers.Serializations
{
    public interface ISerializationBroker
    {
        ValueTask<Stream> SerializeFeedAsync(SyndicationFeed feed);
        SyndicationFeed DeserializeFeed(Stream feedStream);
    }
}
