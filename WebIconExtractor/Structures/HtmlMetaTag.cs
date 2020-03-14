// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace WebIconExtractor.Structures
{
    internal class HtmlMetaTag
    {
        public HtmlMetaTag(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}
