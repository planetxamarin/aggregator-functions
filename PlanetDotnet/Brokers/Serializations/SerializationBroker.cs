// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace PlanetDotnet.Brokers.Serializations
{
    internal class SerializationBroker : ISerializationBroker
    {
        public SyndicationFeed DeserializeFeed(Stream feedStream)
        {
            feedStream.Position = 0;
            using var xmlReader = XmlReader.Create(feedStream, new XmlReaderSettings
            {
                Async = true
            });

            var feed = SyndicationFeed.Load(xmlReader);

            return feed;
        }

        public async ValueTask<Stream> SerializeFeedAsync(SyndicationFeed feed)
        {
            var memoryStream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
            {
                Async = true,
                Indent = true
            });

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("rss");
            xmlWriter.WriteAttributeString("version", "2.0");
            xmlWriter.WriteStartElement("channel");
            xmlWriter.WriteElementString("title", feed.Title?.Text ?? string.Empty);
            xmlWriter.WriteElementString("link", feed.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty);
            xmlWriter.WriteElementString("description", feed.Description?.Text ?? string.Empty);

            if (feed.Language != null)
                xmlWriter.WriteElementString("language", feed.Language);

            if (feed.LastUpdatedTime != DateTimeOffset.MinValue)
                xmlWriter.WriteElementString("lastBuildDate", feed.LastUpdatedTime.ToString("R"));

            // Write items
            foreach (var item in feed.Items)
            {
                xmlWriter.WriteStartElement("item");

                xmlWriter.WriteElementString("title", item.Title?.Text ?? string.Empty);
                xmlWriter.WriteElementString("link", item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty);
                xmlWriter.WriteElementString("description", item.Summary?.Text ?? string.Empty);

                if (item.Id != null)
                    xmlWriter.WriteElementString("guid", item.Id);

                if (item.PublishDate != DateTimeOffset.MinValue)
                    xmlWriter.WriteElementString("pubDate", item.PublishDate.ToString("R"));

                xmlWriter.WriteEndElement(); // item
            }

            xmlWriter.WriteEndElement(); // channel
            xmlWriter.WriteEndElement(); // rss

            await xmlWriter.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
