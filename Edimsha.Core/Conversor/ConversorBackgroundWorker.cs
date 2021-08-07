using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Edimsha.Core.Models;

namespace Edimsha.Core.Conversor
{
    public class ConversorBackgroundWorker : BackgroundWorker
    {
        private readonly ObservableCollection<string> _paths;
        private readonly ConversorConfig _conversorConfig;
        private readonly ImageTypesConversor _format;

        public ConversorBackgroundWorker(ObservableCollection<string> paths, ConversorConfig conversorConfig, ImageTypesConversor format)
        {
            _paths = paths;
            _conversorConfig = conversorConfig;
            _format = format;

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

                var conversion = new Conversion(path, _conversorConfig, _format);
                conversion.Run();

                ReportProgress(cnt, new ConversorPathState {CountPaths = _paths.Count});
                cnt++;
            });
        }
    }

    public class ConversorPathState
    {
        public double CountPaths { get; init; }
    }
}