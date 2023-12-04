// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace PlanetDotnet.Models.Previews
{
    public class Preview
    {
        public string AuthorName { get; set; }
        public string Gravatar { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Body { get; set; }
        public DateTimeOffset PublishDate { get; set; }
    }
}
