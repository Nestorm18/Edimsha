using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Edimsha.Edition;
using Edimsha.Edition.Editor;
using Edimsha.Properties;
using Edimsha.Storage;

namespace Edimsha.Dialogs
{
    /// <summary>
    ///     Lógica de interacción para ResolutionDlg.xaml
    /// </summary>
    public partial class ResolutionDlg : Window
    {
        private readonly StorageResolutions _store = new StorageResolutions(FilePaths.RESOLUTIONS);

        public ResolutionDlg()
        {
            InitializeComponent();
            LoadResolutions();
        }

        private void LoadResolutions()
        {
            if (_store.GetResolutions().Count > 0)
            {
                var toCmb = new List<string>();

                foreach (var item in _store.GetResolutions())
                    toCmb.Add($"X: {item.Width}, Y: {item.Height}");

                CmbResolutions.ItemsSource = toCmb;
                CmbResolutions.SelectedIndex = toCmb.Count - 1;
            }
            else
            {
                CmbResolutions.ItemsSource = new List<string>();
            }

            UpdateUi();
        }

        private void UpdateUi()
        {
            if (_store.GetResolutions().Count > 0)
            {
                CmbResolutions.IsEnabled = true;
                BtnRemove.IsEnabled = true;
                BtnSaveResolution.IsEnabled = true;
                BtnLoadResolution.IsEnabled = true;

                if (CmbResolutions.SelectedItem != null)
                {
                    var splt = CmbResolutions.SelectedItem.ToString().Split();

                    var width = splt[1].Trim().Replace(",", "");
                    var height = splt[3].Trim();

                    TxtWidth.Text = width;
                    TxtHeight.Text = height;
                }
            }
            else
            {
                CmbResolutions.IsEnabled = false;
                BtnRemove.IsEnabled = false;
                BtnSaveResolution.IsEnabled = false;
                BtnLoadResolution.IsEnabled = false;

                TxtWidth.Text = "";
                TxtHeight.Text = "";
            }
        }

        // Events     
        private void CmbResolutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private void Spinners_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TxtWidth.Value > 0 && TxtHeight.Value > 0)
                BtnSaveResolution.IsEnabled = true;
            else
                BtnSaveResolution.IsEnabled = false;
        }

        private void BtnSaveResolution_Click(object sender, RoutedEventArgs e)
        {
            if (TxtWidth.Value != null && TxtHeight.Value != null)
            {
                var width = (int) TxtWidth.Value;
                var height = (int) TxtHeight.Value;

                if (width > 0 && height > 0)
                {
                    var res = new Resolution
                    {
                        Width = width,
                        Height = height
                    };

                    MsgResolutionExist(_store.SaveResolution(res));
                }
            }

            LoadResolutions();
        }

        private void MsgResolutionExist(bool exist)
        {
            if (exist)
                MessageBox.Show("Esa resolucion ya existe.", "Resolucion existente", MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            _store.RemoveResolution(CmbResolutions.SelectedItem.ToString());
            LoadResolutions();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnLoadResolution_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Width = TxtWidth.Text;
            Settings.Default.Height = TxtHeight.Text;
            Close();
        }
    }
}