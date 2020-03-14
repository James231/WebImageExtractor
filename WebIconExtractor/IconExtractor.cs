// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using ImageMagick;
using WebIconExtractor.Extractors;

namespace WebIconExtractor
{
    public static class IconExtractor
    {
        /// <summary>
        /// Delegate which assigns a numerical value to an image, which can be maximized or minimized.
        /// </summary>
        /// <param name="image">Image to assign value to.</param>
        /// <returns>Value assigned to image.</returns>
        public delegate double ImageNorm(MagickImage image);

        /// <summary>
        /// Delegate which assigns boolean value to image. True if image is deemed 'Good Enough'.
        /// </summary>
        /// <param name="image">Image to assign value to.</param>
        /// <returns>Boolean indicating if the image is good enough and searching should stop.</returns>
        public delegate bool IconValid(MagickImage image);

        /// <summary>
        /// Extracts Favicon from uri.
        /// </summary>
        /// <param name="uri">Uri to extract Favicon from.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Favicon as a MagickImage</returns>
        public static MagickImage ExtractFavicon(Uri uri, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return FaviconExtract.ExtractFavicon(uri, extractOptions);
        }

        /// <summary>
        /// Extracts Apple Touch Icon from uri.
        /// </summary>
        /// <param name="uri">Uri to extract Apple Touch Icon from.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Apple Touch Icon as a MagickImage</returns>
        public static MagickImage ExtractTouchIcon(Uri uri, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return TouchIconExtract.ExtractTouchIcon(uri, extractOptions);
        }

        /// <summary>
        /// Extracts the first Icon from a uri which is found to be 'Good Enough'.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="check">Delegate method specifying which images are 'Good Enough'.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Extracted Icon as MagickImage.</returns>
        public static MagickImage ExtractGoodEnough(Uri uri, IconValid check, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractGoodEnough.ExtractGoodEnough(uri, check, extractOptions);
        }

        /// <summary>
        /// Extracts all icons from a uri which it can find.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Extracted Icons as MagickImage array.</returns>
        public static MagickImage[] ExtractAll(Uri uri, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractAll.ExtractAll(uri, extractOptions);
        }

        /// <summary>
        /// Extracts the highest resolution icon it can find from a uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Highest Resolution Icon Extracted as MagickImage.</returns>
        public static MagickImage IconExtractHighestRes(Uri uri, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractConditional.IconExtractHighestRes(uri, extractOptions);
        }

        /// <summary>
        /// Extracts Icon which has resolution closest to provided resolution from a uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="width">Desired width of image.</param>
        /// <param name="height">Desired height of image.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Extracted Icon with resoltion closest to provided resolution as MagickImage.</returns>
        public static MagickImage IconExtractClosestRes(Uri uri, int width, int height, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractConditional.IconExtractClosestRes(uri, width, height, extractOptions);
        }

        /// <summary>
        /// Extracts Icon which has aspect ratio closest to provided ratio from a uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="width">Desired width ratio of image.</param>
        /// <param name="height">Desired height ratio of image.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Extracted Icon with aspect ratio closest to provided ratio as MagickImage.</returns>
        public static MagickImage IconExtractClosestRatio(Uri uri, int width, int height, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractConditional.IconExtractClosestRatio(uri, width, height, extractOptions);
        }

        /// <summary>
        /// Extracts Icon which minimizes a numerical value (norm) specified through delegate function, from a uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="norm">Delegate function which gives each Icon a value, which is minimized over.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Icon with minimized norm.</returns>
        public static MagickImage IconExtractMinimize(Uri uri, ImageNorm norm, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractConditional.IconExtractMinimize(uri, norm, extractOptions);
        }

        /// <summary>
        /// Extracts Icon which maximizes a numerical value (norm) specified through delegate function, from a uri.
        /// </summary>
        /// <param name="uri">Uri to extract from.</param>
        /// <param name="norm">Delegate function which gives each Icon a value, which is maximized over.</param>
        /// <param name="extractOptions">Specifies whether extractions should recursively go back through Uri.</param>
        /// <returns>Icon with maximized norm.</returns>
        public static MagickImage IconExtractMaximize(Uri uri, ImageNorm norm, ExtractOptions extractOptions = ExtractOptions.RecurseUntilSuccess)
        {
            return IconExtractConditional.IconExtractMaximize(uri, norm, extractOptions);
        }
    }
}
