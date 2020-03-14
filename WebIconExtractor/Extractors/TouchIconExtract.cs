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
    internal static class TouchIconExtract
    {
        public static MagickImage ExtractTouchIcon(Uri uri, ExtractOptions extractOptions = ExtractOptions.Recursive)
        {
            if (extractOptions != ExtractOptions.SingleUrl)
            {
                return ExtractTouchIconRecursive(uri);
            }

            return ExtractTouchIcon(uri);
        }

        private static MagickImage ExtractTouchIcon(Uri uri)
        {
            // Download default favicon
            Uri touchIconUri = uri.AddRelativePath("apple-touch-icon.png");
            MagickImage touchIconUriIcon = Downloader.DownloadMagickImage(touchIconUri);
            if (touchIconUriIcon != null)
            {
                return touchIconUriIcon;
            }

            // Get icons through link and meta info
            HtmlMetaTag[] htmlMetaTags = HtmlExtractor.ExtractMetadata(uri);
            HtmlLinkTag[] htmlLinkTags = HtmlExtractor.ExtractLinks(uri);

            if (htmlLinkTags != null)
            {
                foreach (HtmlLinkTag link in htmlLinkTags)
                {
                    if (link.Rel.StartsWith("apple-touch-icon"))
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
                    if (meta.Name.StartsWith("apple-touch-icon"))
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

        private static MagickImage ExtractTouchIconRecursive(Uri uri)
        {
            Uri websiteUri = uri;
            bool reachedRoot = false;
            while (!reachedRoot)
            {
                if (websiteUri.AbsolutePath == "/")
                {
                    reachedRoot = true;
                }

                MagickImage newImg = ExtractTouchIcon(websiteUri);
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
