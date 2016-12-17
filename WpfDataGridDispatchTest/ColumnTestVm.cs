using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfDataGridDispatchTest
{
    public class ColumnTestVm : INotifyPropertyChanged
    {
        private ObservableCollection<DataGridColumn> _columns;

        public ObservableCollection<DataGridColumn> Columns
        {
            get { return _columns; }
            set
            {
                if(Equals(_columns,value))
                    return;

                _columns = value;
                OnPropertyChanged();
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
