using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
    public static class DataGridAtt
    {
        public static readonly DependencyProperty BindableColumnsProperty = DependencyProperty.RegisterAttached(
            "BindableColumns", typeof (ObservableCollection<DataGridColumn>), typeof (DataGridAtt),
            new UIPropertyMetadata(null, BindableColumnsPropertyChanged));
        
        private static void BindableColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs ne)
        {
            var newValue = (ObservableCollection<DataGridColumn>) ne.NewValue;
            var oldValue =  ne.OldValue as ObservableCollection<DataGridColumn>;

            var grid = d as DataGrid;

            if(null == grid)
                return;

            if (null != oldValue)
                oldValue.CollectionChanged -= grid.CollectionChanged;

            if (newValue != null)
            {
                InitGridColumns(grid, newValue);
                newValue.CollectionChanged += grid.CollectionChanged;
            }
        }

        private static void InitGridColumns(DataGrid grid, ObservableCollection<DataGridColumn> newValue)
        {
            grid.Columns.Clear();

            foreach (var dataGridColumn in newValue)
                grid.Columns.Add(dataGridColumn);
        }

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        private static void CollectionChanged(this DataGrid grid, object sender, NotifyCollectionChangedEventArgs ne)
        {
            if (ne.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DataGridColumn column in ne.NewItems)
                {
                    var colLocalCopy = column;
                    grid.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        grid.Columns.Add(colLocalCopy);
                    }), DispatcherPriority.Normal);
                }
            }
            else if (ne.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DataGridColumn column in ne.OldItems)
                {
                    var colLocalCopy = column;
                    grid.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        grid.Columns.Remove(colLocalCopy);
                    }), DispatcherPriority.Normal);

                }
            }
        }

        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumn>) element.GetValue(BindableColumnsProperty);
        }
    }
}
