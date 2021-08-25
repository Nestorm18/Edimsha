using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using Edimsha.Core.Conversor;
using Edimsha.Core.Editor;
using Edimsha.Core.Models;
using Edimsha.Core.Utils;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming

namespace Edimsha.CLI
{
    class Program
    {
        private static CancellationTokenSource _token;

        static void Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.CaseInsensitiveEnumValues = true;
            });
            
            parser.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            switch (opts.RunMode)
            {
                case ViewType.Editor:
                    RunEditorModeInCLI(opts);
                    break;
                case ViewType.Conversor:
                    RunConversorModeInCLI(opts);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("[ERROR] Parsing cannot be done!");
        }

        private static void RunEditorModeInCLI(Options opts)
        {
            var config = new EditorOptions
            {
                IterateSubdirectories = opts.IterateSubdirectories,
                OutputFolder = opts.OutputFolder,
                Edimsha = opts.Edimsha,
                AlwaysIncludeOnReplace = opts.AlwaysIncludeOnReplace,
                KeepOriginalResolution = opts.KeepOriginalResolution,
                CompresionValue = opts.CompresionValue,
                OptimizeImage = opts.OptimizeImage,
                ReplaceForOriginal = opts.ReplaceForOriginal,
                Resolution = new Resolution(opts.Width, opts.Height),
                Paths = opts.Paths.ToList()
            };

            var pathsAsFolder = opts.PathsAsFolder;

            // Bad resolution or not used but not wanted the original values cannot be done
            if (!config.Resolution.IsValid() && !opts.KeepOriginalResolution)
                Console.WriteLine("[ERROR] The resolution value in -w/--width or -h/--height is not valid.");

            config.Paths = ValidatePaths(config.Paths, pathsAsFolder, config.IterateSubdirectories, ViewType.Editor);

            var editor = new Editor(config);
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += ProgressOnProgressChanged;
            _token = new CancellationTokenSource();

            editor.ExecuteProcessing(progress, _token);
        }

        private static void RunConversorModeInCLI(Options opts)
        {
            var config = new ConversorOptions
            {
                IterateSubdirectories = opts.IterateSubdirectories,
                OutputFolder = opts.OutputFolder,
                Edimsha = opts.Edimsha,
                CurrentFormat = opts.ImageType,
                Paths = opts.Paths.ToList()
            };
            
            var pathsAsFolder = opts.PathsAsFolder;
            
            config.Paths = ValidatePaths(config.Paths, pathsAsFolder, config.IterateSubdirectories, ViewType.Conversor);
            
            var conversor = new Conversor(config);
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += ProgressOnProgressChanged;
            _token = new CancellationTokenSource();

            conversor.ExecuteProcessing(progress, _token);
        }
        
        private static List<string> ValidatePaths(List<string> paths, string pathsAsFolder, bool iterateSubdirectories, ViewType type)
        {
            if (paths.Count == 0 && string.IsNullOrEmpty(pathsAsFolder))
            {
                Console.WriteLine("[ERROR] Must use --paths to set list of images or --pathsAsFolder to use a folder that contains images");
                Environment.Exit(-1);
            }

            // Find all images in folder if path list is zero using pathsAsFolder otherwise Paths have priority always.
            if (paths.Count == 0)
                paths = FindAllImagesInPaths(pathsAsFolder, iterateSubdirectories);

            // If path are not valid return false and the paths with the problem
            var badPaths = paths.Where(path => !File.Exists(path)).ToList();

            if (badPaths.Count > 0)
            {
                Console.WriteLine("[ERROR] Some of the paths provided are not valid.");

                foreach (var badPath in badPaths)
                    Console.WriteLine($"[ERROR] Path: \"{badPath}\" is wrong, must be a image not a directory or other format.");

                Environment.Exit(-1);
            }

            return ListCleaner.PathWithoutDuplicatesAndGoodFormats(new List<string>(), paths, type).ToList();
        }

        private static void ProgressOnProgressChanged(object? sender, ProgressReport e)
        {
            switch (e.ReportType)
            {
                case ReportType.Percent:
                    Console.WriteLine($"Percentage to finish the processing {(int) e.Data}%");
                    break;
                case ReportType.MessageB:
                    Console.WriteLine((string) e.Data);
                    break;
                case ReportType.Finalizated:
                    Console.WriteLine("Finished!");
                    break;
            }
        }

        private static List<string> FindAllImagesInPaths(string pathsAsFolder, bool iterateSubdirectories)
        {
            var temp = new List<string>();

            if (Directory.Exists(pathsAsFolder))
                temp.AddRange(iterateSubdirectories
                    ? Directory.GetFiles(pathsAsFolder, "*", SearchOption.AllDirectories)
                    : Directory.GetFiles(pathsAsFolder, "*", SearchOption.TopDirectoryOnly));
            else
                temp.Add(pathsAsFolder);

            return temp;
        }
    }
}