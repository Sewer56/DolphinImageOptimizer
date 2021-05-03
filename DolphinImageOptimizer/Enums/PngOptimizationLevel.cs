namespace DolphinImageOptimizer.Enums
{
    public enum PngOptimizationLevel
    {
        Maximum,
        Medium,
        Minimum
    }

    public static class PngOptimizationLevelExtensions
    {
        public static string ToPingoLevel(this PngOptimizationLevel format)
        {
            return format switch
            {
                PngOptimizationLevel.Maximum => "-sa",
                PngOptimizationLevel.Medium  => "-s1",
                PngOptimizationLevel.Minimum => "-s9 -faster",
                _ => null
            };
        }
    }
}