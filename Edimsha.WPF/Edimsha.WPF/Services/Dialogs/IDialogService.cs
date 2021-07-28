using System.Collections.Generic;
using System.Threading.Tasks;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Services.Data;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.Services.Dialogs
{
    public interface IDialogService
    {
        Task<List<string>> OpenFileSelector(string title, string filter, bool multiselect);

        Task<string> OpenFolderSelector(string title);

        Task<Resolution> OpenResolutionDialog(ILoadSettingsService loadSettingsService, ISaveSettingsService saveSettingsService, IOptions<ConfigPaths> options);

        Task PathsRemovedLastSession(ILoadSettingsService loadSettingsService, ISaveSettingsService saveSettingsService, string filePath);
    }
}