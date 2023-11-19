// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.IO;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace PlanetDotnet.Brokers.Serializations
{
    internal class SerializationBroker : ISerializationBroker
    {
        public ValueTask<SyndicationFeed> DeserializeFeedAsync(Stream feedStream)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask<Stream> SerializeFeedAsync(SyndicationFeed feed)
        {
            var memoryStream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
            {
                Async = true
            });

            var rssFormatter = new Rss20FeedFormatter(feed);
            rssFormatter.WriteTo(xmlWriter);
            await xmlWriter.FlushAsync();

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
