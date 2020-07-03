// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using WebImageExtractor.Models;

namespace WebImageExtractor.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="HtmlLinkTag"/>.
    /// </summary>
    internal static class HtmlLinkExtensions
    {
        /// <summary>
        /// Is this link tag for a favicon?
        /// </summary>
        /// <param name="link">Link tag.</param>
        /// <returns>Returns true if the link is for a favicon.</returns>
        internal static bool IsFavicon(this HtmlLinkTag link)
        {
            if (link.Rel == "shortcut icon")
            {
                return true;
            }

            if (link.Rel == "icon")
            {
                return true;
            }

            if (link.Rel == "mask-icon")
            {
                return true;
            }

            if (link.Rel.Contains("favicon"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Is this link tag for an Apple touch icon?
        /// </summary>
        /// <param name="link">Link tag.</param>
        /// <returns>Returns true if the link is for an Apple touch icon.</returns>
        internal static bool IsAppleTouchIcon(this HtmlLinkTag link)
        {
            if (link.Rel.Contains("apple-touch-icon"))
            {
                return true;
            }

            return false;
        }
    }
}
