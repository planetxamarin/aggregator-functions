// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using PlanetDotnet.Models.Previews;

namespace PlanetDotnet.Services.Processings.Previews
{
    public interface IPreviewProcessingService
    {
        ValueTask<IEnumerable<Preview>> RetrieveAllPreviewsAsync(string language);
    }
}
