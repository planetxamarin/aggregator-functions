// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace PlanetDotnet.Brokers.Feeds
{
    public class FeedBroker : IFeedBroker
    {
        private readonly HttpClient httpClient;

        public FeedBroker(HttpClient httpClient) =>
            this.httpClient = httpClient;

        public async ValueTask<IEnumerable<SyndicationItem>> ReadFeedAsync(string feedUri)
        {
            var response = await httpClient.GetAsync(feedUri).ConfigureAwait(false);
            using var feedStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var reader = XmlReader.Create(feedStream);
            var feed = SyndicationFeed.Load(reader);

            return feed.Items;
        }
    }
}
