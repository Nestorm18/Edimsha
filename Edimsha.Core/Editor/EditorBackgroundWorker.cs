using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Edimsha.Core.Models;

namespace Edimsha.Core.Editor
{
    public class EditorBackgroundWorker : BackgroundWorker
    {
        private readonly ObservableCollection<string> _paths;
        // private readonly EditorConfig _configEditor;
        private readonly Resolution _resolution;

        // public EditorBackgroundWorker(ObservableCollection<string> paths, EditorConfig configEditor, Resolution resolution)
        // {
        //     _paths = paths;
        //     _configEditor = configEditor;
        //     _resolution = resolution;
        //
        //     WorkerSupportsCancellation = true;
        //     WorkerReportsProgress = true;
        //     DoWork += Worker_DoWork;
        // }

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

                // var edt = new Edition(path, _configEditor) {Resolution = _resolution};
                // edt.Run();

                ReportProgress(cnt, new MyUserState {CountPaths = _paths.Count});
                cnt++;
            });
        }
    }
    
    public class MyUserState
    {
        public double CountPaths { get; init; }
    }
}