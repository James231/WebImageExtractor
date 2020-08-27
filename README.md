# Web Image Extractor

.NET library to extract images and icons from websites. Provides options to select Favicons, Apple Touch Icons, or all images on a page.

[Blazor Demo](https://image-extractor.jam-es.com)

[Available on NuGet](https://www.nuget.org/packages/WebImageExtractor/)

Features (all are optional and can be speicified in `ExtractionSettings`):
- Extracts Favicons and Apple touch icons.
- Extracts images from `<img>` tags.
- Extracts images from `<link>` and `<meta>` tags within HTML head.
- Extracts images from background images in CSS `background-image: uri('https://...')`. Currently this only works for inline styles and not referenced `.css` files.
- Supports Uri Recursion. E.g. Extracting from `https://example.com/dir/page.html` also extracts from `https://example.com/dir` and `https://example.com`.
- Supports following Hyperlinks (specified in `<a>` tags) up to a certain depth and extracting images from those pages too.
- Supports triggering events when new images are found or new pages are explored.
- Supports ending the extraction algorithm when a condition is met (e.g. enough images found).
- Supports Blazor (including Client Side Blazor / Blazor WebAssembly).
- Includes example console application.

To see an online example use of this tool, checkout [this Blazor demo](https://image-extractor.jam-es.com). You can find the source code for the demo [here](https://github.com/James231/Web-Image-Extractor-Blazor).

**Note:** If you are after something specific, e.g. a way of getting 48px square `.ico` icons from websites ... please note that some websites might only have one icon available and others might not have any. This library isn't magic, so you might not get exactly what you want. It's always best to give a user choice over the best icon/image, or even let them provide their own, don't rely entirely on the algorithms.


## How does it work?

Favicons are found from the standard places. First the path `/favicon.ico` is checked, and then it checks for `<link>` tags in the HTML head. Any link with a valid Favicon `rel` property will be returned as a Favicon.

It works in a similar way for Apple Touch Icons.

When looking for all images on a page, Favicons and Apple touch icons are retrieved, then all other images in `<link>` and `<meta>` tags are retrieved, followed by all images in `<img>` tags, and finally all images specified using `background-image: url('https://...')` within inline styles (if enabled in settings). Note that images are not pulled from `background-image` styles set in separate `.css` files. It only works for inline styles.

Recursion works by exploring Uris with segments removed, and by following hyperlinks (`<a>` tags) on the page, providing these are enabled in the settings.


## Installation

Install using the NuGet Package Manager:
```
Install-Package WebImageExtractor -Version 1.0.0
```

Or

To use in your project you can download in the library [here]() or clone the repository and add the `WebImageExtractor` C# Project to your solution (then use a project reference in your own project).

The library is a .NET Standard 2.0 library so works with both .NET Framework and .NET Core.


## How to Use

Add the using reference to the top of your file:  
```
using WebImageExtractor;
```

Then use one of the following methods:

| Method  | Description |
| ------------- | ------------- |
| `Extractor.GetFavicons`  | Extracts all Favicons.  |
| `Extractor.GetAppleTouchIcons`  | Extracts all Apple Touch Icons.  |
| `Extractor.GetAllIcons`  | Extracts all Favicons and Apple Touch Icons from a page.  |
| `Extractor.GetPageImages`  | Extracts all images from a page - excluding Favicons and Apple Touch Icons.  |
| `Extractor.GetAllImages`  | Extracts all images from a page - including Favicons and Apple Touch Icons.  |

Each method is awaitable and accepts the Uri to a page (starting with `https://`) as a string. They return a list of images using the type `IEnumerable<WebImage>`.

```
IEnumerable<WebImage> images = await Extractor.GetFavicons("https://github.com");
```

To download an image using the `WebImage` class you can use the `WebImage.GetImageAsync` method:
```
using ImageMagick;
...
MagickImage image = await images[0].GetImageAsync();
```

Although this is awaitable, by default the result will be immediate as all images will have been downloaded in advance. But you can change this to download the image when `GetImageAsync` is called by enabling lazy loading in the settings.

If you have not changed this setting and cannot use `await`, instead use `WebImage.GetImageIfDownloaded`.

The `WebImage` class also contains information about whether the image is a Favicon, Apple touch icon, or background image.

Once you have a `MagickImage` you can convert it to your prefered format using [Magick.NET](https://github.com/dlemstra/Magick.NET). When using Blazor Client Side you may experience issues using Magick.NET this way. Instead you can use the Uri from `WebImage.Uri` to download the images manually. See the source code of the Blazor demo.

The above extraction methods accept settings through an instance of `ExtractionSettings`. For example:

```
ExtractionSettings settings = new ExtractionSettings();
IEnumerable<WebImage> images = await Extractor.GetFavicons("https://github.com", settings);
```


## Settings

The `ExtractionSettings` class allows you to customize how images are extracted.

**ExtractionSettings Properties:**

| Method  | Type | Description |
| ------------- | ------------- | ------------- |
| `SvgOnly`  | `bool` | Only extract Svgs? |
| `RecurseUri`  | `bool` | Recurse Uri segments and extract images from all? |
| `RecurseHyperlinks`  | `bool` | Extract images from any pages linked to by the given Uri? |
| `HyperlinkRecursionDepth`  | `int` | Number of layers of hyperlinks to explore for image extraction. |
| `LazyDownload`  | `bool` | Download images immediatley after extraction? Or download them when required? |
| `GetMetaTagImages`  | `bool` | When extracting all images on a page, should the extractor get images from `<meta>` tags in the html `<head>` |
| `GetLinkTagImages`  | `bool` | When extracting all images on a page, should the extractor get images from `<link>` tags in addition to favicons and apple touch icons. |
| `GetInlineBackgroundImages`  | `bool` | When extracting all images on a page, should the extractor get images from 'background-image' styles? Note this only works for inline styles, and not for images specified in separate css files. |
| `UseCorsAnywhere`  | `bool` | Should Cors Anywhere ([https://cors-anywhere.herokuapp.com/](https://cors-anywhere.herokuapp.com/)) be used? Only required for Web Applications (e.g. Blazor). |
| `DisableValidityCheck`  | `bool` | Disables an additional check (that the image url returns OK) before images are returned. Setting to true improves performance, but returns more false positives. |
| `HttpClient`  | `HttpClient` | An instance of `HttpClient` to use. If not set, a new `HttpClient` is created. |


**ExtractionSettings Events:**

| Event  |  Description |
| ------------- | ------------- |
| OnFoundImage | Callback event when an image is found by the extractor. |
| OnStartNewPage | Callback event when the extractor begins to explore a new Uri. |
| OnEndNewPage | Callback event when the extractor has finished exploring a Uri. |
| ShouldStopOnFoundImage | Callback event when a new image is found. Checks if the extractor should continue or if enough images have been found. |

## Example Use

The below example shows how to retrieve all images from a Uri, and search linked pages up to depth 3, along with Uri recursion. A line is written to the console each time an image is found. Images are saved to files as `jpg` images.
```
using WebImageExtractor;
using ImageMagick;

...

ExtractionSettings settings = new ExtractionSettings()
{
    RecurseUri = true,
    RecurseHyperlinks = true,
    HyperlinkRecursionDepth = 3,
};
settings.OnFoundImage += async (WebImage image) =>
{
    Console.WriteLine($"Found Image at Uri: {image.Uri}");
};
IEnumerable<WebImage> images = await Extractor.GetAllImages("https://github.com", settings);
int imageNum = 0;
foreach (WebImage image in images) {
    MagickImage downloadedImage = await image.GetImageAsync();
    downloadedImage.Write($"image-{imageNum}.jpg");
    imageNum++;
}
```


## License

This code is released under MIT license. This means you can use this for whatever you want. Modify, distribute, sell, fork, and use this as much as you like. Both for personal and commercial use. I hold no responsibility if anything goes wrong.

If you use this, you don't need to refer to this repo, or give me any kind of credit but it would be appreciated.

## Contributing

Pull Requests are welcome. But, note that by creating a pull request you are giving me permission to merge your code and release it under the MIT license mentioned above. At no point will you be able to withdraw merged code from the repository, or change the license under which it has been made available.