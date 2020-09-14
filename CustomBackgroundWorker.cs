using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Edimsha
{
    internal class CustomBackgroundWorker : BackgroundWorker
    {

        private StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
        private List<string> allPaths;

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

            foreach (string path in allPaths)
            {
                if (CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                Edition edt = new Edition(path);
                edt.Run();
                
                ReportProgress(cnt, allPaths.Count);
                cnt++;
            }
        }
    }
}