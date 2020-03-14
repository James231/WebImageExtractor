// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using ImageMagick;
using WebIconExtractor.Extensions;

namespace WebIconExtractor
{
    internal static class Downloader
    {
        /// <summary>
        /// Downloads a MagickImage from a Uri.
        /// </summary>
        /// <param name="uri">Uri to download from.</param>
        /// <returns>Downloaded MagickImage, null if unsuccessful.</returns>
        public static MagickImage DownloadMagickImage(Uri uri)
        {
            try
            {
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead(uri))
                {
                    MagickImage image = null;
                    try
                    {
                        image = new MagickImage(stream, uri.ToMagickFormat());
                    }
                    catch
                    {
                    }

                    stream.Close();
                    client.Dispose();
                    return image;
                }
            }
            catch (WebException)
            {
            }

            return null;
        }
    }
}
