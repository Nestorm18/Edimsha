using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Edimsha.WPF.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        public async Task<List<string>> OpenFileSelector(string title, string filter, bool multiselect)
        {
            var dlg = new OpenFileDialog()
            {
                Title = title,
                Filter = filter,
                Multiselect = multiselect
            };

            return dlg.ShowDialog() == true ? new List<string>(dlg.FileNames) : null;
        }
    }
}