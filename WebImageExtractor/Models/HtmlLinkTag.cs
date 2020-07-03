// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace WebImageExtractor.Models
{
    /// <summary>
    /// Class representing an <link> tag in HTML.
    /// </summary>
    internal class HtmlLinkTag
    {
        public HtmlLinkTag(string rel, string link, string type)
        {
            Rel = rel;
            Link = link;
            Type = type;
        }

        public string Rel { get; set; }

        public string Link { get; set; }

        public string Type { get; set; }
    }
}
