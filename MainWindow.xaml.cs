using Edimsha.Properties;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Edimsha
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Menubar
        private EditionMode currentMode = EditionMode.Editor;

        public MainWindow()
        {
            //Settings.Default.Reset(); // Used to find bugs with defaults values in the app.

            //LoadLanguage();

            InitializeComponent();

            LoadSettings();

            // TEMP
            //ResolutionDlg dlg = new ResolutionDlg();
            //dlg.ShowDialog();
        }

        #region Window
        // Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CleanEditorListOnExit(chkCleanListOnExit.IsChecked);
            SaveEditorOutputFolderPath();
            SaveEditorEdimsha();
            SaveEditorSliderCompression();

        }

        // Logic   
        private void CleanEditorListOnExit(bool? isChecked)
        {
            if (isChecked == true)
            {
                Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
                store.CleanFile();
            }
        }

        private void SaveEditorOutputFolderPath()
        {
            string path = txtOutputFolder.Text;

            if (Directory.Exists(path))
                Settings.Default.txtEditorFolderPath = txtOutputFolder.Text;
            else // Empty path            
                Settings.Default.txtEditorFolderPath = "";

            Settings.Default.Save();
        }
       
        private void SaveEditorEdimsha()
        {
            Settings.Default.txtEdimsha = txtEdimsha.Text;
            Settings.Default.Save();
        }

        private void SaveEditorSliderCompression()
        {
            Settings.Default.sldCompression = (int)sldCompression.Value;
            Settings.Default.Save();
        }

        #endregion

        #region Menubar

        #region Menu
        // Events
        private void SearchForUpdates(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("SearchForUpdates...");
        }

        private void SendErrorLogs(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("SendErrorLogs");
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ExitApp... Doing stuff before close");
            Close();
        }

        // Logic
        #endregion

        #region Mode
        // Events
        private void ChangeToEditor(object sender, RoutedEventArgs e)
        {
            UpdateMode(EditionMode.Editor);
        }

        private void ChangeToConversor(object sender, RoutedEventArgs e)
        {
            UpdateMode(EditionMode.Conversor);
        }

        // Logic
        private void UpdateMode(EditionMode mode)
        {
            switch (mode)
            {
                case EditionMode.Editor:
                    currentMode = EditionMode.Editor;
                    break;
                case EditionMode.Conversor:
                    currentMode = EditionMode.Conversor;
                    break;
            }
            ChangeModeUI();
        }

        private void ChangeModeUI()
        {
            switch (currentMode)
            {
                case EditionMode.Editor:
                    miEditor.IsChecked = true;
                    miConversor.IsChecked = false;

                    imageEditor.Visibility = Visibility.Visible;
                    imageConversor.Visibility = Visibility.Collapsed;
                    break;
                case EditionMode.Conversor:
                    miEditor.IsChecked = false;
                    miConversor.IsChecked = true;

                    imageEditor.Visibility = Visibility.Collapsed;
                    imageConversor.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region Languaje
        // Events
        private void ChangeToEnglish(object sender, RoutedEventArgs e)
        {
            Settings.Default.Lang = "en_En";
            SaveAppLang();
        }

        private void ChangeToSpanish(object sender, RoutedEventArgs e)
        {
            Settings.Default.Lang = "es_Es";
            SaveAppLang();
        }
        // Logic

        /// <summary>
        /// Save the new language and restart the application.
        /// </summary>
        private static void SaveAppLang()
        {
            Settings.Default.Save();
            RestartApp();
        }

        /// <summary>
        /// Restart this application.
        /// </summary>
        private static void RestartApp()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region StackPanel Editor
        // Events
        // ListView
        private void LvEditorDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<string> paths = ExtractDroppedPaths(items);

                Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
                store.KeepSavedPreviousPaths = true;

                store.SavePaths(paths);

                UpdateLvEditor(store);
            }
        }

        private void LvEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCtxLvEditor();
        }

        private void CtxLvDelete(object sender, RoutedEventArgs e)
        {
            if (lvEditor.SelectedItems.Count > 0)
            {
                string item = (string)lvEditor.SelectedItems[0];

                Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
                store.RemovePath(item);

                UpdateLvEditor(store);
            }
        }

        private void CtxLvDeleteAll(object sender, RoutedEventArgs e)
        {
            Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
            store.CleanFile();

            UpdateLvEditor(store);
        }

        // Checkbox chkCleanListOnExit
        private void ChkCleanListOnExit_Click(object sender, RoutedEventArgs e)
        {
            if (chkCleanListOnExit.IsChecked == true)
                Settings.Default.chkCleanListOnExit = true;
            else
                Settings.Default.chkCleanListOnExit = false;

            Settings.Default.Save();
        }

        // Button open file selector
        private void BtnSelectEditorImages(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                List<string> paths = new List<string>(openFileDialog.FileNames);

                Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS)
                {
                    KeepSavedPreviousPaths = true
                };

                store.SavePaths(paths);

                UpdateLvEditor(store);
            }
        }

        // Button open folder selector
        private void BtnSelectEditorOutputFolder(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (openFolderDialog.ShowDialog() == true)
                txtOutputFolder.Text = openFolderDialog.SelectedPath;
        }

        // Checkbox AddOnReplace
        private void ChkAddOnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (chkAddOnReplace.IsChecked == true)
                Settings.Default.chkAddOnReplace = true;
            else
                Settings.Default.chkAddOnReplace = false;

            Settings.Default.Save();
        }

        // Checkbox KeepOriginalResolution 
        private void ChkKeepOriginalResolution_Click(object sender, RoutedEventArgs e)
        {
            if (chkKeepOriginalResolution.IsChecked == true)
                Settings.Default.chkKeepOriginalResolution = true;
            else
                Settings.Default.chkKeepOriginalResolution = false;

            Settings.Default.Save();
        }

        // Button open resolution selection/save
        private void BtnUsedResolutions_Click(object sender, RoutedEventArgs e)
        {
            ResolutionDlg dlg = new ResolutionDlg();
            dlg.ShowDialog();
        }

        // Slider Compression

        // Logic
        private List<string> ExtractDroppedPaths(string[] items)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            List<string> pathsJson = new List<string>();

            foreach (var item in items)
            {
                if (Path.HasExtension(item)) // It is a file, get path
                    pathsJson.Add(item);
                else
                    foreach (string dirFile in Directory.GetFiles(item)) // It is a folder, extract paths
                        pathsJson.Add(dirFile);
            }

            return pathsJson;

        }

        private void UpdateLvEditor(Storage store)
        {
            lvEditor.ItemsSource = store.GetPaths();
            UpdateCtxLvEditor();
        }

        #endregion

        #region StackPanel Conversor
        // Events
        // Logic
        #endregion

        #region General

        /// <summary>
        /// Load the last selected language.
        /// </summary>
        private void LoadLanguage()
        {
            string lang = Settings.Default.Lang.ToString();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        private void LoadSettings()
        {            
            // Listview
            Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
            bool still = store.StillPathsSameFromLastSession();

            if (!still) LaunchPathChangeMsg(store);

            UpdateLvEditor(store);
            UpdateCtxLvEditor();

            // chkCleanListOnExit
            chkCleanListOnExit.IsChecked = Settings.Default.chkCleanListOnExit;

            // txtOutputFolder
            txtOutputFolder.Text = Settings.Default.txtEditorFolderPath;

            // txtEdimsha
            txtEdimsha.Text = Settings.Default.txtEdimsha;

            // Checkbox chkAddOnReplace
            chkAddOnReplace.IsChecked = Settings.Default.chkAddOnReplace;

            // Checkbox chkKeepOriginalResolution
            chkKeepOriginalResolution.IsChecked = Settings.Default.chkKeepOriginalResolution;

            // Slider sldCompression
            sldCompression.Value = Settings.Default.sldCompression;
        }

        private void UpdateCtxLvEditor()
        {
            if (lvEditor.SelectedItems.Count > 0)
                ctxLvRemove.IsEnabled = true;
            else
                ctxLvRemove.IsEnabled = false;

            if (lvEditor.Items.Count > 0)
                ctxLvRemoveAll.IsEnabled = true;
            else
                ctxLvRemoveAll.IsEnabled = false;
        }

        private void LaunchPathChangeMsg(Storage store)
        {
            MessageBoxResult result = MessageBox.Show("Las rutas que estaban anteriormente se han modificado, Pulsa \"Si\" para ver los cambios.",
                "Rutas modificadas",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                string append = "";

                List<string> paths = store.GetPathChanges();
                foreach (var text in paths)
                    append += text + "\n\n";

                MessageBox.Show(append, "Rutas eliminadas", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            store.RemoveMissingPathsFromLastSession();
        }
        #endregion

  
    }
}
