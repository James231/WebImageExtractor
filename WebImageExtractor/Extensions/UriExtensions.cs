// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using ImageMagick;

namespace WebImageExtractor.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="System.Uri"/>.
    /// </summary>
    internal static class UriExtensions
    {
        /// <summary>
        /// Adds html formatted link to uri.
        /// See https://www.w3schools.com/html/html_filepaths.asp.
        /// </summary>
        /// <param name="currentPage">Current Page Uri.</param>
        /// <param name="link">Link to add.</param>
        /// <returns>Link to resource.</returns>
        public static Uri AddHtmlLink(this Uri currentPage, string link)
        {
            // Deal with absolute links
            if (link.StartsWith("http"))
            {
                return new Uri(link);
            }

            // Links on the base domain
            if (link.StartsWith("/"))
            {
                return new Uri(currentPage, link);
            }

            // Ignore section links
            if (link.StartsWith("#"))
            {
                return currentPage;
            }

            // Links on current folder
            Uri currentFolder = RemoveLastSegment(currentPage);
            string[] paths = new string[] { link };
            return new Uri(paths.Aggregate(currentFolder.ToString(), (current, p) => string.Format("{0}/{1}", current.TrimEnd('/'), p.TrimStart('/'))), UriKind.RelativeOrAbsolute);
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
        /// Gets Image extension as <see cref="MagickFormat"/> from Uri.
        /// </summary>
        /// <param name="uri">Uri to get the format of.</param>
        /// <returns>Image extension as <see cref="MagickFormat"/>.</returns>
        public static MagickFormat ToMagickFormat(this Uri uri)
        {
            string uriString = uri.ToString().ToLower();

            Array values = Enum.GetValues(typeof(MagickFormat));
            foreach (MagickFormat value in values)
            {
                if (uriString.EndsWith($".{Enum.GetName(typeof(MagickFormat), value).ToLower()}"))
                {
                    return value;
                }
            }

            return MagickFormat.Png;
        }

        /// <summary>
        /// Does the Uri have an image extension supported by Magick.NET?
        /// </summary>
        /// <param name="uri">Uri to check extension of.</param>
        /// <returns>True if image extension supported by Magick.NET.</returns>
        public static bool HasImageExtension(this Uri uri)
        {
            string uriString = uri.ToString().ToLower();
            string[] magickFormats = Enum.GetNames(typeof(MagickFormat));
            return magickFormats.Any(f => !Constants.BadMagickTypes.Contains(f) && uriString.EndsWith($".{f.ToLower()}"));
        }

        /// <summary>
        /// Does the Uri have an image extension which is supported by Magick.NET but not by WebImageExtractor
        /// </summary>
        /// <param name="uri">Uri to check extension of.</param>
        /// <returns>True if image extension is not supported.</returns>
        public static bool IsBadMagickType(this Uri uri)
        {
            string uriString = uri.ToString().ToLower();
            return Constants.BadMagickTypes.Any(ts => uriString.EndsWith($".{ts}"));
        }

        /// <summary>
        /// Does the Uri have an svg image extension?
        /// </summary>
        /// <param name="uri">Uri to check extension of.</param>
        /// <returns>True if image extension is .svg</returns>
        public static bool HasSvgExtension(this Uri uri)
        {
            string uriString = uri.ToString().ToLower();
            return uriString.EndsWith($".svg");
        }

        /// <summary>
        /// To string, with the protocol removed.
        /// </summary>
        /// <param name="uri">Uri to remove protocol of.</param>
        /// <returns>Returns Uri as a string with the protocol removed.</returns>
        public static string ToStringWithoutProtocol(this Uri uri)
        {
            string uriString = uri.ToString();
            int i = uriString.IndexOf(':');
            if (i > 0)
            {
                uriString = uriString.Substring(i + 1);
            }

            while (uriString[0] == '/')
            {
                uriString = uriString.Substring(1);
            }

            return uriString;
        }
    }
}
