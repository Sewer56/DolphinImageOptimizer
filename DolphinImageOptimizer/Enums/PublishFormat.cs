namespace DolphinImageOptimizer.Enums
{
    public enum PublishFormat
    {
        None,
        DXT1,
        DXT5,
        DXTAuto,
        R8G8B8A8,
        BC7
    }

    public static class PublishFormatExtensions
    {
        public static string ToTexConvFormat(this PublishFormat format)
        {
            return format switch
            {
                PublishFormat.None => null,
                PublishFormat.DXT1 => "DXT1",
                PublishFormat.DXT5 => "DXT5",
                PublishFormat.R8G8B8A8 => "R8G8B8A8_UNORM",
                PublishFormat.BC7 => "BC7_UNORM",
                PublishFormat.DXTAuto => "DXT5",
                _ => null
            };
        }
    }
}