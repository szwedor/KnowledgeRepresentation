namespace Stories
{
    using MahApps.Metro.Controls;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //na odpierdol sie, do wyjebania potem - i tak trzeba zrobić do tego jakies lepsze gui
            var query = Parser.Parsing.GetQuery(QueryTextBox.Text);
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(
                new { query }, Newtonsoft.Json.Formatting.Indented);
            System.Windows.MessageBox.Show(serialized);
        }
    }
}