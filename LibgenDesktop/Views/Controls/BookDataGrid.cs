using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace LibgenDesktop.Views.Controls
{
    public class BookDataGrid : DataGrid
    {
        public static readonly DependencyProperty SelectedRowsProperty = DependencyProperty.Register("SelectedRows", typeof(IList), typeof(BookDataGrid));

        public IList SelectedRows
        {
            get
            {
                return (IList)GetValue(SelectedRowsProperty);
            }
            set
            {
                SetValue(SelectedRowsProperty, value);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SelectedRows = SelectedItems;
        }
    }
}
