using System;
using System.Collections.Generic;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Main;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels.Factories;
using Microsoft.Extensions.Options;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Edimsha.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // IOC
        private readonly IEdimshaViewModelFactory _viewModelFactory;
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly IOptions<ConfigPaths> _options;

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
            ILoadSettingsService loadSettingsService,
            IOptions<ConfigPaths> options)
        {
            Logger.Info("Constructor");

            _viewModelFactory = viewModelFactory;
            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;
            _options = options;

            CurrentModeViewModel = _viewModelFactory.CreateViewModel(Mode);

            // Commands
            // Window event
            WindowCloseCommand = new RelayCommand(WindowClose);

            // Menu
            QuitCommand = new QuitCommand();
            ChangeLanguageCommand = new ChangeLanguageCommand(this, _saveSettingsService, _options);
            ChangeModeCommand = new ChangeModeCommand(this, _viewModelFactory);

            LoadLanguageFromSettings();
        }

        private void WindowClose()
        {
            Logger.Info("Closed event");
            try
            {
                switch (Mode)
                {
                    case ViewType.Editor:
                    {
                        var cleanListOnExit = ((EditorViewModel) CurrentModeViewModel).CleanListOnExit;
                        if (cleanListOnExit)
                            _saveSettingsService.SaveConfigurationSettings<List<string>, ConversorOptions>("Paths", new List<string>(), _options.Value.EditorOptions);
                        break;
                    }
                    case ViewType.Converter:
                    {
                        var cleanListOnExit = ((ConversorViewModel) CurrentModeViewModel).CleanListOnExit;
                        if (cleanListOnExit)
                            _saveSettingsService.SaveConfigurationSettings<List<string>, ConversorOptions>("Paths", new List<string>(), _options.Value.ConversorOptions);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
            }
        }

        private void LoadLanguageFromSettings()
        {
            var lang = _loadSettingsService.LoadConfigurationSetting<string, EditorOptions>("Language", _options.Value.EditorOptions);

            Language = ChangeLanguage.ResolveLanguage(lang);

            ChangeLanguage.SetLanguage(lang);

            Logger.Info($"Language changed to: {lang}");
        }
    }
}