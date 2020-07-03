// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebImageExtractor
{
    /// <summary>
    /// Settings for Image Extraction.
    /// </summary>
    public class ExtractionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractionSettings"/> class.
        /// </summary>
        /// <param name="lazyDownload">If false, all images be downloaded to an instance of <see cref="ImageMagick.MagickImage"/> immediatley after extracted.</param>
        /// <param name="svgOnly">Should only Svgs be extracted?</param>
        /// <param name="recurseUri">Should images be extracted from pages with segments removed from given Uri?</param>
        /// <param name="recurseHyperlinks">Should images be extracted from pages linked from the given Uri?</param>
        /// <param name="hyperlinkRecursionDepth">If recursing hyperlinks, to what depth should hyperlinks be explored for image extraction.</param>
        public ExtractionSettings(bool lazyDownload = false, bool svgOnly = false, bool recurseUri = false, bool recurseHyperlinks = false, int hyperlinkRecursionDepth = 1)
        {
            LazyDownload = lazyDownload;
            SvgOnly = svgOnly;
            RecurseUri = RecurseUri;
            RecurseHyperlinks = recurseHyperlinks;
            HyperlinkRecursionDepth = hyperlinkRecursionDepth;
        }

        public delegate Task ImageFoundEvent(WebImage image);

        public delegate Task PageStartEvent(string uri);

        public delegate Task PageEndEvent(string uri, IEnumerable<WebImage> image);

        public delegate bool ShouldStopCheck(WebImage image);

        /// <summary>
        /// Only extract Svgs?
        /// </summary>
        public bool SvgOnly { get; set; } = false;

        /// <summary>
        /// Recurse Uri segments and extract images from all?
        /// </summary>
        /// <example>If true and extracting from uri 'http://example.com/mydir/mypage.html' then images will also be extracted from 'http://example.com/mydir' and 'http://example.com'.</example>
        public bool RecurseUri { get; set; } = false;

        /// <summary>
        /// Extract images from any pages linked to by the given Uri?
        /// </summary>
        public bool RecurseHyperlinks { get; set; } = false;

        /// <summary>
        /// Number of layers of hyperlinks to explore for image extraction.
        /// </summary>
        public int HyperlinkRecursionDepth { get; set; } = 1;

        /// <summary>
        /// Download images immediatley after extraction? Or download them when required?
        /// </summary>
        public bool LazyDownload { get; set; } = false;

        /// <summary>
        /// When extracting all images on a page, should the extractor get images from meta tags in the html <head>
        /// </summary>
        public bool GetMetaTagImages { get; set; } = true;

        /// <summary>
        /// When extracting all images on a page, should the extractor get images from link tags in addition to favicons and apple touch icons
        /// </summary>
        public bool GetLinkTagImages { get; set; } = true;

        /// <summary>
        /// When extracting all images on a page, should the extractor get images from 'background-image' styles?
        /// Note this only works for inline styles, and not for images specified in separate css files.
        /// </summary>
        public bool GetInlineBackgroundImages { get; set; } = true;

        /// <summary>
        /// Should Cors Anywhere (https://cors-anywhere.herokuapp.com/) be used? Only required for Web Applications (e.g. Blazor).
        /// </summary>
        public bool UseCorsAnywhere { get; set; } = false;

        /// <summary>
        /// Disables an additional check (that the image url returns OK) before images are returned.
        /// Setting to true improves performance, but returns more false positives.
        /// </summary>
        public bool DisableValidityCheck { get; set; } = false;

        /// <summary>
        /// An instance of HttpClient to use. If not set, a new HttpClient is created.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Callback event when an image is found by the extractor.
        /// </summary>
        public ImageFoundEvent OnFoundImage { get; set; }

        /// <summary>
        /// Callback event when the extractor begins to explore a new Uri.
        /// </summary>
        public PageStartEvent OnStartNewPage { get; set; }

        /// <summary>
        /// Callback event when the extractor has finished exploring a Uri.
        /// </summary>
        public PageEndEvent OnEndNewPage { get; set; }

        /// <summary>
        /// Callback event when a new image is found. Checks if the extractor should continue or if enough images have been found.
        /// </summary>
        public ShouldStopCheck ShouldStopOnFoundImage { get; set; }
    }
}