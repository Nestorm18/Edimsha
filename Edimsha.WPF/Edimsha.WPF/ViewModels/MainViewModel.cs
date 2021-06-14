using System;
using System.Collections.Generic;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.Factories;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Edimsha.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // IOC
        private readonly IEdimshaViewModelFactory _viewModelFactory;
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly ILoadSettingsService _loadSettingsService;

        // Properties
        private ViewModelBase _currentModeViewModel;
        private Languages _language = Languages.Spanish;
        private ViewType _mode = ViewType.Editor;

        public Languages Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        public ViewType Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand QuitCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ChangeModeCommand { get; }

        public ICommand WindowCloseCommand { get; }

        // Viewmodel
        public ViewModelBase CurrentModeViewModel
        {
            get => _currentModeViewModel;
            set
            {
                _currentModeViewModel = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public MainViewModel(
            IEdimshaViewModelFactory viewModelFactory,
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService)
        {
            Logger.Log("Constructor");
            _viewModelFactory = viewModelFactory;
            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;
            CurrentModeViewModel = _viewModelFactory.CreateViewModel(Mode);

            // Commands
            // Window event
            WindowCloseCommand = new RelayCommand(WindowClose);

            // Menu
            QuitCommand = new QuitCommand();
            ChangeLanguageCommand = new ChangeLanguageCommand(this, _saveSettingsService);
            ChangeModeCommand = new ChangeModeCommand(this, _viewModelFactory);

            LoadLanguageFromSettings();
        }

        private void WindowClose()
        {
            Logger.Log($"Closed event");

            try
            {
                var cleanListOnExit = ((EditorViewModel) CurrentModeViewModel).CleanListOnExit;

                if (cleanListOnExit) _saveSettingsService.SavePathsListview(new List<string>(), ViewType.Editor);
            }
            catch (Exception e)
            {
                Logger.Log("Mode was conversor when close");
            }
        }

        private void LoadLanguageFromSettings()
        {
            var lang = _loadSettingsService.LoadConfigurationSetting<string>(ViewType.Editor, "Language");
            Language = ChangeLanguage.ResolveLanguage(lang);

            ChangeLanguage.SetLanguage(lang);

            Logger.Log($"Language changed to: {lang}");
        }
    }
}