// -------------------------------------------------------------------------------------------------
// WebIconExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using ImageMagick;

namespace WebIconExtractor.App
{
    public class Program
    {
        /// <summary>
        /// Entry point for the console application
        /// </summary>
        /// <param name="args">Command Line Arguments</param>
        public static void Main(string[] args)
        {
            // Just change the output file path to end in '.png' (or another extension) and a png file will be output
            string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.ico");
            string uri = string.Empty;

            // Get uri from first command line argument and outputPath from second (if there is one)
            if (args != null)
            {
                if (args.Length > 0)
                {
                    uri = args[0];
                }

                if (args.Length > 1)
                {
                    outputFilePath = args[1];
                }
            }

            if (uri == null)
            {
                Console.WriteLine("Please provide a Url!");
                return;
            }

            ExtractIcon(uri, outputFilePath);
        }

        private static void ExtractIcon(string uriString, string outputPath)
        {
            Uri uri = new Uri(uriString);

            // Extract icons closest to 96x96 resolution (the Windows 10 "large icon" size)
            MagickImage icon = IconExtractor.IconExtractClosestRes(uri, 96, 96);
            if (icon == null)
            {
                Console.WriteLine("Failed to find a suitable icon.");
                return;
            }

            icon.Write(outputPath);
            Console.WriteLine("Output Icon file to:");
            Console.WriteLine(outputPath);
            return;
        }
    }
}
