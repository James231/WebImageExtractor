// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebImageExtractor.Extensions;
using WebImageExtractor.Models;

namespace WebImageExtractor
{
    internal static class Recurser
    {
        private static bool stopAlg = false;
        private static List<string> exploredUris;

        /// <summary>
        /// Delegate method which extracts images for a specific Uri.
        /// </summary>
        /// <param name="uri">Uri to extract images from.</param>
        /// <param name="doc">Parsed Html tree as instance of <see cref="HtmlDocument"/>.</param>
        /// <param name="settings">Extraction settings.</param>
        /// <returns>Returns Images extracted from Uri.</returns>
        public delegate Task<List<WebImage>> ExtractionMethod(Uri uri, HtmlDocument doc, ExtractionSettings settings);

        public static List<string> FoundUris { get; set; }

        /// <summary>
        /// Recurses through pages where images need to be extracted. Invokes a extraction method on each Uri found.
        /// </summary>
        /// <param name="method">Method which extracts images from speicific Uri.</param>
        /// <param name="uri">Uri to start recursion from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns Images extracted from explored Uris.</returns>
        public static async Task<IEnumerable<WebImage>> Recurse(ExtractionMethod method, string uri, ExtractionSettings settings, CancellationToken cancellationToken)
        {
            stopAlg = false;
            exploredUris = new List<string>();
            FoundUris = new List<string>();

            if (!settings.RecurseHyperlinks)
            {
                settings.HyperlinkRecursionDepth = 0;
            }
            else
            {
                if (settings.HyperlinkRecursionDepth < 0)
                {
                    settings.HyperlinkRecursionDepth = 0;
                }
            }

            // Extract images for start Uri and linked pages.
            Uri extractUri = new Uri(uri);
            List<WebImage> images = await HyperlinkRecurse(method, extractUri, settings, 0, cancellationToken);

            // If enabled, recurse through Uris by removing segments from the end
            if (settings.RecurseUri)
            {
                while (extractUri.AbsoluteUri != "/" && !stopAlg)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    extractUri = extractUri.RemoveLastSegment();
                    List<WebImage> moreImages = await HyperlinkRecurse(method, extractUri, settings, 0, cancellationToken);
                    images.AddRange(moreImages);
                }
            }

            if (!settings.LazyDownload)
            {
                // Images have already been downloaded, so remove any which failed
                return images.Where(i => i.GetImageIfDownloaded() != null);
            }

            return images;
        }

        /// <summary>
        /// Extracts images for a page and linked pages.
        /// </summary>
        /// <param name="method">Method which extracts images from speicific Uri.</param>
        /// <param name="uri">Uri to extract images and start hyperlink recursion from.</param>
        /// <param name="settings">Extraction Settings.</param>
        /// <param name="depth">Depth to recurse hyperlinks to.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns extracted images for given Uri and linked pages.</returns>
        public static async Task<List<WebImage>> HyperlinkRecurse(ExtractionMethod method, Uri uri, ExtractionSettings settings, int depth, CancellationToken cancellationToken)
        {
            HtmlDocument doc = null;
            bool gotDoc = false;
            List<WebImage> images = new List<WebImage>();

            if (!exploredUris.Contains(uri.ToString()))
            {
                exploredUris.Add(uri.ToString());

                if (settings.OnStartNewPage != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    await settings.OnStartNewPage.Invoke(uri.ToString());
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                gotDoc = true;
                doc = await GetDocumnent(uri, cancellationToken);
                images = await method.Invoke(uri, doc, settings);

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (settings.OnEndNewPage != null)
                {
                    await settings.OnEndNewPage.Invoke(uri.ToString(), images);
                }

                if (settings.ShouldStopOnFoundImage != null)
                {
                    // Take all images up to the point where should stop
                    int index = images.TakeWhile(i => !settings.ShouldStopOnFoundImage.Invoke(i)).Count();
                    if (index != images.Count)
                    {
                        images.RemoveRange(index + 1, images.Count - index - 1);
                        stopAlg = true;
                    }
                }

                if (!settings.LazyDownload)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    await Task.WhenAll(images.Select(i => i.GetImagesAsync(cancellationToken)).ToArray());
                }

                if (settings.OnFoundImage != null)
                {
                    images.ForEach(i => settings.OnFoundImage.Invoke(i));
                }
            }

            if (!stopAlg && settings.RecurseHyperlinks && depth < settings.HyperlinkRecursionDepth)
            {
                if (!gotDoc)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    doc = await GetDocumnent(uri, cancellationToken);
                }

                if (doc != null)
                {
                    IEnumerable<HtmlATag> aTags = HtmlExtractor.ExtractATags(doc);
                    foreach (HtmlATag aTag in aTags)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return null;
                        }

                        Uri newUri = uri.AddHtmlLink(aTag.Href);
                        List<WebImage> moreImages = await HyperlinkRecurse(method, newUri, settings, depth + 1, cancellationToken);
                        images.AddRange(moreImages);
                    }
                }
            }

            return images;
        }

        /// <summary>
        /// Gets parsed Html as a <see cref="HtmlDocument"/> from a Uri.
        /// </summary>
        /// <param name="uri">Uri to get parsed Html from.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns parsed Html as an instance of <see cref="HtmlDocument"/>.</returns>
        private static async Task<HtmlDocument> GetDocumnent(Uri uri, CancellationToken cancellationToken)
        {
            if (Extractor.ExtractionSettings == null)
            {
                Extractor.ExtractionSettings = new ExtractionSettings();
            }

            if (Extractor.ExtractionSettings.HttpClient == null)
            {
                Extractor.ExtractionSettings.HttpClient = new HttpClient();
            }

            // TODO: try{}catch{} can be removed when the following issue has been fixed of the HtmlAgilityPack
            // https://github.com/zzzprojects/html-agility-pack/pull/327
            // The issue causes exception to be thrown for some urls like "https://facebook.com"
            try
            {
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

                    using (HttpContent content = response.Content)
                    {
                        string result = await content.ReadAsStringAsync();
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(result);
                        return doc;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
