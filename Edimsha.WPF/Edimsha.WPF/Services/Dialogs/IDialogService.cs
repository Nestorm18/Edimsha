using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edimsha.WPF.Services.Dialogs
{
    public interface IDialogService
    {
        Task<List<string>> OpenFileSelector(string title, string filter, bool multiselect);
        
        Task<string> OpenFolderSelector(string title);
    }
}