using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        public async Task<string> OpenFolderSelector(string title)
        {
            var dlg = new CommonOpenFileDialog {IsFolderPicker = true, Title = title};
            
            return dlg.ShowDialog() == CommonFileDialogResult.Ok ? dlg.FileName : null;
        }
    }
}