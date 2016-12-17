using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace WpfDataGridDispatchTest
{
    public class DataGridAtt
    {
        public static readonly DependencyProperty BindableColumnsProperty = DependencyProperty.RegisterAttached(
            "BindableColumns", typeof (ObservableCollection<DataGridColumn>), typeof (DataGridAtt),
            new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static readonly ConcurrentDictionary<ObservableCollection<DataGridColumn>, NotifyCollectionChangedEventHandler> BoundCollections =
            new ConcurrentDictionary<ObservableCollection<DataGridColumn>, NotifyCollectionChangedEventHandler>();

        private static void BindableColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs ne)
        {
            var newValue = (ObservableCollection<DataGridColumn>) ne.NewValue;
            var oldValue =  ne.OldValue as ObservableCollection<DataGridColumn>;

            NotifyCollectionChangedEventHandler handlerWithCapturedContext = (sender, e) => { cols_CollectionChanged(d as DataGrid, e); };

            NotifyCollectionChangedEventHandler oldHandler;

            if (null != oldValue && BoundCollections.TryGetValue(oldValue,out oldHandler))
            {
                oldValue.CollectionChanged -= oldHandler;
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += handlerWithCapturedContext;
                BoundCollections.TryAdd(newValue, handlerWithCapturedContext);
            }
        }

        static void cols_CollectionChanged(DataGrid sender, NotifyCollectionChangedEventArgs ne)
        {
            if (ne.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataGridColumn column in ne.NewItems)
                {
                    var colLocalCopy = column;
                    sender.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        sender.Columns.Add(colLocalCopy);
                    }), DispatcherPriority.Normal);
                }
            }
            else if (ne.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DataGridColumn column in ne.OldItems)
                {
                    var colLocalCopy = column;
                    sender.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        sender.Columns.Remove(colLocalCopy);
                    }), DispatcherPriority.Normal);
                    
                }
            }
        }

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumn>) element.GetValue(BindableColumnsProperty);
        }
    }
}
