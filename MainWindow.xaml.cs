using Edimsha.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

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

            // TODO: TEMP CODE
            Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
            bool still = store.IsPathsStillAvailableFromLastSession(true);

            UpdateLvEditor(store);
        }

        #region Window
        // Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CleanListOnExit(chkCleanListOnExit.IsChecked);
        }

        // Logic   
        private void CleanListOnExit(bool? isChecked)
        {
            if (isChecked == true)
            {
                Storage store = new Storage(FilePaths.EDITOR_FILE_PATHS);
                store.CleanFile();
            }
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
            lvEditor.Items.Clear();

            foreach (var path in store.GetPaths())
                lvEditor.Items.Add(path);
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


        #endregion
    }
}
