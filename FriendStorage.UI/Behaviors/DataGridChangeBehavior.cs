using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FriendStorage.UI.Behaviors
{
    public static class DataGridChangeBehavior
    {
        static DataGridChangeBehavior()
        {
            IsActiveProperty =
                DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(DataGridChangeBehavior), new PropertyMetadata(false,OnIsActivePropertyChanged));
        }


        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty;

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool) e.NewValue)
                {
                    dataGrid.Loaded += DataGrid_Loaded;
                }
                else
                {
                    dataGrid.Loaded -= DataGrid_Loaded;
                }
            }
        }

        private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var datagrid = (DataGrid)sender;
            foreach (var textColumn in datagrid.Columns.OfType<DataGridTextColumn>())
            {
                var binding = textColumn.Binding as Binding;
                if (binding != null)
                {
                    textColumn.EditingElementStyle = CreateEditingElementStyle(datagrid, binding.Path.Path);

                    textColumn.ElementStyle = CreateElementStyle(datagrid, binding.Path.Path);
                }
            }
        }

        private static Style CreateElementStyle(DataGrid datagrid, string bindingPath)
        {
            var baseStyle = datagrid.FindResource("TextBlockBaseStyle") as Style;
            var style = new Style(typeof(TextBlock),baseStyle);
            AddSetters(style,bindingPath,datagrid);
            return style;
        }

        private static Style CreateEditingElementStyle(DataGrid datagrid, string bindingPath)
        {
            var baseStyle = datagrid.FindResource(typeof(TextBox)) as Style;
            var style = new Style(typeof(TextBox), baseStyle);
            AddSetters(style, bindingPath,datagrid);
            return style;
        }

        private static void AddSetters(Style style, string bindingPath,DataGrid datagrid)
        {
            style.Setters.Add(new Setter(ChangeBehavior.IsActiveProperty,false));
            style.Setters.Add(new Setter(ChangeBehavior.IsChangedProperty,new Binding(bindingPath + "IsChanged")));
            style.Setters.Add(new Setter(ChangeBehavior.OriginalValueProperty,new Binding(bindingPath+"OriginalValue")));
            style.Setters.Add(new Setter(Validation.ErrorTemplateProperty, datagrid.FindResource("ErrorInsideErrorTemplate")));
        }

    }
}
