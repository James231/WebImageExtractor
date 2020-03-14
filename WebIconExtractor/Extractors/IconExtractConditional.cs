// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using ImageMagick;
using static WebIconExtractor.IconExtractor;

namespace WebIconExtractor.Extractors
{
    internal static class IconExtractConditional
    {
        public static MagickImage IconExtractMinimize(Uri uri, ImageNorm norm, ExtractOptions extractOptions = ExtractOptions.Recursive)
        {
            ImageNorm newNorm = (MagickImage image) => (-norm.Invoke(image));
            return IconExtractMaximize(uri, newNorm, extractOptions);
        }

        public static MagickImage IconExtractMaximize(Uri uri, ImageNorm norm, ExtractOptions extractOptions = ExtractOptions.Recursive)
        {
            MagickImage[] imgs = IconExtractAll.ExtractAll(uri, extractOptions);

            if (imgs == null)
            {
                return null;
            }

            if (imgs.Length == 0)
            {
                return null;
            }

            if (imgs.Length == 1)
            {
                return imgs[0];
            }

            double dist = norm.Invoke(imgs[0]);
            MagickImage bestImg = imgs[0];
            foreach (MagickImage img in imgs)
            {
                double newDist = norm.Invoke(img);
                if (newDist > dist)
                {
                    dist = newDist;
                    bestImg = img;
                }
            }

            return bestImg;
        }

        public static MagickImage IconExtractClosestRes(Uri uri, int width, int height, ExtractOptions extractOptions = ExtractOptions.Recursive)
        {
            ImageNorm imageNorm = (MagickImage image) => (Math.Pow(Math.Abs(image.Width - width), 2) * Math.Pow(Math.Abs(image.Height - height), 2));
            return IconExtractMinimize(uri, imageNorm, extractOptions);
        }

        public static MagickImage IconExtractClosestRatio(Uri uri, int width, int height, ExtractOptions extractOptions = ExtractOptions.Recursive)
        {
            ImageNorm imageNorm = (MagickImage image) => Math.Abs((width / height) - (image.Width / image.Height));
            return IconExtractMinimize(uri, imageNorm, extractOptions);
        }

        public static MagickImage IconExtractHighestRes(Uri uri, ExtractOptions extractOptions)
        {
            ImageNorm imageNorm = (MagickImage image) => (Math.Pow(image.Width, 2) + Math.Pow(image.Height, 2));
            return IconExtractMaximize(uri, imageNorm, extractOptions);
        }
    }
}