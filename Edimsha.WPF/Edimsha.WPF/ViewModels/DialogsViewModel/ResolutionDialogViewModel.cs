#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Dialogs;
using Edimsha.WPF.Services.Data;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.ViewModels.DialogsViewModel
{
    public class ResolutionDialogViewModel : ViewModelBase
    {
        #region IOC

        private readonly ILoadSettingsService _loadSettingsService;
        private readonly IOptions<ConfigPaths> _options;

        #endregion

        #region Fields

        /// <summary>
        /// Allows you to override the limitation of entering negative numbers to return a cancellation value or clear fields.
        /// </summary>
        public bool BypassWidthOrHeightLimitations;

        // Property to return as Resolution
        /// <summary>
        /// When the dialog closes, use this method to obtain a result.
        /// </summary>
        /// <returns>Null if the dialog is cancelled otherwise a <see cref="Resolution"/> object</returns>
        public Resolution? GetResolution()
        {
            // Canceled
            if (Width == -1 || Heigth == -1) return null;

            // Resolution to return
            return new Resolution(Width, Heigth);
        }

        #endregion

        #region Properties

        private bool _hasValidResolutions;
        private ObservableCollection<Resolution> _resolutions = null!;
        private int _cmbIndex;
        private int _width;
        private int _heigth;
        private string _errorMessage = null!;

        /// <summary>
        /// Used to have disabled fields.
        /// </summary>
        public bool HasValidResolutions
        {
            get => _hasValidResolutions;
            set
            {
                if (value == _hasValidResolutions) return;
                _hasValidResolutions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Save a observableCollection of <see cref="Resolution"/> to be displayed in a combobox.
        /// </summary>
        public ObservableCollection<Resolution> Resolutions
        {
            get => _resolutions;
            private init
            {
                if (Equals(value, _resolutions)) return;
                _resolutions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Used to select the zero value of the combobox when adding a new resolution.
        /// </summary>
        public int CmbIndex
        {
            get => _cmbIndex;
            set
            {
                if (value == _cmbIndex) return;
                _cmbIndex = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Image width (positive numbers only except for resets and dialog cancellations)
        /// </summary>
        public int Width
        {
            get => _width;
            set
            {
                // Allow to put 0 or -1 only if all resolutions are removed/cancel dialog
                if (!BypassWidthOrHeightLimitations)
                    if ((value == _width || value <= 0))
                        return;

                _width = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Image heigth (positive numbers only except for resets and dialog cancellations)
        /// </summary>
        public int Heigth
        {
            get => _heigth;
            set
            {
                // Allow to put 0 or -1 only if all resolutions are removed/cancel dialog
                if (!BypassWidthOrHeightLimitations)
                    if ((value == _heigth || value <= 0))
                        return;

                _heigth = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Useful in translations. You must first use a TranslationSource instance.
        /// <para></para>Example: <code>TranslationSource.GetTranslationFromString("your_string")</code> and pass the message.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        # region Commands

        /// <summary>
        /// New selected resolution updates width and height fields.
        /// </summary>
        public ICommand SelectionChangedCommand { get; }

        public ICommand SaveResolutionCommand { get; }

        public ICommand RemoveResolutionCommand { get; }

        /// <summary>
        /// Ignore any selected resolution.
        /// </summary>
        public ICommand CancelCommand { get; }

        public ICommand AcceptCommand { get; }

        #endregion

        # region Contructor

        public ResolutionDialogViewModel(
            ILoadSettingsService loadSettingsService,
            ISaveSettingsService saveSettingsService,
            IOptions<ConfigPaths> options)
        {
            Logger.Info("Constructor");

            _loadSettingsService = loadSettingsService;
            _options = options;

            Resolutions = new ObservableCollection<Resolution>();
            Resolutions.CollectionChanged += ResolutionsOnCollectionChanged;

            // Commands
            SelectionChangedCommand = new ParameterizedRelayCommand(ComboboxSelectionChangedEvent);
            SaveResolutionCommand = new SaveResolutionCommand(this, saveSettingsService, options);
            RemoveResolutionCommand = new RemoveResolutionCommand(this, saveSettingsService,options);
            CancelCommand = new QuitResolutionsCommand(this);
            AcceptCommand = new AcceptResolutionCommand();
            
            SetUserSettings();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Activates GUI components if we have available resolutions.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void ResolutionsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Logger.Info("Resolutions.Count > 0");
            if (Resolutions.Count > 0) HasValidResolutions = true;
        }

        /// <summary>
        /// Updates the width and height values by selecting them in the combobox
        /// </summary>
        /// <param name="parameter">A <see cref="Resolution"/>. must be object because it comes as a parameter from xaml and does not allow any other format.</param>
        private void ComboboxSelectionChangedEvent(object parameter)
        {
            Logger.Info($"parameter {parameter}");
            if (!(parameter is Resolution resolution)) return;

            Width = resolution.Width;
            Heigth = resolution.Height;
        }

        /// <summary>
        /// Load/initialize defaults at GUI startup.
        /// </summary>
        private void SetUserSettings()
        {
            Logger.Info("Loading settings");
            BypassWidthOrHeightLimitations = false;
            LoadResolutions();
        }

        /// <summary>
        /// Loads the resolutions of the saving file and adds them to the ObservableCollection Resolutions property.
        /// </summary>
        private void LoadResolutions()
        {
            Logger.Info("Loading resolutions");
            Resolutions.Clear();

            var resolutions = _loadSettingsService.LoadConfigurationSetting<List<Resolution>, EditorOptions>
                ("Resolutions", _options.Value.EditorOptions);

            foreach (var resolution in resolutions) Resolutions.Add(resolution);

            // Load first resolution values and ignore others. Used in combination with CmbIndex = 0.
            if (Resolutions.Count <= 0) return;

            Width = Resolutions[0].Width;
            Heigth = Resolutions[0].Height;
            CmbIndex = 0;
        }

        #endregion
    }
}