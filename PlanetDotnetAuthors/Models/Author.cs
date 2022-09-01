using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanetDotnetAuthors.Models
{
    public class Author
    {
        [JsonProperty("firstName", Required = Required.DisallowNull)]
        public string FirstName { get; set; }
        [JsonProperty("lastName", Required = Required.DisallowNull)]
        public string LastName { get; set; }
        [JsonProperty("stateOrRegion", Required = Required.DisallowNull)]
        public string StateOrRegion { get; set; }
        [EmailAddress]
        [JsonProperty("emailAddress", Required = Required.Always)]
        public string EmailAddress { get; set; }
        [JsonProperty("tagOrBio", Required = Required.DisallowNull)]
        public string ShortBioOrTagLine { get; set; }
        [Url]
        [JsonProperty("webSite", Required = Required.Always)]
        public Uri WebSite { get; set; }
        [JsonProperty("twitterHandle", Required = Required.DisallowNull)]
        public string TwitterHandle { get; set; }
        [JsonProperty("githubHandle", Required = Required.Always)]
        public string GitHubHandle { get; set; }
        [JsonProperty("gravatarHash", Required = Required.DisallowNull)]
        public string GravatarHash { get; set; }
        [JsonProperty("feedUris", Required = Required.Always)]
        public IEnumerable<Uri> FeedUris { get; set; }
        [JsonProperty("position", Required = Required.DisallowNull)]
        public GeoPosition Position { get; set; }

        // In ISO 639-1, lowercase, 2 letters
        // https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        [JsonProperty("languageCode", Required = Required.Always)]
        public string FeedLanguageCode { get; set; }
    }
}
