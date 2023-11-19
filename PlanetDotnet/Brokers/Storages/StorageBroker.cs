// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace PlanetDotnet.Brokers.Storages
{
    public class StorageBroker : IStorageBroker
    {
        private readonly BlobContainerClient blobContainerClient;
        private const string BlobContainerName = "feeds";
        private const string FeedBlobStorageKey = "FeedBlobStorage";
        private const string BlobName = "newfeed.{0}.rss";

        public StorageBroker()
        {
            var blobConnectString = Environment.GetEnvironmentVariable(
                variable: FeedBlobStorageKey,
                target: EnvironmentVariableTarget.Process);

            this.blobContainerClient = new BlobContainerClient(
                connectionString: blobConnectString,
                blobContainerName: BlobContainerName);
        }

        public async ValueTask UploadBlobAsync(string language, Stream content)
        {
            var blobName = string.Format(BlobName, language);

            var blobClient = this.blobContainerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(content, overwrite: true);
        }

        public async ValueTask<Stream> ReadBlobAsync(string language)
        {
            var blobName = string.Format(BlobName, language);
            var blobClient = this.blobContainerClient.GetBlobClient(blobName);

            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }
    }
}
