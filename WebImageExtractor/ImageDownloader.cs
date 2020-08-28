// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using WebImageExtractor.Extensions;

namespace WebImageExtractor
{
    internal static class ImageDownloader
    {
        /// <summary>
        /// Downloads a MagickImage from a Uri.
        /// </summary>
        /// <param name="uri">Uri to download from.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Downloaded MagickImage, null if unsuccessful.</returns>
        public static async Task<MagickImage> DownloadMagickImage(Uri uri, CancellationToken cancellationToken)
        {
            if (Extractor.ExtractionSettings == null)
            {
                Extractor.ExtractionSettings = new ExtractionSettings();
            }

            if (Extractor.ExtractionSettings.HttpClient == null)
            {
                Extractor.ExtractionSettings.HttpClient = new HttpClient();
            }

            HttpClient client = Extractor.ExtractionSettings.HttpClient;
            string uriString = string.Empty;
            if (Extractor.ExtractionSettings.UseCorsAnywhere)
            {
                uriString = Constants.CorsAnywhereUri + uri.ToStringWithoutProtocol();
            }
            else
            {
                uriString = uri.ToString();
            }

            using (HttpResponseMessage response = await client.GetAsync(uriString, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    MagickImage image = null;
                    image = new MagickImage(stream, uri.ToMagickFormat());
                    return image;
                }
            }
        }

        /// <summary>
        /// Checks a Uri returns a 2xx OK with a GET request, without downloading from the stream.
        /// </summary>
        /// <param name="uri">Uri to check.</param>
        /// <returns>True if 2xx was returned.</returns>
        public static async Task<bool> IsOkUri(Uri uri)
        {
            if (Extractor.ExtractionSettings.DisableValidityCheck)
            {
                return true;
            }

            if (Extractor.ExtractionSettings == null)
            {
                Extractor.ExtractionSettings = new ExtractionSettings();
            }

            if (Extractor.ExtractionSettings.HttpClient == null)
            {
                Extractor.ExtractionSettings.HttpClient = new HttpClient();
            }

            HttpClient client = Extractor.ExtractionSettings.HttpClient;
            string uriString = string.Empty;
            if (Extractor.ExtractionSettings.UseCorsAnywhere)
            {
                uriString = Constants.CorsAnywhereUri + uri.ToStringWithoutProtocol();
            }
            else
            {
                uriString = uri.ToString();
            }

            using (HttpResponseMessage response = await client.GetAsync(uriString))
            {
                return IsSuccessStatusCode(response.StatusCode);
            }
        }

        /// <summary>
        /// Checks a status code is a success code in form 2xx.
        /// </summary>
        /// <param name="statusCode">Status code to check.</param>
        /// <returns>True if the status code is a success code.</returns>
        private static bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }
    }
}
