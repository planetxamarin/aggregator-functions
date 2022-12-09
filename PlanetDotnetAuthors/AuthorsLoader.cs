using Newtonsoft.Json;
using PlanetDotnetAuthors.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PlanetDotnetAuthors
{
    public static class AuthorsLoader
    {
        public static async Task<IEnumerable<Author>> GetAllAuthors()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var authorsResourceNames = resourceNames.Where(res =>
                res.StartsWith("PlanetDotnetAuthors", StringComparison.OrdinalIgnoreCase) &&
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
