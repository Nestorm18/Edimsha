﻿using Edimsha.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Edimsha
{
    internal class CustomBackgroundWorker : BackgroundWorker
    {

        private readonly StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
        private readonly List<string> allPaths;

        public CustomBackgroundWorker()
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            DoWork += Worker_DoWork;

            allPaths = store.GetObject<string>();
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int cnt = 1;
            Parallel.ForEach(allPaths, path =>
            {
                if (CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                Edition edt = new Edition(path)
                {
                    OutputFolder = Settings.Default.txtEditorFolderPath,
                    Edimsha = Settings.Default.txtEdimsha,
                    AddOnReplace = Settings.Default.chkAddOnReplace,
                    Width = int.Parse(Settings.Default.Width),
                    Height = int.Parse(Settings.Default.Height),
                    OriginalResolution = Settings.Default.chkKeepOriginalResolution,
                    OptimizeImage = Settings.Default.chkOptimizeImage,
                    ReplaceOriginal = Settings.Default.chkReplaceForOriginal
                };
                edt.Run();

                ReportProgress(cnt, allPaths.Count);
                cnt++;
            });
        }
    }
}