// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PlanetDotnet.Brokers.Hashes
{
    public class HashBroker : IHashBroker
    {
        public string Hash(string value)
        {
            var unhashedBytes = Encoding.UTF8.GetBytes(value);
            var hashedBytes = MD5.Create().ComputeHash(unhashedBytes);

            var hashedString = string.Join(string.Empty,
                hashedBytes.Select(b => b.ToString("X2")).ToArray());
            return hashedString;
        }
    }
}
