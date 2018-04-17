using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Stories.Execution;
using Stories.Parser;

namespace Stories
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample usage of parser and execution

            var yaleShootingProblem = @"
            initially not loaded
            initially alive

            load causes loaded
            shoot causes not loaded
            when bob shoot causes not alive if loaded";

            var history = Parsing.GetHistory(yaleShootingProblem);

            var story = new Story(history);
        }
    }
}
