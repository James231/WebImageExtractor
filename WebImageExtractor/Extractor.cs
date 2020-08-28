// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebImageExtractor
{
    public static class Extractor
    {
        internal static ExtractionSettings ExtractionSettings { get; set; }

        /// <summary>
        /// Extracts all Favicons.
        /// </summary>
        /// <param name="uri">Uri to start extracting from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Extracted Favicons as instances of <see cref="WebImage"/>.</returns>
        public static async Task<IEnumerable<WebImage>> GetFavicons(string uri, ExtractionSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Extract(uri, settings, cancellationToken, HtmlUtilities.GetFavicons);
        }

        /// <summary>
        /// Extracts all Apple Touch Icons.
        /// </summary>
        /// <param name="uri">Uri to start extracting from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Extracted Apple Touch Icons as instances of <see cref="WebImage"/>.</returns>
        public static async Task<IEnumerable<WebImage>> GetAppleTouchIcons(string uri, ExtractionSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Extract(uri, settings, cancellationToken, HtmlUtilities.GetAppleTouchIcons);
        }

        /// <summary>
        /// Extracts all Favicons and Apple Touch Icons from a page.
        /// </summary>
        /// <param name="uri">Uri to start extracting from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Extracted Favicons and Apple Touch Icons as instances of <see cref="WebImage"/>.</returns>
        public static async Task<IEnumerable<WebImage>> GetAllIcons(string uri, ExtractionSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Extract(uri, settings, cancellationToken, async (Uri u, HtmlDocument d, ExtractionSettings s) =>
            {
                List<WebImage> favicons = await HtmlUtilities.GetFavicons(u, d, s);
                List<WebImage> appleTouchIcons = await HtmlUtilities.GetAppleTouchIcons(u, d, s);
                favicons.AddRange(appleTouchIcons);
                return favicons;
            });
        }

        /// <summary>
        /// Extracts all images from a page (excluding Favicons and Apple Touch Icons).
        /// </summary>
        /// <param name="uri">Uri to start extracting from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Extracted images as instances of <see cref="WebImage"/>.</returns>
        public static async Task<IEnumerable<WebImage>> GetPageImages(string uri, ExtractionSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Extract(uri, settings, cancellationToken, HtmlUtilities.GetPageImages);
        }

        /// <summary>
        /// Extracts all images from a page (including Favicons and Apple Touch Icons).
        /// </summary>
        /// <param name="uri">Uri to start extracting from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Extracted images as instances of <see cref="WebImage"/>.</returns>
        public static async Task<IEnumerable<WebImage>> GetAllImages(string uri, ExtractionSettings settings = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Extract(uri, settings, cancellationToken, async (Uri u, HtmlDocument d, ExtractionSettings s) =>
            {
                List<WebImage> favicons = await HtmlUtilities.GetFavicons(u, d, s);
                List<WebImage> appleTouchIcons = await HtmlUtilities.GetAppleTouchIcons(u, d, s);
                List<WebImage> images = await HtmlUtilities.GetPageImages(u, d, s);
                favicons.AddRange(appleTouchIcons);
                favicons.AddRange(images);
                return favicons;
            });
        }

        private static async Task<IEnumerable<WebImage>> Extract(string uri, ExtractionSettings settings, CancellationToken cancellationToken, Recurser.ExtractionMethod extractionMethod)
        {
            if (settings == null)
            {
                settings = new ExtractionSettings();
            }

            ExtractionSettings = settings;

            return await Recurser.Recurse(extractionMethod, uri, settings, cancellationToken);
        }
    }
}