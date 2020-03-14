# WebIconExtractor

C# .NET Standard 2.0 library to automate extraction of icons from websites.

The library provides multiple methods each with different priorities. Maybe you want ...  
... small favicons?  
... higher resolution icons?  
... icons close to a specific resolution or aspect ratio?  
... lots of icons which you can choose between manually?

Please note that some websites might only have one icon available and others might not have any. And this library doesn't use magic, so you might not get exactly what you want.

However, better established websites tend to have a lot of icons associated with them so using different options will give different results.

Note: This won't work on .NET platforms where System.Drawing.Common is not available (e.g. it won't work on Blazor WebAssembly).

## Download

If you just want the .NET Standard library you can download it [here](https://github.com/James231/WebIconExtractor/raw/master/Build/WebIconExtractor.dll).

If you want the source code or want to build it yourself you can clone the repository and open the solution in Visual Studio. The solution also includes a .NET Framework console application which accepts a URL as a  command line argument and pulls an icon for you (it looks for the icon closest to 96x96 resolution, ideal for Windows 10 icons).

## How does it work?

It looks for favicons and Apple touch icons in the standard places but also checks for icon links in `<link>` and `<meta>` tags in HTML page headers.

If it fails to find icons at a URL it recursively goes back a segment ('directory') in the URL and tries again until the root domain is reached. This behaviour can be changed.

## How to Use

The library returns images in a `MagickImage` format, using the `ImageMagick` library for .NET.  

Note: All methods return `null` if they fail. No exceptions are thrown.

Add using references to the top of your file:  
```
using WebIconExtractor;
using ImageMagick;
```

To get a favicon (very low resolution icon but with high success rate):  
```
Uri uri = new Uri("https://google.com");
MagickImage icon =  IconExtractor.ExtractFavicon(uri);
```

or to get a Apple Touch Icon:
```
MagickImage icon =  IconExtractor.ExtractTouchIcon(uri);
```


To get the highest resolution Icon available:
```
MagickImage icon =  IconExtractor.IconExtractHighestRes(uri);
```

To get an array of all the Icons it can find:
```
MagickImage[] icons =  IconExtractor.ExtractAll(uri);
```

To get a single icon which is as close to a specific resolution as possible (in this case 120x80 pixels):
```
MagickImage icon =  IconExtractor.IconExtractClosestRes(uri, 120, 80);
```

To get a single (highest resolution) icon which is as close to a specific aspect ratio as possible (in this case 3:2):
```
MagickImage icon =  IconExtractor.IconExtractClosestRatio(uri, 3, 2);
```

And note that you can save `MagickImage`s to files in the following way:
```
string outputFilePath = "C:\...\output.ico";
icon.Write(outputPath);
```

where the file extension on the file path can by changed to any format supported by ImageMagick including `.png`, `.bmp` and `.jpg`. The extension `.ico` is used for icons in Windows.

All `IconExtractor` methods also support an `ExtractOptions` parameter, for example:
```
MagickImage[] icons =  IconExtractor.ExtractAll(uri, ExtractOptions.Recursive);
```
where  
- `ExtractOptions.SingleUrl` - Will only extract icons from the exact URL given.  
- `ExtractOptions.Recursive` - Will recursively extract icons from the URL, removing segments from the end. e.g. URL `https://example.com/a/b` with this option will extract icons from `https://example.com/a/b`, `https://example.com/a` and `https://example.com`.  
- `ExtractOptions.RecurseUntilSuccess` (Default) - After any recursive iteration, if an icon is found, then the recusion stops and no more segments are removed from the URL.

### Additional Icon Requirements

If you require something more specific there are a few more methods you might find useful.

Firstly if you define a method which assigns icons with a numerical value (we will call such a method a 'Norm'), you can find the icon which maximizes or minimizes the norm. For example, we can define a method which gives a low value for square images:
```
public double CloseToSquareNorm(MagickImage icon) {
	return Math.Abs(icon.Width - icon.Height);
}
```
Then we can find the Icon which minimizes this as follows:
```
MagickImage icon =  IconExtractor.IconExtractMinimize(uri, CloseToSquareNorm);
```
This will give an icon which is as close to being square (width=height) as possible. Similarily there is a method for maximizing a norm `IconExtractMaximize`.

### Faster Icon Extraction

If you need icons fast I would recommend just using the `ExtractFavicon` or `ExtractTouchIcon` methods. But, if you have some additional requirements on the icon, there is another option:

First define a method which takes an icon and decides whether it is acceptable or not, for example if you need a square icon:
```
public bool IsAcceptable(MagickImage icon) {
	return icon.width == icon.height;
} 
```

Then use the following method which will extract icons in a sensible order until an acceptable icon is found:
```
MagickImage icon =  IconExtractor.ExtractGoodEnough(uri, IsAcceptable);
```

Obviously the stricter you make your `IsAcceptable` method, the less likely an acceptable icon will be found. You will need to find the right balance between the icon properties you need and the failure rate you are happy with.

## References

- Uses [Magick.NET](https://github.com/dlemstra/Magick.NET) which version of ImageMagick for .NET and C#. I use this to convert between images like pngs and `.ico` files.
- Uses [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack/) which is a HTML parser for C#. This is used to get icon references from HTML `<link>` or `<meta>` tags.


## License

This code is released under MIT license. This means you can use this for whatever you want. Modify, distribute, sell, fork, and use this as much as you like. Both for personal and commercial use. I hold no responsibility if anything goes wrong.

If you use this, you don't need to refer to this repo, or give me any kind of credit but it would be appreciated.

## Contributing

Pull Requests are welcome. But, note that by creating a pull request you are giving me permission to merge your code and release it under the MIT license mentioned above. At no point will you be able to withdraw merged code from the repository, or change the license under which it has been made available.