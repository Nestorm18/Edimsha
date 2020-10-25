using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Edimsha.Properties;
using Edimsha.Storage;

namespace Edimsha.Edition.Editor
{
    internal class EditorBackgroundWorker : BackgroundWorker
    {
        private readonly List<string> allPaths;

        private readonly StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);

        public EditorBackgroundWorker()
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            DoWork += Worker_DoWork;

            allPaths = store.GetObject<string>();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var cnt = 1;
            Parallel.ForEach(allPaths, path =>
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var edt = new Edition(path)
                {
                    OutputFolder = Settings.Default.txtEditorFolderPath,
                    Edimsha = Settings.Default.txtEdimsha,
                    AddOnReplace = Settings.Default.chkAddOnReplace,
                    Width = int.Parse(Settings.Default.Width),
                    Height = int.Parse(Settings.Default.Height),
                    OriginalResolution = Settings.Default.chkKeepOriginalResolution,
                    CompressionValue = Settings.Default.sldCompression,
                    OptimizeImage = Settings.Default.chkOptimizeImage,
                    ReplaceOriginal = Settings.Default.chkReplaceForOriginal
                };
                edt.Run();

                ReportProgress(cnt, new MyUserState {CountPaths = allPaths.Count, PathName = path});
                cnt++;
            });
        }
    }

    internal class MyUserState
    {
        public double CountPaths { get; set; }
        public string PathName { get; set; }
    }
}