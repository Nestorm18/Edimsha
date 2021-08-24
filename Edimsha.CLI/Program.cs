using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using Edimsha.Core.Editor;
using Edimsha.Core.Models;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming

namespace Edimsha.CLI
{
    class Program
    {
        private static CancellationTokenSource _token;

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
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
                case ViewType.Converter:
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
            var config = new EditorOptions();

            config.IterateSubdirectories = opts.IterateSubdirectories;
            config.OutputFolder = opts.OutputFolder;
            config.Edimsha = opts.Edimsha;
            config.AlwaysIncludeOnReplace = opts.AlwaysIncludeOnReplace;
            config.KeepOriginalResolution = opts.KeepOriginalResolution;
            config.CompresionValue = opts.CompresionValue;
            config.OptimizeImage = opts.OptimizeImage;
            config.ReplaceForOriginal = opts.ReplaceForOriginal;
            config.Resolution = new Resolution(opts.Width, opts.Height);
            config.Paths = opts.Paths.ToList();
            var pathsAsFolder = opts.PathsAsFolder;

            if (config.Paths.Count == 0 && string.IsNullOrEmpty(pathsAsFolder))
                Console.WriteLine("[ERROR] Must use --paths or --pathsAsFolder to set list of images or folder that contains");

            // Bad resolution or not used but not wanted the original values cannot be done
            if (!config.Resolution.IsValid() && !opts.KeepOriginalResolution)
                Console.WriteLine("[ERROR] The resolution value in -w/--width or -h/--height is not valid.");

            // Find all images in folder if path list is zero using pathsAsFolder otherwise Paths have priority always.
            if (config.Paths.Count == 0)
                config.Paths = FindAllImagesInPaths(pathsAsFolder, config);

            var editor = new Editor(config);
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += ProgressOnProgressChanged;
            _token = new CancellationTokenSource();

            editor.ExecuteProcessing(progress, _token);
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

        private static void RunConversorModeInCLI(Options opts)
        {
            throw new NotImplementedException();
        }

        private static List<string> FindAllImagesInPaths(string pathsAsFolder, EditorOptions config)
        {
            var temp = new List<string>();

            if (Directory.Exists(pathsAsFolder))
                temp.AddRange(config.IterateSubdirectories
                    ? Directory.GetFiles(pathsAsFolder, "*", SearchOption.AllDirectories)
                    : Directory.GetFiles(pathsAsFolder, "*", SearchOption.TopDirectoryOnly));
            else
                temp.Add(pathsAsFolder);

            return temp;
        }
    }
}