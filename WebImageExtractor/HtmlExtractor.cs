// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using HtmlAgilityPack;
using WebImageExtractor.Models;

namespace WebImageExtractor
{
    /// <summary>
    /// Extracts a list of tag types from a page.
    /// </summary>
    internal static class HtmlExtractor
    {
        public static IEnumerable<HtmlMetaTag> ExtractMetadata(HtmlDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var list = doc.DocumentNode.SelectNodes("//meta");
            if (list == null)
            {
                return null;
            }

            List<HtmlMetaTag> metadata = new List<HtmlMetaTag>();
            foreach (var node in list)
            {
                string name = node.GetAttributeValue("name", string.Empty);
                string content = node.GetAttributeValue("content", string.Empty);
                metadata.Add(new HtmlMetaTag(name, content));
            }

            return metadata;
        }

        public static IEnumerable<HtmlLinkTag> ExtractLinks(HtmlDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var list = doc.DocumentNode.SelectNodes("//link");
            if (list == null)
            {
                return null;
            }

            List<HtmlLinkTag> metadata = new List<HtmlLinkTag>();
            foreach (var node in list)
            {
                string rel = node.GetAttributeValue("rel", string.Empty);
                string href = node.GetAttributeValue("href", string.Empty);
                string type = node.GetAttributeValue("type", string.Empty);
                metadata.Add(new HtmlLinkTag(rel, href, type));
            }

            return metadata;
        }

        public static IEnumerable<HtmlImgTag> ExtractImages(HtmlDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var list = doc.DocumentNode.SelectNodes("//img");
            if (list == null)
            {
                return null;
            }

            List<HtmlImgTag> imgData = new List<HtmlImgTag>();
            foreach (var node in list)
            {
                string src = node.GetAttributeValue("src", string.Empty);
                imgData.Add(new HtmlImgTag(src));
            }

            return imgData;
        }

        public static IEnumerable<HtmlATag> ExtractATags(HtmlDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var list = doc.DocumentNode.SelectNodes("//a");
            if (list == null)
            {
                return null;
            }

            List<HtmlATag> aData = new List<HtmlATag>();
            foreach (var node in list)
            {
                string href = node.GetAttributeValue("href", string.Empty);
                aData.Add(new HtmlATag(href));
            }

            return aData;
        }
    }
}
