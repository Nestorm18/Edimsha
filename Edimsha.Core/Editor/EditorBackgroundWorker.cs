﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Edimsha.Core.Models;

namespace Edimsha.Core.Editor
{
    public class EditorBackgroundWorker : BackgroundWorker
    {
        private readonly ObservableCollection<string> _paths;
        private readonly EditorConfig _editorConfig;

        public EditorBackgroundWorker(ObservableCollection<string> paths, EditorConfig editorConfig)
        {
            _paths = paths;
            _editorConfig = editorConfig;
        
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
            DoWork += Worker_DoWork;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var cnt = 1;

            Parallel.ForEach(_paths, path =>
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var edt = new Edition(path, _editorConfig);
                edt.Run();

                ReportProgress(cnt, new EditorPathState {CountPaths = _paths.Count});
                cnt++;
            });
        }
    }
    
    public class EditorPathState
    {
        public double CountPaths { get; init; }
    }
}