// -------------------------------------------------------------------------------------------------
// WebImageExtractor - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using ImageMagick;

namespace WebImageExtractor.App
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide a Uri as a Command Line Argument!");
                Console.WriteLine("Example Use:");
                Console.WriteLine("WebImageExtracter.exe \"https://google.com\"");
                return;
            }

            string uri = args[0];

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string exeDir = Path.GetDirectoryName(exePath);
            string outputPath = Path.Combine(exeDir, "Output");
            Directory.CreateDirectory(outputPath);
            Utils.ClearDirectory(outputPath);

            Console.WriteLine("Extracting Images ...");
            ExtractionSettings settings = new ExtractionSettings(false, false, true, true, 3);
            IEnumerable<WebImage> images = Extractor.GetAllImages(uri, settings).Result;

            int i = 0;
            foreach (WebImage image in images)
            {
                var magickImages = image.GetImagesIfDownloaded();
                foreach (MagickImage magickImage in magickImages)
                {
                    string outputFilePath = Path.Combine(outputPath, $"{i}.{Enum.GetName(typeof(MagickFormat), magickImage.Format).ToLowerInvariant()}");
                    if (magickImage.Format != MagickFormat.Svg)
                    {
                        magickImage.Write(outputFilePath, magickImage.Format);
                    }
                    else
                    {
                        // MagickImage.Write would rasterize svgs which is not what we want
                        // Instead just download to a file
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(image.Uri);
                        request.Method = "GET";
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                            {
                                string responseText = reader.ReadToEnd();
                                StreamWriter writer = new StreamWriter(outputFilePath, false);
                                writer.Write(responseText);
                                writer.Close();
                            }
                        }
                    }

                    i++;
                }
            }

            Console.WriteLine("Finished");
        }
    }
}
