## About The Project
 This is a WPF library for generating a color palette from an image ported from [Android's Palette API](https://developer.android.com/training/material/palette-colors)
 | Package |                    NuGet ID                     |                         NuGet Status                         |
| :-----: | :---------------------------------------------: | :----------------------------------------------------------: |
| WPF Color Picker  | [Color Picker](https://www.nuget.org/packages/Doushi/1.0.0) | [![Stat](https://img.shields.io/nuget/v/Doushi.svg)](https://www.nuget.org/packages/Doushi/1.0.0) |
 
## Installation
 The package can be installed by NuGet:
 ```powershell
 Install-Package Doushi -Version 1.0.0
 ```
 Or reference it in your project:
 ```xml
 <PackageReference Include="Doushi" Version="1.0.0" />
 ```

## Create a palette
 A `Palette` object gives you access to the primary colors in an image, as well as the corresponding colors for overlaid text. Use palettes to design your games's style and to dynamically change your games's color scheme based on a given source image.
 
 ### Generate a Palette instance
 Generate a `Palette` instance using `Palette`'s `Generate(BitmapSource image)` function
 ```csharp
 var bm = new BitmapImage(new Uri(@"C:/../...png"));
 Palette palette = Palette.Generate(bm);
 ```
 Based on the standards of material design, the palette library extracts commonly used color profiles from an image. Each profile is defined by a Target, and colors extracted from the texture image are scored against each profile based on saturation, luminance, and population (number of pixels in the texture represented by the color). For each profile, the color with the best score defines that color profile for the given image.
 
The palette library attempts to extract the following six color profiles:
* Light Vibrant
* Vibrant
* Dark Vibrant
* Light Muted
* Muted
* Dark Muted

Each of `Palette`'s `Get<Profile>Color()` methods returns the color in the palette associated with that particular profile, where `<Profile>` is replaced by the name of one of the six color profiles. For example, the method to get the Dark Vibrant color profile is `GetDarkVibrantColor()`. Since not all images will contain all color profiles, you can also provide a default color to return.

This figure displays a photo and its corresponding color profiles from the `Get<Profile>Color()` methods.
<p align="center">
<img src="https://developer.android.com/training/material/images/palette-library-color-profiles_2-1_2x.png" width="500" title="Figure 1">
</p>

```csharp
Color MutedColor = palette.GetMutedColor();
Color VibrantColor = palette.GetVibrantColor();
Color LightMutedColor = palette.GetLightMutedColor();
Color LightVibrantColor = palette.GetLightVibrantColor();
Color DarkMutedColor = palette.GetDarkMutedColor();
Color DarkVibrantColor = palette.GetDarkVibrantColor();
```
You can aso create more comprehensive color schemes using the `GetBodyTextColor()` and `GetTitleTextColor()` extension methods the `Color` struct. These methods return colors appropriate for use over the swatch’s color.
```csharp
var muted = palette.GetMutedColor(Colors.White);
mutedBtn.Background = new SolidColorBrush(muted);
mutedBtn.Foreground = new SolidColorBrush(muted.GetTitleTextColor());
```
An example image with its vibrant-colored toolbar and corresponding title text color.
<p align="center">
<img src="https://developer.android.com/training/material/images/palette-library-title-text-color_2-1_2x.png" width="300" title="Figure 2">
</p>

## Preview
 These are some preview images from the example project
 <p align="center">
<img src="Preview/Screenshot 2021-12-19 161054.png" width="600" title="Preview 1">
</p>
<p align="center">
<img src="Preview/Screenshot 2021-12-19 160946.png" width="600" title="Preview 2">
</p>