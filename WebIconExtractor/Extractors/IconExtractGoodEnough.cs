// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using ImageMagick;
using WebIconExtractor.Extensions;
using WebIconExtractor.Structures;
using static WebIconExtractor.IconExtractor;

namespace WebIconExtractor.Extractors
{
    internal static class IconExtractGoodEnough
    {
        public static MagickImage ExtractGoodEnough(Uri uri, IconValid check, ExtractOptions extractOptions)
        {
            if (extractOptions != ExtractOptions.SingleUrl)
            {
                return ExtractGoodEnoughRecursive(uri, check);
            }

            return ExtractGoodEnough(uri, check);
        }

        private static MagickImage ExtractGoodEnough(Uri uri, IconValid check)
        {
            // Download default favicon
            Uri faviconUri = uri.AddRelativePath("favicon.ico");
            MagickImage faviconUriIcon = Downloader.DownloadMagickImage(faviconUri);
            if (faviconUriIcon != null)
            {
                if (check.Invoke(faviconUriIcon))
                {
                    return faviconUriIcon;
                }
            }

            Uri touchIconUri = uri.AddRelativePath("apple-touch-icon.png");
            MagickImage touchIconUriIcon = Downloader.DownloadMagickImage(touchIconUri);
            if (touchIconUriIcon != null)
            {
                if (check.Invoke(touchIconUriIcon))
                {
                    return touchIconUriIcon;
                }
            }

            // Get icons through link and meta info
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
                            if (check.Invoke(linkIcon))
                            {
                                return linkIcon;
                            }
                        }
                    }
                }
            }

            HtmlMetaTag[] htmlMetaTags = HtmlExtractor.ExtractMetadata(uri);

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
                            if (check.Invoke(metaIcon))
                            {
                                return metaIcon;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static MagickImage ExtractGoodEnoughRecursive(Uri uri, IconValid check)
        {
            Uri websiteUri = uri;
            bool reachedRoot = false;
            while (!reachedRoot)
            {
                if (websiteUri.AbsolutePath == "/")
                {
                    reachedRoot = true;
                }

                MagickImage newImg = ExtractGoodEnough(websiteUri, check);
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
