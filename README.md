# Dolphin Image Optimizer

Dolphin Image Optimizer is a quick tool I made to for my own personal use to optimize individual textures found in Dolphin Emu texture packs.

It auto-optimizes texture packs to desired target resolutions and optimized file formats, allowing for smaller downloads, faster loading times, less stutters and better overall performance. 

The nice benefit of this tool is you can make your assets super high resolution/quality and simply throw them at this tool to create optimal DDS compressed downloads for multiple resolutions.

## Features

### Automatic Texture Resizing
- Automatically scales textures to the desired target resolution.
- Using original resolution from file name as base.
- Smaller downloads with faster performance, optimized for target resolutions.

Additionally, for performance reasons, if the texture is not a power of 2, it will be rounded to the nearest power.

### PNG Optimization
Using Pingo (see below), PNGs can be optimized to various levels.
Use when you wish to include the reference PNGs alongside your optimized downloads.

### DDS Conversion
Using natively supported texture formats lead to lower memoey usage, faster load times and less stutter.

- Built-in formats: 
  - BC7 (Recommended)
  - Auto DXT (DXT1 or DXT5 depending on file)
  - LZ4 Compression Support (for *Riders.Tweakbox**
  - Or your own custom format if you're more advanced.

### Dummy Detection
If an image is a flat colour, empty or completely transparent, it's reduced to 4 pixels for optimal performance.

### Aspect Ratio Fixing
Sometimes artists working on textures don't maintain the aspect ratio of the original textures; this tool automatically fixes it.

## Multithreaded
In other words, it's fast.

## Help
You should be familiar with using commandline applications; if you're not, google it 😇.

Get Help:
`DolphinImageOptimizer.exe --help`

```
DolphinImageOptimizer 1.0.0
Created by Sewer56, licensed under GNU LGPL V3

  --source             Required. The folder to optimize.

  --target             Required. (Default: P960) Target resolution to optimize
                       image size to. Set to first option greater than desired
                       screen resolution. Valid values: P960, P1920, P3840

  --optimization       (Default: Medium) PNG Optimization Level. Use Maximum if
                       releasing as PNG, Medium for testing. This is auto set to
                       Minimal when Publishing as DDS. Valid values: Maximum,
                       Medium, Minimum

  --publish            (Default: None) Converts textures to DDS and deletes
                       original. Recommended BC7 for Dolphin and R8G8B8A8 (with
                       UseLZ4) for Riders.Tweakbox. DXTAuto uses DXT1 when no
                       transparency, DXT5 when transparency. If you know better,
                       use PublishAdvanced instead. Valid values: None, DXT1,
                       DXT5, DXTAuto, R8G8B8A8, BC7

  --publishadvanced    Custom format to pass to TexConv. e.g. BC5_UNORM

  --uselz4             (Default: false) Riders.Tweakbox specific. Compresses
                       output DDS files with LZ4. Use with R8G8B8A8.

  --help               Display this help screen.
```

## Example Usage

### Common

Optimize for up to 1440p, export as Optimized PNG:
`DolphinImageOptimizer.exe --source "C:\Users\Sewer56\Downloads\HD Texture Pack V1.2 Reference PNGs\GXEE8P" --target P1920 --optimization Maximum`

### Dolphin

When releasing your texture packs and working with `Dolphin` you it is recommended to use DDS BC7 for optimal performance and size.

You can include your source and/or optimized PNGs in separate downloads :)

Optimize for up to 1440p, export as DDS BC7 (Optimal):
`DolphinImageOptimizer.exe --source "C:\Users\Sewer56\Downloads\HD Texture Pack V1.2 Reference PNGs\GXEE8P" --target P1920 --publish BC7`

Optimize for up to 5K (2880p), export as DDS BC7 (Optimal):
`DolphinImageOptimizer.exe --source "C:\Users\Sewer56\Downloads\HD Texture Pack V1.2 Reference PNGs\GXEE8P" --target P3840 --publish BC7`

### Riders.Tweakbox

Export Medium Quality (Low End Machines!):
`DolphinImageOptimizer.exe --source "C:\Users\Sewer56\Downloads\HD Texture Pack V1.2 Reference PNGs\GXEE8P" --target P1920 --publish DXTAuto`

This option (DXTAuto) will use **DXT1** when Alpha (transparency) is not present and **DXT5** when it is present.

Export Full Quality:
`DolphinImageOptimizer.exe --source "C:\Users\Sewer56\Downloads\HD Texture Pack V1.2 Reference PNGs\GXEE8P" --target P1920 --publish R8G8B8A8 --uselz4`

This will export a DDS with the format, `R8G8B8A8` and compress it using the lz4 compression algorithm.

Note that `R8G8B8A8` is an uncompressed format, which may use a considerably amount of video memory.

#### Recommendation

For optimal performance, it is generally recommended to use `R8G8B8A8` for UI assets drawn to the screen while using `DXTAuto` for Stage textures.

## Usage Hints

GameCube's internal resolution is (480p), i.e. P480.

As a general rule of thumb:

- For 720p target 2x GameCube Resolution (P960)
- For 1440p target 4x GameCube Resolution (P1920) 
- For 5K target 8x GameCube Resolution (P3840)

## Download

Please navigate to the [releases page](https://github.com/Sewer56/DolphinImageOptimizer/releases) for the latest version.

## Credits

Image Optimiser uses the [open source TexConv utility](https://github.com/microsoft/DirectXTex) under the MIT license.

---

Image Optimiser uses the `pingo` donationware utility by Cédric Louvrier in order to efficiently compress PNGs under the hood. 

If you like what it's doing, consider [donating](https://css-ig.net/donate) to the original author or [visiting the author's website.](https://css-ig.net/donate)