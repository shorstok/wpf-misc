using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using wpf_test_2.Annotations;

namespace WpfDataGridDispatchTest
{
    /// <summary>
    /// Collection item viewmodel
    /// </summary>
    public class SomeItemVm : INotifyPropertyChanged
    {
        private string _text;

        public SomeItemVm(string txt)
        {
            Text = txt;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Viewmodel containing some collection
    /// </summary>
    public class DataTemplateTestVm : INotifyPropertyChanged
    {
        private ObservableCollection<SomeItemVm> _items;

        public ObservableCollection<SomeItemVm> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public DataTemplateTestVm()
        {
            Items = new ObservableCollection<SomeItemVm>();

            for (int i = 0; i < 100; i++)
                Items.Add(new SomeItemVm("item " + i));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}