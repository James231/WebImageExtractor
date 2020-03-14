// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace WebIconExtractor.Structures
{
    internal class HtmlLinkTag
    {
        public HtmlLinkTag(string rel, string link)
        {
            Rel = rel;
            Link = link;
        }

        public string Rel { get; set; }

        public string Link { get; set; }
    }
}
