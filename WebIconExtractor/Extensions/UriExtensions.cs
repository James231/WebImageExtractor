// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;

namespace WebIconExtractor.Extensions
{
    internal static class UriExtensions
    {
        /// <summary>
        /// Adds relative path on to the end of a Uri.
        /// </summary>
        /// <param name="uri">The uri to add path to.</param>
        /// <param name="path">The relative path to add.</param>
        /// <returns>New Uri.</returns>
        public static Uri AddRelativePath(this Uri uri, string path)
        {
            // If the relative path is actually absolute, just return that instead
            if (path.StartsWith("http"))
            {
                return new Uri(path);
            }

            string[] paths = new string[] { path };
            return new Uri(paths.Aggregate(uri.ToString(), (current, p) => string.Format("{0}/{1}", current.TrimEnd('/'), p.TrimStart('/'))), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Removes the last segment of a Uri.
        /// </summary>
        /// <param name="uri">The Uri to remove the last segment of.</param>
        /// <returns>The new Uri with segment removed.</returns>
        public static Uri RemoveLastSegment(this Uri uri)
        {
            var noLastSegment = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
            for (int i = 0; i < uri.Segments.Length - 1; i++)
            {
                noLastSegment += uri.Segments[i];
            }

            return new Uri(noLastSegment.Trim("/".ToCharArray()));
        }

        /// <summary>
        /// Gets Image extention as <see cref="MagickFormat"/> from Uri.
        /// </summary>
        /// <param name="uri">Uri to get the format of.</param>
        /// <returns>Image extension as <see cref="MagickFormat"/>.</returns>
        public static MagickFormat ToMagickFormat(this Uri uri)
        {
            string uriString = uri.ToString().ToLower();

            KeyValuePair<string, MagickFormat>[] extensions = new KeyValuePair<string, MagickFormat>[]
            {
                new KeyValuePair<string, MagickFormat>(".png", MagickFormat.Png),
                new KeyValuePair<string, MagickFormat>(".ico", MagickFormat.Ico),
                new KeyValuePair<string, MagickFormat>(".icon", MagickFormat.Ico),
                new KeyValuePair<string, MagickFormat>(".bmp", MagickFormat.Bmp),
                new KeyValuePair<string, MagickFormat>(".data", MagickFormat.Data),
                new KeyValuePair<string, MagickFormat>(".gif", MagickFormat.Gif),
                new KeyValuePair<string, MagickFormat>(".jpeg", MagickFormat.Jpeg),
                new KeyValuePair<string, MagickFormat>(".jpg", MagickFormat.Jpg),
                new KeyValuePair<string, MagickFormat>(".raw", MagickFormat.Raw),
                new KeyValuePair<string, MagickFormat>(".svg", MagickFormat.Svg),
            };

            foreach (KeyValuePair<string, MagickFormat> extension in extensions)
            {
                if (uriString.EndsWith(extension.Key))
                {
                    return extension.Value;
                }
            }

            return MagickFormat.Png;
        }
    }
}
