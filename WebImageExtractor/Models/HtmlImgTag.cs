// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace WebImageExtractor.Models
{
    /// <summary>
    /// Class representing an <img> tag in HTML.
    /// </summary>
    internal class HtmlImgTag
    {
        public HtmlImgTag(string src)
        {
            Src = src;
        }

        public string Src { get; set; }
    }
}
