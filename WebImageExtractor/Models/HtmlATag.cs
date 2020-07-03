// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace WebImageExtractor.Models
{
    /// <summary>
    /// Class representing an <a> tag in HTML.
    /// </summary>
    internal class HtmlATag
    {
        public HtmlATag(string href)
        {
            Href = href;
        }

        public string Href { get; set; }
    }
}
