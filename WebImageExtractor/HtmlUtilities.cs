// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebImageExtractor.Extensions;
using WebImageExtractor.Models;

namespace WebImageExtractor
{
    /// <summary>
    /// Algorithms for extracting icons and images from a specific Uri.
    /// </summary>
    internal static class HtmlUtilities
    {
        public delegate Task TraversalStep(HtmlNode node);

        /// <summary>
        /// Returns images for a Uri (excluding Favicons and Apple Touch Icons).
        /// </summary>
        /// <param name="uri">Uri to explore.</param>
        /// <param name="doc">Instance of <see cref="HtmlDocument"/> containing parsed Html for the Uri.</param>
        /// <param name="settings">Extraction settings.</param>
        /// <returns>List of images as instances of <see cref="WebImage"/>.</returns>
        public static async Task<List<WebImage>> GetPageImages(Uri uri, HtmlDocument doc, ExtractionSettings settings)
        {
            List<WebImage> arr = new List<WebImage>();

            if (doc == null)
            {
                return arr;
            }

            if (settings.GetLinkTagImages)
            {
                IEnumerable<HtmlLinkTag> htmlLinkTags = HtmlExtractor.ExtractLinks(doc);
                if (htmlLinkTags != null)
                {
                    foreach (HtmlLinkTag link in htmlLinkTags)
                    {
                        if (!link.IsFavicon() && !link.IsAppleTouchIcon())
                        {
                            Uri absUri = uri.AddHtmlLink(link.Link);
                            if (settings.SvgOnly && !absUri.HasSvgExtension())
                            {
                                continue;
                            }

                            if (!settings.SvgOnly)
                            {
                                if (!absUri.HasImageExtension())
                                {
                                    continue;
                                }
                            }

                            if (Recurser.FoundUris.Contains(absUri.ToString()))
                            {
                                continue;
                            }

                            if (await ImageDownloader.IsOkUri(absUri))
                            {
                                Recurser.FoundUris.Add(absUri.ToString());
                                arr.Add(new WebImage(absUri.ToString(), false, false));
                            }
                        }
                    }
                }
            }

            if (settings.GetMetaTagImages)
            {
                IEnumerable<HtmlMetaTag> htmlMetaTags = HtmlExtractor.ExtractMetadata(doc);
                if (htmlMetaTags != null)
                {
                    foreach (HtmlMetaTag meta in htmlMetaTags)
                    {
                        Uri absUri = uri.AddHtmlLink(meta.Content);
                        if (settings.SvgOnly && !absUri.HasSvgExtension())
                        {
                            continue;
                        }

                        if (!settings.SvgOnly)
                        {
                            if (!absUri.HasImageExtension())
                            {
                                continue;
                            }
                        }

                        if (Recurser.FoundUris.Contains(absUri.ToString()))
                        {
                            continue;
                        }

                        if (await ImageDownloader.IsOkUri(absUri))
                        {
                            Recurser.FoundUris.Add(absUri.ToString());
                            arr.Add(new WebImage(absUri.ToString(), false, false));
                        }
                    }
                }
            }

            if (settings.GetInlineBackgroundImages)
            {
                // Inline background images
                HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//body");
                if (bodyNode != null)
                {
                    await TraverseNode(bodyNode, async (HtmlNode node) =>
                    {
                        string styleValue = node.GetAttributeValue("style", null);
                        if (!string.IsNullOrEmpty(styleValue))
                        {
                            MatchCollection matches = Regex.Matches(styleValue, @"(?<=url\()(.*)(?=\))");
                            foreach (Match match in matches)
                            {
                                string value = match.Value;
                                if (!string.IsNullOrEmpty(value))
                                {
                                    if ((value[0] == '\'' && value[value.Length - 1] == '\'') || (value[0] == '\"' && value[value.Length - 1] == '\"'))
                                    {
                                        value = value.Substring(1, value.Length - 2);
                                    }

                                    Uri link = uri.AddHtmlLink(value);
                                    if (settings.SvgOnly && !link.HasSvgExtension())
                                    {
                                        continue;
                                    }

                                    if (!settings.SvgOnly)
                                    {
                                        if (!link.HasImageExtension())
                                        {
                                            continue;
                                        }
                                    }

                                    if (Recurser.FoundUris.Contains(link.ToString()))
                                    {
                                        continue;
                                    }

                                    if (await ImageDownloader.IsOkUri(link))
                                    {
                                        Recurser.FoundUris.Add(link.ToString());
                                        arr.Add(new WebImage(link.ToString(), false, false, true));
                                    }
                                }
                            }
                        }
                    });
                }

                // TODO: Add Option to get background images from CSS files
            }

            IEnumerable<HtmlImgTag> imgTags = HtmlExtractor.ExtractImages(doc);
            if (imgTags != null)
            {
                foreach (HtmlImgTag img in imgTags)
                {
                    Uri absUri = uri.AddHtmlLink(img.Src);
                    if (settings.SvgOnly && !absUri.HasSvgExtension())
                    {
                        continue;
                    }

                    if (absUri.IsBadMagickType())
                    {
                        continue;
                    }

                    if (Recurser.FoundUris.Contains(absUri.ToString()))
                    {
                        continue;
                    }

                    if (await ImageDownloader.IsOkUri(absUri))
                    {
                        Recurser.FoundUris.Add(absUri.ToString());
                        arr.Add(new WebImage(absUri.ToString(), false, false));
                    }
                }
            }

            return arr;
        }

        /// <summary>
        /// Returns Favicons for a Uri.
        /// </summary>
        /// <param name="uri">Uri to explore.</param>
        /// <param name="doc">Instance of <see cref="HtmlDocument"/> containing parsed Html for the Uri.</param>
        /// <param name="settings">Extraction settings.</param>
        /// <returns>List of Favicons as instances of <see cref="WebImage"/>.</returns>
        public static async Task<List<WebImage>> GetFavicons(Uri uri, HtmlDocument doc, ExtractionSettings settings)
        {
            List<WebImage> arr = new List<WebImage>();

            // Check default favicon location
            if (!settings.SvgOnly)
            {
                Uri faviconUri = uri.AddHtmlLink(Constants.DefaultFaviconPath);
                if (await ImageDownloader.IsOkUri(faviconUri))
                {
                    if (!Recurser.FoundUris.Contains(faviconUri.ToString()))
                    {
                        Recurser.FoundUris.Add(faviconUri.ToString());
                        arr.Add(new WebImage(faviconUri.ToString(), true, false));
                    }
                }
            }

            // Parse HTML Tree
            if (doc == null)
            {
                return arr;
            }

            // Get icons through link info
            IEnumerable<HtmlLinkTag> htmlLinkTags = HtmlExtractor.ExtractLinks(doc);

            if (htmlLinkTags != null)
            {
                foreach (HtmlLinkTag link in htmlLinkTags)
                {
                    if (link.IsFavicon())
                    {
                        Uri absUri = uri.AddHtmlLink(link.Link);
                        if (settings.SvgOnly && !absUri.HasSvgExtension())
                        {
                            continue;
                        }

                        if (!settings.SvgOnly)
                        {
                            if (!absUri.HasImageExtension())
                            {
                                continue;
                            }
                        }

                        if (Recurser.FoundUris.Contains(absUri.ToString()))
                        {
                            continue;
                        }

                        if (await ImageDownloader.IsOkUri(absUri))
                        {
                            Recurser.FoundUris.Add(absUri.ToString());
                            arr.Add(new WebImage(absUri.ToString(), true, false));
                        }
                    }
                }
            }

            return arr;
        }

        /// <summary>
        /// Returns Apple Touch Icons for a Uri.
        /// </summary>
        /// <param name="uri">Uri to explore.</param>
        /// <param name="doc">Instance of <see cref="HtmlDocument"/> containing parsed Html for the Uri.</param>
        /// <param name="settings">Extraction settings.</param>
        /// <returns>List of Apple Touch Icons as instances of <see cref="WebImage"/>.</returns>
        public static async Task<List<WebImage>> GetAppleTouchIcons(Uri uri, HtmlDocument doc, ExtractionSettings settings)
        {
            List<WebImage> arr = new List<WebImage>();

            // Check default apple touch icon location
            if (!settings.SvgOnly)
            {
                Uri atiUri = uri.AddHtmlLink(Constants.DefaultAppleTouchIconPath);
                if (await ImageDownloader.IsOkUri(atiUri))
                {
                    if (!Recurser.FoundUris.Contains(atiUri.ToString()))
                    {
                        Recurser.FoundUris.Add(atiUri.ToString());
                        arr.Add(new WebImage(atiUri.ToString(), false, true));
                    }
                }
            }

            // Parse HTML Tree
            if (doc == null)
            {
                return arr;
            }

            // Get icons through link info
            IEnumerable<HtmlLinkTag> htmlLinkTags = HtmlExtractor.ExtractLinks(doc);

            if (htmlLinkTags != null)
            {
                foreach (HtmlLinkTag link in htmlLinkTags)
                {
                    if (link.IsAppleTouchIcon())
                    {
                        Uri absUri = uri.AddHtmlLink(link.Link);
                        if (settings.SvgOnly && !absUri.HasSvgExtension())
                        {
                            continue;
                        }

                        if (!settings.SvgOnly)
                        {
                            if (!absUri.HasImageExtension())
                            {
                                continue;
                            }
                        }

                        if (Recurser.FoundUris.Contains(absUri.ToString()))
                        {
                            continue;
                        }

                        if (await ImageDownloader.IsOkUri(absUri))
                        {
                            Recurser.FoundUris.Add(absUri.ToString());
                            arr.Add(new WebImage(absUri.ToString(), false, true));
                        }
                    }
                }
            }

            return arr;
        }

        /// <summary>
        /// Recursively traverses all instances of <see cref="HtmlNode"/> from a given root in the tree, applying a callback method for each.
        /// </summary>
        /// <param name="node">Root node to start from.</param>
        /// <param name="method">Method to call for each node.</param>
        /// <returns>Empty Task.</returns>
        private static async Task TraverseNode(HtmlNode node, TraversalStep method)
        {
            if (node != null)
            {
                await method.Invoke(node);
            }

            if (node.HasChildNodes)
            {
                foreach (HtmlNode n in node.ChildNodes)
                {
                    await TraverseNode(n, method);
                }
            }
        }
    }
}
