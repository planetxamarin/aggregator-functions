// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace PlanetDotnet.Brokers.Feeds
{
    internal class FeedBroker : IFeedBroker
    {
        private readonly HttpClient httpClient;

        public FeedBroker(HttpClient httpClient) =>
            this.httpClient = httpClient;

        public async Task<SyndicationFeed> ReadFeedAsync(string feedUri)
        {
            var response = await httpClient.GetAsync(feedUri).ConfigureAwait(false);
            using var feedStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using var reader = XmlReader.Create(feedStream, settings);
            var feed = SyndicationFeed.Load(reader);

            return feed;
        }
    }
}
