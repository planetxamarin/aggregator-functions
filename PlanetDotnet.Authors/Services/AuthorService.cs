// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlanetDotnet.Authors.Models.Authors;

namespace PlanetDotnet.Authors.Services
{
    public static class AuthorService
    {
        public static async Task<IEnumerable<Author>> GetAllAuthors()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var authorsResourceNames = resourceNames.Where(res =>
                res.StartsWith("PlanetDotnet.Authors", StringComparison.OrdinalIgnoreCase) &&
                res.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

            var authorsTasks = authorsResourceNames.Select(name => ReadAuthor(assembly, name));
            var authors = await Task.WhenAll(authorsTasks).ConfigureAwait(false);
            return authors;
        }

        private static async Task<Author> ReadAuthor(Assembly assembly, string authorResourceName)
        {
            using var stream = assembly.GetManifestResourceStream(authorResourceName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false);
            var author = JsonConvert.DeserializeObject<Author>(json);
            return author;
        }
    }
}
