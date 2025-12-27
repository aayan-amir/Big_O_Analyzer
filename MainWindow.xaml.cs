using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sem_Projec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CodeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Buttonn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeInput.Text))
            {
                OutputBox.Text = "❌ No code provided.";
                return;
            }
            else
            {
                string code = CodeInput.Text;
                string result = "";
                {
                    result += Analyzer.CountKeywords(code);
                    result += Analyzer.ShowStructure(code);
                    result += Analyzer.EstimateTimeComplexity(code);
                }

                OutputBox.Text = result;
            }
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Dark Theme
            Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Resources["TextBoxBackground"] = new SolidColorBrush(Color.FromRgb(45, 45, 48));
            Resources["TextBoxForeground"] = Brushes.White;
            Resources["TextBoxBorder"] = new SolidColorBrush(Color.FromRgb(62, 62, 66));
            Resources["ButtonBackground"] = new SolidColorBrush(Color.FromRgb(0, 122, 204));
            Resources["ButtonForeground"] = Brushes.White;
            Resources["TitleForeground"] = Brushes.White;
            Resources["ToggleBackground"] = new SolidColorBrush(Color.FromRgb(62, 62, 66));
            if (ThemeToggle != null) ThemeToggle.Content = "Light Mode";
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(240, 242, 245));
            Resources["TextBoxBackground"] = Brushes.White;
            Resources["TextBoxForeground"] = Brushes.Black;
            Resources["TextBoxBorder"] = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            Resources["ButtonBackground"] = new SolidColorBrush(Color.FromRgb(0, 120, 215));
            Resources["ButtonForeground"] = Brushes.White;
            Resources["TitleForeground"] = new SolidColorBrush(Color.FromRgb(17, 17, 17));
            Resources["ToggleBackground"] = new SolidColorBrush(Color.FromRgb(0, 120, 215));
            if (ThemeToggle != null) ThemeToggle.Content = "Dark Mode";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "C# files (*.cs)|*.cs|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = ".cs"
                };

                bool? ok = dlg.ShowDialog(this);
                if (ok != true)
                {
                    return;
                }

                File.WriteAllText(dlg.FileName, CodeInput.Text ?? string.Empty, Encoding.UTF8);
                MessageBox.Show(this, $"Saved to:\n{dlg.FileName}", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Save failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "C# files (*.cs)|*.cs|Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };

                bool? ok = dlg.ShowDialog(this);
                if (ok != true)
                {
                    // user cancelled
                    return;
                }

                CodeInput.Text = File.ReadAllText(dlg.FileName, Encoding.UTF8);
                MessageBox.Show(this, $"Loaded:\n{dlg.FileName}", "Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Load failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
                CodeInput.Text = "// Sample: nested loops\nfor (int i = 0; i < n; i++)\n{\n    for (int j = 0; j < n; j++)\n    {\n        // do work\n    }\n}\n";
        }
    }
}