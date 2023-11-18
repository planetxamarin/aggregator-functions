// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace PlanetDotnet.Brokers.Feeds
{
    public interface IFeedBroker
    {
        ValueTask<IEnumerable<SyndicationItem>> ReadFeedAsync(string feedUri);
    }
}
