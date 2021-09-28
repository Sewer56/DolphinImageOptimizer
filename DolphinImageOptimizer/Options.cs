using CommandLine;
using DolphinImageOptimizer.Enums;

namespace DolphinImageOptimizer
{
    internal class Options
    {
        public const string DefaultRegexPattern = @"(?:tex1_)?(\d*)x(\d*)_.*";

        [Option(Required = true, HelpText = "The folder to optimize.")]
        public string Source { get; private set; }

        [Option(Required = true, HelpText = "Target resolution to optimize image size to. Set to first option greater than desired screen resolution.", Default = TargetResolution.P960)]
        public TargetResolution Target { get; private set; }

        [Option(Required = false, HelpText = "PNG Optimization Level. Use Maximum if releasing as PNG, Medium for testing. This is auto set to Minimal when Publishing as DDS.", Default = PngOptimizationLevel.Medium)]
        public PngOptimizationLevel Optimization { get; set; }

        [Option(Required = false, HelpText = "Converts textures to DDS and deletes original. Recommended BC7 for Dolphin and R8G8B8A8 (with UseLZ4) for Riders.Tweakbox. DXTAuto uses DXT1 when no transparency, DXT5 when transparency. If you know better, use PublishAdvanced instead.", Default = PublishFormat.None)]
        public PublishFormat Publish { get; private set; }

        [Option(Required = false, HelpText = "Custom format to pass to TexConv. e.g. BC5_UNORM")]
        public string PublishAdvanced { get; private set; }

        [Option(Required = false, HelpText = "Riders.Tweakbox specific. Compresses output DDS files with LZ4. Use with R8G8B8A8.", Default = false)]
        public bool? UseLZ4 { get; private set; }

        [Option(Required = false, HelpText = "Generates mipmaps if set to true. Else set to false.", Default = true)]
        public bool? GenerateMipmaps { get; private set; }

        [Option(Required = false, HelpText = "Regular expression pattern for obtaining original intended image size from texture name.", Default = DefaultRegexPattern)]
        public string RegexPattern { get; private set; }
    }
}
