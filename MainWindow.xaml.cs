using Edimsha.Dialogs;
using Edimsha.Edition;
using Edimsha.Edition.Editor;
using Edimsha.Properties;
using Edimsha.Storage;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Edimsha
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Menubar
        private EditionMode currentMode = EditionMode.Editor;
        private EditorBackgroundWorker bw;

        public MainWindow()
        {
            //Settings.Default.Reset(); // Used to find bugs with defaults values in the app.

            //LoadLanguage();

            InitializeComponent();

            LoadSettings();
        }

        #region Window
        // Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CleanEditorListOnExit(chkCleanListOnExit.IsChecked);
            SaveEditorSliderCompression();
        }

        // Logic   
        private void CleanEditorListOnExit(bool? isChecked)
        {
            if (isChecked == true)
            {
                StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
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
            if (txtEdimsha.Text.Equals(""))
                Settings.Default.txtEdimsha = "edimsha_";
            else
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
        #region Events
        // ListView
        private void LvEditorDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<string> paths = ExtractDroppedPaths(items);

                StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS)
                {
                    KeepSavedPreviousPaths = true
                };

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

                StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
                store.RemovePath(item);

                UpdateLvEditor(store);
            }
        }

        private void CtxLvDeleteAll(object sender, RoutedEventArgs e)
        {
            DeleteJsonEditor();
        }

        // Checkbox chkCleanListOnExit
        private void ChkCleanListOnExit_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.chkCleanListOnExit = (chkCleanListOnExit.IsChecked == true);
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

                StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS)
                {
                    KeepSavedPreviousPaths = true
                };

                store.SavePaths(paths);

                UpdateLvEditor(store);
            }
        }

        // Textfield TxtOutputFolder
        private void TxtOutputFolder_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveEditorOutputFolderPath();
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

        // Textfield TxtEdimsha
        private void TxtEdimsha_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveEditorEdimsha();
        }

        // Checkbox AddOnReplace
        private void ChkAddOnReplace_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.chkAddOnReplace = (chkAddOnReplace.IsChecked == true);
            Settings.Default.Save();
        }

        // Checkbox KeepOriginalResolution 
        private void ChkKeepOriginalResolution_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.chkKeepOriginalResolution = (chkKeepOriginalResolution.IsChecked == true);
            Settings.Default.Save();
        }

        // Button open resolution selection/save
        private void BtnUsedResolutions_Click(object sender, RoutedEventArgs e)
        {
            ResolutionDlg dlg = new ResolutionDlg();
            dlg.ShowDialog();

            LoadWidthAndHeigth();
        }

        // Slider Compression
        private void SldCompression_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (statusbar != null)
                statusbar.Text = $"Dejar sin comprimir un {(int)sldCompression.Value}%";
        }

        // Checkbox ChkOptimizeImage
        private void ChkOptimizeImage_Click(object sender, RoutedEventArgs e)
        {
            sldCompression.IsEnabled = (chkOptimizeImage.IsChecked == true);
            Settings.Default.chkOptimizeImage = (chkOptimizeImage.IsChecked == true);
            Settings.Default.Save();
        }

        // Checkbox ChkReplaceForOriginal
        private void ChkReplaceForOriginal_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.chkReplaceForOriginal = (chkReplaceForOriginal.IsChecked == true);
            Settings.Default.Save();
        }

        // Button Reset
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            DeleteJsonEditor();
            Settings.Default.Reset();
            lvEditor.ItemsSource = new List<string>();
            chkCleanListOnExit.IsChecked = false;
            txtOutputFolder.Text = "";
            txtEdimsha.Text = "";
            chkAddOnReplace.IsChecked = false;
            txtWidth.Text = Settings.Default.Width;
            txtHeight.Text = Settings.Default.Height;
            chkKeepOriginalResolution.IsChecked = false;
            sldCompression.Value = 80;
            chkOptimizeImage.IsChecked = false;
            chkReplaceForOriginal.IsChecked = false;
        }

        // Button Stop
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            bw.CancelAsync();
            EnableEditorUI();
        }

        // Button Start
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            DisableEditorUI();

            bw = null;
            bw = new EditorBackgroundWorker();
            bw.ProgressChanged += Worker_ProgressChanged;
            bw.RunWorkerCompleted += Worker_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        #endregion

        #region Logic
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

        private void UpdateLvEditor(StoragePaths store)
        {
            List<string> src = store.GetObject<string>();
            lvEditor.ItemsSource = src;

            if (src.Count == 0)
            {
                pbEditor.Maximum = 1;
                btnStart.IsEnabled = false;
            }
            else
            {
                pbEditor.Maximum = src.Count;
                btnStart.IsEnabled = true;
            }

            UpdateCtxLvEditor();
        }

        private void EnableEditorUI()
        {
            menubar.IsEnabled = true;
            lvEditor.IsEnabled = true;
            chkCleanListOnExit.IsEnabled = true;
            btnSelectEditorImages.IsEnabled = true;
            txtOutputFolder.IsEnabled = true;
            btnSelectEditorOutputFolder.IsEnabled = true;
            txtEdimsha.IsEnabled = true;
            chkAddOnReplace.IsEnabled = true;
            chkKeepOriginalResolution.IsEnabled = true;
            btnUsedResolutions.IsEnabled = true;
            sldCompression.IsEnabled = true;
            chkOptimizeImage.IsEnabled = true;
            chkReplaceForOriginal.IsEnabled = true;
            btnReset.IsEnabled = true;
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
        }

        private void DisableEditorUI()
        {
            menubar.IsEnabled = false;
            lvEditor.IsEnabled = false;
            chkCleanListOnExit.IsEnabled = false;
            btnSelectEditorImages.IsEnabled = false;
            txtOutputFolder.IsEnabled = false;
            btnSelectEditorOutputFolder.IsEnabled = false;
            txtEdimsha.IsEnabled = false;
            chkAddOnReplace.IsEnabled = false;
            chkKeepOriginalResolution.IsEnabled = false;
            btnUsedResolutions.IsEnabled = false;
            sldCompression.IsEnabled = false;
            chkOptimizeImage.IsEnabled = false;
            chkReplaceForOriginal.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;
        }

        // BackgroundWorker
        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbEditor.Value = e.ProgressPercentage;
            statusbar.Text = $"Editada {e.ProgressPercentage} de {e.UserState}";
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                statusbar.Text = "Cancelled by user...";
            else
            {
                bw.ProgressChanged -= Worker_ProgressChanged;
                bw.RunWorkerCompleted -= Worker_RunWorkerCompleted;
                EnableEditorUI();
            }
        }

        #endregion
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
            StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
            bool still = store.StillPathsSameFromLastSession();

            if (!still) LaunchPathChangeMsg(store);

            UpdateLvEditor(store);
            UpdateCtxLvEditor();

            chkCleanListOnExit.IsChecked = Settings.Default.chkCleanListOnExit;
            txtOutputFolder.Text = Settings.Default.txtEditorFolderPath;

            if (Settings.Default.txtEdimsha.Equals("") || Settings.Default.txtEdimsha.Equals("edimsha_"))
                txtEdimsha.Text = "";
            else
                txtEdimsha.Text = Settings.Default.txtEdimsha;

            chkAddOnReplace.IsChecked = Settings.Default.chkAddOnReplace;
            LoadWidthAndHeigth();
            chkKeepOriginalResolution.IsChecked = Settings.Default.chkKeepOriginalResolution;
            sldCompression.Value = Settings.Default.sldCompression;
            sldCompression.IsEnabled = Settings.Default.chkOptimizeImage;
            chkOptimizeImage.IsChecked = Settings.Default.chkOptimizeImage;
            chkReplaceForOriginal.IsChecked = Settings.Default.chkReplaceForOriginal;

            statusbar.Text = "Aplicacion iniciada";
        }

        private void LoadWidthAndHeigth()
        {
            txtWidth.Text = Settings.Default.Width;
            txtHeight.Text = Settings.Default.Height;
        }

        private void UpdateCtxLvEditor()
        {
            ctxLvRemove.IsEnabled = (lvEditor.SelectedItems.Count > 0);
            ctxLvRemoveAll.IsEnabled = (lvEditor.Items.Count > 0);
        }

        private void LaunchPathChangeMsg(StoragePaths store)
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

        private void DeleteJsonEditor()
        {
            StoragePaths store = new StoragePaths(FilePaths.EDITOR_FILE_PATHS);
            store.CleanFile();

            UpdateLvEditor(store);
        }

        #endregion
    }
}
