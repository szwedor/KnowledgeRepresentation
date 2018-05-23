namespace Stories.View
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Microsoft.Win32;
    using Stories.Parser.Parsers;
    using Stories.ViewModel;

    public partial class StoryInputView : UserControl
    {
        private string currentFileName;
        private bool addedHighlighting;
        public StoryInputView()
        {
            this.InitializeComponent();

            this.textEditor.Text = string.Join("\r\n", @"
            initially not loaded
            initially alive
            load causes loaded
            shoot causes not loaded
            shoot typically causes not alive if loaded"
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim()));


            this.textEditor.ResetHighlighting();
        }

        private void OpenFileToEditClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true};
            if (dlg.ShowDialog() ?? false)
            {
                var length = new FileInfo(dlg.FileName).Length;
                if (length > 5 * 1_000_000)
                {
                    MessageBox.Show("File must be smaller than 5 MB", "File too big",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                this.currentFileName = dlg.FileName;
                this.textEditor.Load(this.currentFileName);
            }
        }

        private void SaveFileToEditClick(object sender, EventArgs e)
        {
            if (this.currentFileName == null)
            {
                var dlg = new SaveFileDialog
                {
                    DefaultExt = ".txt"
                };

                if (dlg.ShowDialog() ?? false)
                    this.currentFileName = dlg.FileName;
                else
                    return;
            }

            this.textEditor.Save(this.currentFileName);
        }

        private void SaveResultClick(object sender, EventArgs e)
        {
            if (this.currentFileName == null)
            {
                var dlg = new SaveFileDialog
                {
                    DefaultExt = ".txt"
                };
                if (dlg.ShowDialog() ?? false)
                    this.currentFileName = dlg.FileName;
                else
                    return;
            }

            this.textOutput.Save(this.currentFileName);
        }

        private new void DataContextChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is StoriesViewModel viewmodel)
            {
                viewmodel.GetInput += () => this.Dispatcher.Invoke(() =>
                {
                    this.textOutput.Clear();
                    return this.textEditor.Text;
                });
                viewmodel.AddTextHighlighting += (t) =>
                    this.Dispatcher.Invoke(() =>
                    {
                        if(!addedHighlighting)
                        { foreach (var tuple in t)
                        {
                            this.textEditor.AddHighlighting(tuple.keywords, tuple.color);
                        }
                            this.textEditor.RefreshHighlighting();
                        }

                        this.addedHighlighting = true;
                    });
                
                viewmodel.SaveOutput += output => this.Dispatcher.Invoke(() => this.textOutput.AppendText(output));
            }
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if(this.addedHighlighting)
                this.textEditor.ResetHighlighting();
            this.addedHighlighting = false;
        }

        private void SaveFileToEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void InformationButtonClick(object sender, RoutedEventArgs e)
        {
            var informationDialog = new InformationWindow();
            informationDialog.ShowDialog();
        }
    }
}