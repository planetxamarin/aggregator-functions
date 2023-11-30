// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Humanizer;
using PlanetDotnet.Authors.Models.Authors;
using PlanetDotnet.Brokers.Storages;
using PlanetDotnet.Extensions;
using PlanetDotnet.Models.Previews;
using PlanetDotnet.Services.Foundations.Authors;

namespace PlanetDotnet.Services.Foundations.Previews
{
    public partial class PreviewService : IPreviewService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IAuthorService authorService;
        private const int MaxLength = 400;

        public PreviewService(
            IStorageBroker storageBroker,
            IAuthorService authorService)
        {
            this.storageBroker = storageBroker;
            this.authorService = authorService;
        }

        public async ValueTask<IEnumerable<Preview>> RetrieveAllPreviewsAsync(string language)
        {
            var authors = await this.authorService.RetrieveAllAuthorsAsync();

            var stream = await this.storageBroker.ReadBlobAsync(language);

            using XmlReader reader = XmlReader.Create(stream);

            var feed = SyndicationFeed.Load(reader);

            return MapToPreviewList(feed, authors);
        }

        private IEnumerable<Preview> MapToPreviewList(
            SyndicationFeed feed,
            IEnumerable<Author> authors)
        {
            foreach (var item in feed.Items)
            {
                var author = authors.FirstOrDefault(author =>
                    MatchesAuthorUrls(
                        author: author,
                        urls: item.Links.Select(l => l.Uri),
                        item: item));

                string authorName;

                if (author != null)
                {
                    authorName = $"{author.FirstName} {author.LastName}".Trim();
                }
                else
                {
                    var creator = item.ElementExtensions.FirstOrDefault(x =>
                        x.OuterName == "creator" && x.OuterNamespace == "http://purl.org/dc/elements/1.1/");

                    if (creator != null)
                    {
                        authorName = creator.GetObject<XmlElement>().Value ?? string.Empty;
                    }
                    else
                    {
                        authorName = string.Join(", ", item.Authors.Select(a => $"{a.Name} {a.Email}".Trim()));
                    }
                }

                var link = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty;
                var html = item.Content?.ToHtml() ?? item.Summary?.ToHtml() ?? string.Empty;

                var gravatar = this.authorService.GetGravatarImage(author);

                yield return new Preview
                {
                    AuthorName = authorName,
                    Gravatar = gravatar,
                    Title = item.Title.Text,
                    Link = link,
                    Body = Sanitize(html),
                    PublishDate = item.PublishDate.Humanize()
                };
            }
        }

        private static bool MatchesAuthorUrls(Author author, IEnumerable<Uri> urls, SyndicationItem item)
        {
            var authorHosts = author.FeedUris.Select(au => au.Host.ToLowerInvariant()).Concat(new[] { author.WebSite.Host.ToLowerInvariant() }).ToArray();
            var feedBurnerAuthors = author.FeedUris.Where(au => au.Host.Contains("feeds.feedburner")).ToList();
            var mediumAuthors = author.FeedUris.Where(au => au.Host.Contains("medium.com")).ToList();
            var youtubeAuthors = author.FeedUris.Where(au => au.Host.Contains("youtube.com")).ToList();

            foreach (var itemUrl in urls)
            {
                var host = itemUrl.Host.ToLowerInvariant();

                if (host.Contains("medium.com"))
                {
                    if (itemUrl.Segments.Count() >= 3)
                    {
                        var mediumId = itemUrl.Segments[1].Trim('/');
                        return mediumAuthors.Any(fba => fba.AbsoluteUri.Contains(mediumId));
                    }
                }

                if (host.Contains("feedproxy.google")) //  feed burner is messed up :(
                {
                    // url will look like:
                    // feedproxy.google.com/~r/<feedburnerId>/~3/bgJNuDXwkU0/O
                    if (itemUrl.Segments.Count() >= 5)
                    {
                        var feedBurnerId = itemUrl.Segments[2].Trim('/');
                        return feedBurnerAuthors.Any(fba => fba.AbsoluteUri.Contains(feedBurnerId));
                    }
                }

                if (host.Contains("youtube.com")) //need to match youtube channel
                {
                    var channel = item?.Authors?.FirstOrDefault()?.Uri;
                    if (channel == null)
                        return false;

                    var id = channel.Replace("https://www.youtube.com/channel/", string.Empty);

                    return youtubeAuthors.Any(yt => yt.AbsoluteUri.Contains(id));
                }

                if (authorHosts.Contains(host))
                    return true;

                if (authorHosts.Contains(host.Replace("www.", "")))
                    return true;

                if (authorHosts.Contains(host.Insert(0, "www.")))
                    return true;
            }

            return false;
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return Truncate(StripHtmlTags(value), MaxLength);
        }

        private static string StripHtmlTags(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent)) return htmlContent;

            // Strip out any HTML content.
            var strippedContent = Regex.Replace(htmlContent, @"<[^>]*>", String.Empty);
            return strippedContent;
        }

        private static string Truncate(string value, int maxLength, bool includeLastSentence = true)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.Length <= maxLength) return value;

            var truncatedContent = value[..maxLength];

            if (includeLastSentence && value.IndexOf('.', maxLength) > -1)
            {
                truncatedContent += value.Substring(maxLength, value.IndexOf('.', maxLength) - maxLength + 1);
            }

            return truncatedContent;
        }

    }
}
