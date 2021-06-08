using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Edimsha.WPF.Models;
using Edimsha.WPF.Settings;

namespace Edimsha.WPF.Services.Editor
{
    public class EditorBackgroundWorker : BackgroundWorker
    {
        private readonly ObservableCollection<string> _paths;
        private readonly Config _config;
        private readonly Resolution _resolution;

        public EditorBackgroundWorker(ObservableCollection<string> paths, Config config, Resolution resolution)
        {
            _paths = paths;
            _config = config;
            _resolution = resolution;

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

                var edt = new Edition(path, _config) {Resolution = _resolution};
                edt.Run();

                ReportProgress(cnt, new MyUserState {CountPaths = _paths.Count});
                cnt++;
            });
        }
    }

    internal class MyUserState
    {
        public double CountPaths { get; init; }
    }
}