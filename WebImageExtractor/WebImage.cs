// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using ImageMagick;

namespace WebImageExtractor
{
    /// <summary>
    /// Class representing an extracted image.
    /// </summary>
    public class WebImage
    {
        private MagickImage image;
        private bool downloadAttempted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebImage"/> class.
        /// </summary>
        /// <param name="uri">Uri link to the image.</param>
        /// <param name="isFavicon">Is the image a Favicon?</param>
        /// <param name="isAppleTouchIcon">Is the image an Apple Touch Icon?</param>
        /// <param name="isBackgroundImage">Is the image a Background Image?</param>
        public WebImage(string uri, bool isFavicon = false, bool isAppleTouchIcon = false, bool isBackgroundImage = false)
        {
            Uri = uri;
            IsFavicon = isFavicon;
            IsAppleTouchIcon = isAppleTouchIcon;
            IsBackgroundImage = isBackgroundImage;
        }

        /// <summary>
        /// Is the image a Favicon?
        /// </summary>
        public bool IsFavicon { get; set; }

        /// <summary>
        /// Is the image an Apple Touch Icon?
        /// </summary>
        public bool IsAppleTouchIcon { get; set; }

        /// <summary>
        /// Is the image a background image?
        /// </summary>
        public bool IsBackgroundImage { get; set; }

        /// <summary>
        /// Uri linking to the image.
        /// </summary>
        public string Uri { get; set; }

        internal void SetImage(MagickImage val)
        {
            image = val;
        }

        /// <summary>
        /// Downloads the image or returns it if already downloaded.
        /// </summary>
        /// <returns>Returns the image as instance of <see cref="MagickImage"/>.</returns>
        public async Task<MagickImage> GetImageAsync()
        {
            if (!downloadAttempted)
            {
                await DownloadImage();
            }

            return image;
        }

        /// <summary>
        /// Returns the MagickImage if it has been downloaded. Otherwise returns null.
        /// </summary>
        /// <returns>MagickImage if it has been downloaded, otherwise null.</returns>
        public MagickImage GetImageIfDownloaded()
        {
            return image;
        }

        private async Task DownloadImage()
        {
            if (string.IsNullOrEmpty(Uri))
            {
                return;
            }

            if (downloadAttempted)
            {
                return;
            }

            image = await ImageDownloader.DownloadMagickImage(new Uri(Uri));
            downloadAttempted = true;
        }
    }
}