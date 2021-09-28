using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using DolphinImageOptimizer.Enums;
using DolphinImageOptimizer.Utiltiies;
using K4os.Compression.LZ4;
using Reloaded.Memory.Streams;
using Image = DolphinImageOptimizer.Utiltiies.Image;

namespace DolphinImageOptimizer
{
    class Program
    {
        public const string PngFilter = "*.png";
        public const string DdsFilter = "*.dds";
        private const int MinWidth = 4;
        private const int MinHeight = 4;
        
        static async Task Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.AutoHelp = true;
                with.CaseSensitive = false;
                with.CaseInsensitiveEnumValues = true;
                with.EnableDashDash = true;
                with.HelpWriter = null;
            });

            var parserResult = parser.ParseArguments<Options>(args);
            parserResult.WithNotParsed(errs => HandleParseError(parserResult, errs));
            await parserResult.WithParsedAsync<Options>(Run);
        }

        private static void HandleParseError(ParserResult<Options> options, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(options, help =>
            {
                help.Copyright = "Created by Sewer56, licensed under GNU LGPL V3";
                help.AutoHelp = true;
                help.AutoVersion = false;
                help.AddDashesToOption = true;
                help.AddEnumValuesToHelpText = true;
                help.AdditionalNewLineAfterOption = true;
                return HelpText.DefaultParsingErrorsHandler(options, help);
            }, example => example, true);

            Console.WriteLine(helpText);
            Console.WriteLine("NOTE: Please take a backup of your work before using this tool!!");
        }

        private static async Task Run(Options options)
        {
            Regex.Init(options.RegexPattern);
            bool convertToDds = options.Publish != PublishFormat.None || !string.IsNullOrEmpty(options.PublishAdvanced);
            if (convertToDds)
                options.Optimization = PngOptimizationLevel.Minimum;

            await OptimizeImages(options);

            if (convertToDds)
            {
                Console.WriteLine($"Converting to DDS!");
                await ConvertToDds(options);
            }

            Console.WriteLine("All Done!");
        }

        static async Task OptimizeImages(Options options)
        {
            // Partition for each thread.
            var files       = Directory.GetFiles(options.Source, PngFilter, SearchOption.AllDirectories);
            var partitioner = Partitioner.Create(0, files.Length);

            Parallel.ForEach(partitioner, (range, state) =>
            {
                for (int x = range.Item1; x < range.Item2; x++)
                    ResizeFile((int) options.Target, files[x]);
            }); 

            Console.WriteLine($"Compressing, Hang on Tight! | {options.Source}, Hang on Tight!");
            await Tools.Tools.RunPingoForDirectoryRecursive(options.Source, options.Optimization.ToPingoLevel());
        }

        private static void ResizeFile(int power, string file)
        {
            // Parse size from filename
            if (Regex.TryGetImageSizeFromName(Path.GetFileName(file), out int width, out int height))
            {
                // Scale to desired IR.
                width  *= power;
                height *= power;

                System.Drawing.Image image = null;
                Bitmap bitmap  = null;
                Bitmap resized = null;
                var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                try
                {
                    image  = System.Drawing.Image.FromStream(stream);
                    bitmap = new Bitmap(image);

                    if (Image.AllOneColor(bitmap))
                    {
                        resized = Image.ResizeImage(image, MinWidth, MinHeight);
                    }
                    else
                    {
                        if (image.Width > width && image.Height > height)
                        {
                            resized = Image.ResizeImage(image, width, height);
                        }
                        else if (!IsPowerOfTwo(image.Width) || !IsPowerOfTwo(image.Height))
                        {
                            resized = Image.ResizeImage(image, NearestPowerOfTwo((uint) image.Width), NearestPowerOfTwo((uint) image.Height));
                        }
                        else
                        {
                            var modW = image.Width  % 4;
                            var modH = image.Height % 4;

                            if (modW != 0 || modH != 0)
                            {
                                Console.WriteLine($"[WARNING]: {file}, W,H Not Multiples of 4. Trying to fix this.");
                                resized = Image.ResizeImage(image, image.Width + (4 - modW), image.Height + (4 - modH));
                            }

                            Console.WriteLine($"[SKIPPING]: {file}");
                            goto exit;
                        }
                    }

                    Console.WriteLine($"Saving: {file}");
                    resized.Save(file, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EXCEPTION!!] {ex.Message}");
                }
                finally
                {
                    image?.Dispose();
                    bitmap?.Dispose();
                    resized?.Dispose();
                    stream?.Dispose();
                }

                exit:;
            }
        }

        static async Task ConvertToDds(Options options)
        {
            var files = Directory.GetFiles(options.Source, PngFilter, SearchOption.AllDirectories);

            // Auto Alpha
            if (options.Publish == PublishFormat.DXTAuto)
            {
                // Partition for each thread.
                var tasks = files.Select(async file =>
                {
                    await AutoDdsConvert(file, options.GenerateMipmaps);
                });

                await Task.WhenAll(tasks);
            }
            else
            {
                var format = options.Publish.ToTexConvFormat();
                if (!string.IsNullOrEmpty(options.PublishAdvanced))
                    format = options.PublishAdvanced;
                
                await Tools.Tools.RunTexConvForDirectoryRecursive(options.Source, format, options.GenerateMipmaps);
            }

            foreach (var file in files)
                File.Delete(file);

            // Compress with LZ4 if needed
            if (options.UseLZ4)
            {
                Console.WriteLine("Compressing DDS");
                var ddsFiles    = Directory.GetFiles(options.Source, DdsFilter, SearchOption.AllDirectories);
                var partitioner = Partitioner.Create(0, ddsFiles.Length);

                Parallel.ForEach(partitioner, (range, state) =>
                {
                    for (int x = range.Item1; x < range.Item2; x++)
                    {
                        var file    = ddsFiles[x];
                        Console.WriteLine($"Compressing DDS: {file}");

                        var data    = File.ReadAllBytes(file);
                        Compression.PickleToFile(file + ".lz4", data, LZ4Level.L12_MAX);
                    }
                }); 

                foreach (var file in ddsFiles)
                    File.Delete(file);
            }
        }

        private static async Task AutoDdsConvert(string file, bool generateMipmaps)
        {
            var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            System.Drawing.Image image = null;
            Bitmap bitmap = null;

            try
            {
                image  = System.Drawing.Image.FromStream(stream);
                bitmap = new Bitmap(image);

                if (Image.HasMultiBitTransparency(bitmap))
                {
                    await Tools.Tools.RunTexConvForFile(file, PublishFormat.DXT5.ToTexConvFormat(), generateMipmaps);
                }
                else
                {
                    await Tools.Tools.RunTexConvForFile(file, PublishFormat.DXT1.ToTexConvFormat(), generateMipmaps);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION!!] {ex.Message}");
            }
            finally
            {
                image?.Dispose();
                bitmap?.Dispose();
                stream?.Dispose();
            }
        }

        private static bool IsPowerOfTwo(int x) => (x & (x - 1)) == 0;
        private static int NearestPowerOfTwo(uint x)
        {
            var next = 1 << (sizeof(uint) * 8 - BitOperations.LeadingZeroCount(x - 1));
            var last = next >> 1;
            return (next - x) > (x - last) ? last : next;
        }
    }
}
