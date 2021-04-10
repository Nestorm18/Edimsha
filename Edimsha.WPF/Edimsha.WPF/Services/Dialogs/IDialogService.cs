using System.Collections.Generic;
using System.Threading.Tasks;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;

namespace Edimsha.WPF.Services.Dialogs
{
    public interface IDialogService
    {
        Task<List<string>> OpenFileSelector(string title, string filter, bool multiselect);
        
        Task<string> OpenFolderSelector(string title);
        
        Task<Resolution> OpenResolutionDialog(ILoadSettingsService loadSettingsService, ISaveSettingsService saveSettingsService);
    }
}