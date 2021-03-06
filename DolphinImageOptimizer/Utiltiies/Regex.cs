using System.Text.RegularExpressions;

namespace DolphinImageOptimizer.Utiltiies
{
    public class Regex
    {
        private static System.Text.RegularExpressions.Regex _imageResolutionRegex = new System.Text.RegularExpressions.Regex(@"(?:tex1_)?(\d*)x(\d*)_.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void Init(string pattern) => _imageResolutionRegex = new System.Text.RegularExpressions.Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryGetImageSizeFromName(string imageName, out int width, out int height)
        {
            var matches = _imageResolutionRegex.Match(imageName);
            if (matches.Groups.Count < 3)
            {
                width = 0;
                height = 0;
                return false;
            }

            width = int.Parse(matches.Groups[1].Value);
            height = int.Parse(matches.Groups[2].Value);
            return true;
        }
    }
}