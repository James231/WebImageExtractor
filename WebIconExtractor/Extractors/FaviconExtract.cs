// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using ImageMagick;
using WebIconExtractor.Extensions;
using WebIconExtractor.Structures;

namespace WebIconExtractor.Extractors
{
    internal static class FaviconExtract
    {
        public static MagickImage ExtractFavicon(Uri uri, ExtractOptions extractOptions)
        {
            if (extractOptions != ExtractOptions.SingleUrl)
            {
                return ExtractFaviconRecursive(uri);
            }

            return ExtractFavicon(uri);
        }

        private static MagickImage ExtractFavicon(Uri uri)
        {
            // Download default favicon
            Uri faviconUri = uri.AddRelativePath("favicon.ico");
            MagickImage faviconUriIcon = Downloader.DownloadMagickImage(faviconUri);
            if (faviconUriIcon != null)
            {
                return faviconUriIcon;
            }

            // Get icons through link and meta info
            HtmlMetaTag[] htmlMetaTags = HtmlExtractor.ExtractMetadata(uri);
            HtmlLinkTag[] htmlLinkTags = HtmlExtractor.ExtractLinks(uri);

            if (htmlLinkTags != null)
            {
                foreach (HtmlLinkTag link in htmlLinkTags)
                {
                    if (link.Rel.StartsWith("favicon"))
                    {
                        Uri absUri = uri.AddRelativePath(link.Link);
                        MagickImage linkIcon = Downloader.DownloadMagickImage(absUri);
                        if (linkIcon != null)
                        {
                            return linkIcon;
                        }
                    }
                }
            }

            if (htmlMetaTags != null)
            {
                foreach (HtmlMetaTag meta in htmlMetaTags)
                {
                    if (meta.Name.StartsWith("favicon"))
                    {
                        Uri absUri = uri.AddRelativePath(meta.Content);
                        MagickImage metaIcon = Downloader.DownloadMagickImage(absUri);
                        if (metaIcon != null)
                        {
                            return metaIcon;
                        }
                    }
                }
            }

            return null;
        }

        private static MagickImage ExtractFaviconRecursive(Uri uri)
        {
            Uri websiteUri = uri;
            bool reachedRoot = false;
            while (!reachedRoot)
            {
                if (websiteUri.AbsolutePath == "/")
                {
                    reachedRoot = true;
                }

                MagickImage newImg = ExtractFavicon(websiteUri);
                if (newImg != null)
                {
                    return newImg;
                }

                websiteUri = websiteUri.RemoveLastSegment();
            }

            return null;
        }
    }
}
