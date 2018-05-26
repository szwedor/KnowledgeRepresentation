namespace Stories.View
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Microsoft.Win32;
    using Stories.Parser.Parsers;
    using Stories.ViewModel;

    public partial class StoryInputView : UserControl
    {
        private Dictionary<string, string> fileNames = new Dictionary<string, string>();

        private bool addedHighlighting;
        public StoryInputView()
        {
            this.InitializeComponent();

            this.storyEditor.Text = string.Join("\r\n", @"
            initially not loaded
            initially alive
            when John load causes loaded
            shoot causes not loaded
            shoot typically causes not alive if loaded"
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim()));
            this.queryEditor.Text = string.Join("\r\n", @"
           necessary John in load
           necessary Mike in cut"
             .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(p => p.Trim()));

            this.storyEditor.ResetHighlighting(false);
            this.queryEditor.ResetHighlighting(true);
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is TextEditor te)
            { var dlg = new OpenFileDialog { CheckFileExists = true };
                if (dlg.ShowDialog() ?? false)
                {
                    var length = new FileInfo(dlg.FileName).Length;
                    if (length > 5 * 1_000_000)
                    {
                        MessageBox.Show("File must be smaller than 5 MB", "File too big",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    fileNames[te.Name] = dlg.FileName;
                    this.storyEditor.Load(fileNames[te.Name]);
                }
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {
            if (sender is Button b && b.DataContext is TextEditor te)
            {
                if (fileNames.TryGetValue(te.Name, out string value))
                    this.storyEditor.Save(value);
                else
                {
                    var dlg = new SaveFileDialog
                    {
                        DefaultExt = ".txt"
                    };

                    if (dlg.ShowDialog() ?? false)
                        fileNames[te.Name] = dlg.FileName;
                    else
                        return;
                }

            }
        }


        private new void DataContextChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is StoriesViewModel viewmodel)
            {
                viewmodel.GetInput += () => this.Dispatcher.Invoke(() =>
                {
                    return (this.storyEditor.Text, this.queryEditor.Text);
                });
                viewmodel.AddTextHighlighting += (t) =>
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!addedHighlighting)
                        { foreach (var tuple in t)
                            {
                                this.storyEditor.AddHighlighting(tuple.keywords, tuple.color);
                                this.queryEditor.AddHighlighting(tuple.keywords, tuple.color);
                            }
                            this.storyEditor.RefreshHighlighting();
                            this.queryEditor.RefreshHighlighting();
                        }

                        this.addedHighlighting = true;
                    });
              
                viewmodel.SaveOutput += (t) => this.Dispatcher.Invoke(() =>
                 {
                     var treeItem = new TreeViewItem();
                     treeItem.Header = t.query + " " + DateTime.Now.ToLocalTime();
                     var color = t.result ? TrueColor : FalseColor;
                     treeItem.Background = color;
                     treeItem.Items.Add(new TreeViewItem() { Header = t.history });

                     this.box.Items.Insert(0, treeItem);
                 });
                viewmodel.Copy += () => this.Dispatcher.Invoke(() =>
                   {
                       var text = (this.box.SelectedItem as TreeViewItem).Header.ToString();
                       Clipboard.SetText(text);
                   });

            }
        }
        private Brush FalseColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#70FF6961"));
        private Brush TrueColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#70AAFCA3"));
        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (this.addedHighlighting)
            { this.storyEditor.ResetHighlighting(false);
            
                this.queryEditor.ResetHighlighting(true);}
            this.addedHighlighting = false;
        }
      

    }
}