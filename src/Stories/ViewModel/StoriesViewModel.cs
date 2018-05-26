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
    using Stories.Query;
    using Stories.Parser.Statements;
    using Stories.Parser.Statements.QueryStatements;

    public class StoriesViewModel : INotifyPropertyChanged
    {
        private bool _isProcessing;
        private IExecutor Execute<T>(T qt) where T: QueryStatement
        {
            var type = typeof(IExecutor<>).MakeGenericType(qt.GetType());
            return (IExecutor)DI.Container.GetInstance(type);
        }
        
        public StoriesViewModel()
        {
            this.CopyListItem = new AsyncCommand(async () =>
            {
                Copy();
            });
            this.ProcessCommand = new AsyncCommand(
                async () =>
                {
                    try
                    {
                        this.IsProcessing = true;
                        await Task.Run(() =>
                        {
                            var (storyText, queryText) = this.GetInput();
                            var queryTexts = queryText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => p.Trim());
                          
                            var history = Parsing.GetHistory(storyText);
                            var story = new Story(history);
                            var keywords = 
                                new (IEnumerable<string>,Color)[]
                                {
                                    (story.Agents, Colors.Purple),
                                    (story.Actions, Colors.CadetBlue),
                                    (story.Fluents.Where(p=>p.IsInertial).Select(p=>p.Label), Colors.DeepPink),
                                    (story.Fluents.Where(p=>!p.IsInertial).Select(p=>p.Label), Colors.DeepSkyBlue)
                                };

                            this.AddTextHighlighting(keywords);
                            foreach (var qt in queryTexts.Reverse())
                            {
                                var query = Parser.Parsing.GetQuery(qt);
                                var executor = Execute(query);
                                var graph = Graph.CreateGraph(story, query);
                                var result = executor.Execute(query, graph, history);
                                SaveOutput((storyText, qt, result));
                            }
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
        public ICommand CopyListItem { get; }

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

        public event Func<(string story, string query)> GetInput;

        public event Action<(string history, string query, bool result)> SaveOutput;
        public event Action Copy;

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