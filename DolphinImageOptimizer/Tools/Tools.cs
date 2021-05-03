using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DolphinImageOptimizer.Tools
{
    /// <summary>
    /// Provides access to external tools.
    /// </summary>
    public static class Tools
    {
        public static async Task RunTexConvForDirectoryRecursive(string directory, string format)
        {
            await RunProcessAsync("Tools/texconv.exe", $"-f {format} -m 1 -r:keep -bc x -o \"{directory}\" \"{directory + "\\" + Program.PngFilter}\"");
        }

        public static async Task RunTexConvForFile(string file, string format)
        {
            await RunProcessAsync("Tools/texconv.exe", $"-f {format} -if LINEAR_DITHER -m 1 -r:keep -bc dx -o \"{Path.GetDirectoryName(file)}\" \"{file}\"");
        }

        public static async Task RunPingoForDirectoryRecursive(string directory, string optimizationLevel)
        {
            await RunProcessAsync("Tools/pingo.exe", $"-strip {optimizationLevel} \"{directory + "\\*"}\"");
        }

        public static async Task RunProcessAsync(string toolPath, string arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = toolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }
    }
}
