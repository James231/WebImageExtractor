// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ImageMagick;
using WebIconExtractor.Extensions;
using WebIconExtractor.Structures;

namespace WebIconExtractor
{
    internal static class IconExtractAll
    {
        /// <summary>
        /// Extracts all icons from a given Uri.
        /// </summary>
        /// <param name="uri">The Uri to extract icons from.</param>
        /// <param name="options">Extraction Options.</param>
        /// <returns>Array of all icons extracted from the Uri.</returns>
        public static MagickImage[] ExtractAll(Uri uri, ExtractOptions options)
        {
            if (options == ExtractOptions.Recursive)
            {
                List<MagickImage> arr = ExtractAllRecursive(uri, options == ExtractOptions.RecurseUntilSuccess);
                if (arr == null)
                {
                    return null;
                }

                return arr.ToArray();
            }
            else
            {
                List<MagickImage> arr = ExtractAll(uri);
                if (arr == null)
                {
                    return null;
                }

                return arr.ToArray();
            }
        }

        private static List<MagickImage> ExtractAllRecursive(Uri uri, bool untilSuccess)
        {
            List<MagickImage> arr = new List<MagickImage>();
            Uri websiteUri = uri;
            bool reachedRoot = false;
            while (!reachedRoot && (!untilSuccess || (untilSuccess && arr.Count == 0)))
            {
                if (websiteUri.AbsolutePath == "/")
                {
                    reachedRoot = true;
                }

                List<MagickImage> newArr = ExtractAll(websiteUri);
                if (newArr != null)
                {
                    arr.AddRange(newArr);
                }

                websiteUri = websiteUri.RemoveLastSegment();
            }

            return arr;
        }

        private static List<MagickImage> ExtractAll(Uri uri)
        {
            List<MagickImage> arr = new List<MagickImage>();

            // Download default favicon and touch icon
            Uri faviconUri = uri.AddRelativePath("favicon.ico");
            MagickImage faviconUriIcon = Downloader.DownloadMagickImage(faviconUri);
            if (faviconUriIcon != null)
            {
                arr.Add(faviconUriIcon);
            }

            Uri touchIconUri = uri.AddRelativePath("apple-touch-icon.png");
            MagickImage touchIconUriIcon = Downloader.DownloadMagickImage(touchIconUri);
            if (touchIconUriIcon != null)
            {
                arr.Add(touchIconUriIcon);
            }

            // Get icons through link and meta info
            HtmlMetaTag[] htmlMetaTags = HtmlExtractor.ExtractMetadata(uri);
            HtmlLinkTag[] htmlLinkTags = HtmlExtractor.ExtractLinks(uri);

            if (htmlLinkTags != null)
            {
                foreach (HtmlLinkTag link in htmlLinkTags)
                {
                    if (link.Rel.StartsWith("apple-touch-icon") || link.Rel.StartsWith("favicon") || link.Rel.StartsWith("icon"))
                    {
                        Uri absUri = uri.AddRelativePath(link.Link);
                        MagickImage linkIcon = Downloader.DownloadMagickImage(absUri);
                        if (linkIcon != null)
                        {
                            arr.Add(linkIcon);
                        }
                    }
                }
            }

            if (htmlMetaTags != null)
            {
                foreach (HtmlMetaTag meta in htmlMetaTags)
                {
                    if (meta.Name.StartsWith("apple-touch-icon") || meta.Name.StartsWith("favicon") || meta.Name.StartsWith("icon"))
                    {
                        Uri absUri = uri.AddRelativePath(meta.Content);
                        MagickImage metaIcon = Downloader.DownloadMagickImage(absUri);
                        if (metaIcon != null)
                        {
                            arr.Add(metaIcon);
                        }
                    }
                }
            }

            if (arr.Count == 0)
            {
                return null;
            }

            return arr;
        }
    }
}
