using Stories.Information;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Stories.View
{
    /// <summary>
    /// Interaction logic for InformationControl.xaml
    /// </summary>
    public partial class InformationControl : UserControl
    {
        ObservableCollection<InformationEntry> InformationEntries;


        void OrganizeBindingsAndData()
        {
            InformationEntries = new ObservableCollection<InformationEntry>(InformationsDefinitions.definitions);
            //TODO niech ma kolekcję podawaną z zewnątrz, a nie tak, bo się nie da tego nigdzie indziej użyć potem
            //chociaż to akurat w przypadku projektu takiej skali bez znaczenia xd
            EntriesPanel.ItemsSource = InformationEntries;
        }

        public InformationControl()
        {
            InitializeComponent();
            OrganizeBindingsAndData();
            EntriesPanel.SelectedItem = InformationEntries[0];
        }

        private void EntriesPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.InformationBrowser.NavigateToString(((InformationEntry)EntriesPanel.SelectedItem).HtmlDescription);
        }

        
    }


}
