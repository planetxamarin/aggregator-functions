// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace PlanetDotnet.Services.Processings.Feeds
{
    public interface IFeedProcessingService
    {
        ValueTask ProcessFeedLoadingAsync();
    }
}
