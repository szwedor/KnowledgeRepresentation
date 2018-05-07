namespace Stories.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Newtonsoft.Json;
    using Stories.Annotations;
    using Stories.Execution;
    using Stories.Parser;
    using Stories.Graph;

    public class StoriesViewModel : INotifyPropertyChanged
    {
        private bool _isProcessing;

        public StoriesViewModel()
        {
            this.ProcessCommand = new AsyncCommand(
                async () =>
                {
                    try
                    {
                        this.IsProcessing = true;
                        await Task.Run(() =>
                        {
                            var history = Parsing.GetHistory(this.GetInput());
                            var story = new Story(history);
                            var graph = Graph.CreateGraph(story, null);
                            var serialized = JsonConvert.SerializeObject(
                                new{history,story}, Formatting.Indented);
                            var keywords = 
                                new (IEnumerable<string>,Color)[]
                                {
                                    (story.Agents, Colors.Purple),
                                    (story.Actions, Colors.CadetBlue),
                                    (story.Fluents.Where(p=>p.IsInertial).Select(p=>p.Label), Colors.DeepPink),
                                    (story.Fluents.Where(p=>!p.IsInertial).Select(p=>p.Label), Colors.DeepSkyBlue)
                                };

                            this.AddTextHighlighting(keywords);
                            this.SaveOutput(serialized +"\n"+graph.GetString());
                        });
                        this.IsProcessing = false;
                    }
                    catch (Exception)
                    {
                      //  MessageBox.Show("Processing failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
        }

        public ICommand ProcessCommand { get; }

        public bool IsProcessing
        {
            get => this._isProcessing;
            set
            {
                if (value == this._isProcessing) return;
                this._isProcessing = value;
                this.OnPropertyChanged();
            }
        }

        public event Action<(IEnumerable<string> keywords, Color color)[]> AddTextHighlighting;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Func<string> GetInput;

        public event Action<string> SaveOutput;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}