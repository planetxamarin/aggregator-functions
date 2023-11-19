// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace PlanetDotnet.Brokers.Storages
{
    public interface IStorageBroker
    {
        ValueTask UploadBlobAsync(string language, Stream content);
        ValueTask<Stream> ReadBlobAsync(string language);
    }
}
