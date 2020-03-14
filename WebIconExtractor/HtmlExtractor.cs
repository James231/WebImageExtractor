// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using WebIconExtractor.Structures;

namespace WebIconExtractor
{
    internal static class HtmlExtractor
    {
        public static HtmlMetaTag[] ExtractMetadata(Uri uri)
        {
            // TODO: try{}catch{} can be removed when the following issue has been fixed of the HtmlAgilityPack
            // https://github.com/zzzprojects/html-agility-pack/pull/327
            // The issue causes exception to be thrown for some urls like "https://facebook.com"
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(uri.ToString());
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

                return metadata.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static HtmlLinkTag[] ExtractLinks(Uri uri)
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(uri);
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
                    metadata.Add(new HtmlLinkTag(rel, href));
                }

                return metadata.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }
}
