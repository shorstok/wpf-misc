using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace WpfDataGridDispatchTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColumnTestVm ViewModel { get { return (ColumnTestVm)Resources["ViewModel"]; } }

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private async void AddDataGridColsAddedInWorkerThread(object sender, RoutedEventArgs e)
        {         
            await Task.Delay(1).ConfigureAwait(false);    //detaching to thread pool thread

            ViewModel.Columns = new ObservableCollection<DataGridColumn>(); //initializing colleciton in thread pool, but this gets marshalled by WPF

            Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);  //wait for dispatched PropertyChanged to take effect

            //creating cols in thread pool thread
            var cols = new List<DataGridColumn>
            {
                new DataGridTextColumn {Header = "test col 1", Binding = new Binding()},
                new DataGridTextColumn {Header = "test col 2", Binding = new Binding()}
            };

            //adding previously created columns - should throw
            foreach (var dataGridColumn in cols)
                ViewModel.Columns.Add(dataGridColumn);
        }

        private async void AddDataGridColsAddedInUiThread(object sender, RoutedEventArgs e)
        {
            //creating cols in UI thread

            var cols = new List<DataGridColumn>
            {
                new DataGridTextColumn {Header = "test col 1", Binding = new Binding()},
                new DataGridTextColumn {Header = "test col 2", Binding = new Binding()}
            };

            await Task.Delay(1).ConfigureAwait(false);    //detaching to thread pool thread

            ViewModel.Columns = new ObservableCollection<DataGridColumn>(); //initializing colleciton in thread pool

            Dispatcher.Invoke(() => { },DispatcherPriority.ApplicationIdle);  //wait for dispatched PropertyChanged to take effect

            //adding previously created columns - should succeed
            foreach (var dataGridColumn in cols)
                ViewModel.Columns.Add(dataGridColumn);
        }
    }
}
