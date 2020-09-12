using System.Collections.Generic;
using System.Windows;

namespace Edimsha
{
    /// <summary>
    /// Lógica de interacción para ResolutionDlg.xaml
    /// </summary>
    public partial class ResolutionDlg : Window
    {
        private StorageResolutions store = new StorageResolutions(FilePaths.RESOLUTIONS);
        
        public ResolutionDlg()
        {
            InitializeComponent();
            LoadResolutions();
        }

        private void LoadResolutions()
        {
            if (store.GetResolutions().Count > 0)
            {
                List<string> toCmb = new List<string>();
                
                foreach (var item in store.GetResolutions())            
                    toCmb.Add($"X: {item.Width}, Y: {item.Height}");
                
                cmbResolutions.ItemsSource = toCmb;
                cmbResolutions.SelectedIndex = toCmb.Count - 1;

            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (store.GetResolutions().Count > 0)
            {
                cmbResolutions.IsEnabled = true;
                btnRemove.IsEnabled = true;
                btnSaveResolution.IsEnabled = true;
                
                // TODO: fix Return null cuando elige index 0 
                string[] splt = cmbResolutions.SelectedItem.ToString().Split();
                string width = splt[1].Trim().Replace(",", "");
                string height = splt[3].Trim();

                txtWidth.Text = width;
                txtHeight.Text = height;
            }
            else
            {
                cmbResolutions.IsEnabled = false;
                btnRemove.IsEnabled = false;
                btnSaveResolution.IsEnabled = false;
            }
        }

        // Events     
        private void CmbResolutions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void Spinners_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (txtWidth.Value > 0 && txtHeight.Value > 0)            
                btnSaveResolution.IsEnabled = true;            
            else
                btnSaveResolution.IsEnabled = false;
        }

        private void BtnSaveResolution_Click(object sender, RoutedEventArgs e)
        {
            int width = (int)txtWidth.Value;
            int height = (int)txtHeight.Value;

            if (width > 0 && height > 0)
            {
                Resolution res = new Resolution
                {
                    Width = width,
                    Height = height
                };
                store.SaveResolution(res);
            }
            LoadResolutions();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            store.RemoveResolution(cmbResolutions.SelectedItem.ToString());
            LoadResolutions();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnLoadResolution_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Guarda resolucion en Settings y cargar en mainview
            //Settings.Default.Resolution = ;
        }

    }
}
