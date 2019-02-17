using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FriendStorage.UI.Behaviors
{
    public static class ChangeBehavior
    {
        private static readonly Dictionary<Type, DependencyProperty> _defaultProperties;

        static ChangeBehavior()
        {
            IsActiveProperty =
                DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(ChangeBehavior), new PropertyMetadata(false,OnIsActivePropertyChanged));

            IsChangedProperty =
                DependencyProperty.RegisterAttached("IsChanged", typeof(bool), typeof(ChangeBehavior), new PropertyMetadata(false));

            OriginalValueProperty =
                DependencyProperty.RegisterAttached("OriginalValue", typeof(object), typeof(ChangeBehavior), new PropertyMetadata(null));

            OriginalValueConverterProperty =
                DependencyProperty.RegisterAttached("OriginalValueConverter", typeof(IValueConverter), typeof(ChangeBehavior), new PropertyMetadata(null,OnOriginalValueConverterPropertyChanged));

            _defaultProperties = new Dictionary<Type, DependencyProperty>()
            {
                {typeof(TextBox),TextBox.TextProperty},
                {typeof(CheckBox),ToggleButton.IsCheckedProperty },
                {typeof(DatePicker),DatePicker.SelectedDateProperty},
                {typeof(ComboBox),Selector.SelectedValueProperty }
            };
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



        public static bool GetIsChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsChangedProperty);
        }

        public static void SetIsChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsChangedProperty;



        public static object GetOriginalValue(DependencyObject obj)
        {
            return (object)obj.GetValue(OriginalValueProperty);
        }

        public static void SetOriginalValue(DependencyObject obj, object value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }


        // Using a DependencyProperty as the backing store for OriginalValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalValueProperty;



        public static IValueConverter GetOriginalValueConverter(DependencyObject obj)
        {
            return (IValueConverter)obj.GetValue(OriginalValueConverterProperty);
        }

        public static void SetOriginalValueConverter(DependencyObject obj, IValueConverter value)
        {
            obj.SetValue(OriginalValueConverterProperty, value);
        }

        // Using a DependencyProperty as the backing store for OriginalValueConverter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalValueConverterProperty;




        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_defaultProperties.ContainsKey(d.GetType()))
            {
                var defaultProperty = _defaultProperties[d.GetType()];
                if ((bool) e.NewValue)
                {
                    var binding = BindingOperations.GetBinding(d, defaultProperty);
                    if (binding != null)
                    {
                        string bindingPath = binding.Path.Path;
                        BindingOperations.SetBinding(d, IsChangedProperty, 
                            new Binding(bindingPath + "IsChanged"));
                        //BindingOperations.SetBinding(d, OriginalValueProperty,
                        //    new Binding(bindingPath + "OriginalValue")
                        //    {
                        //        Converter = GetOriginalValueConverter(d)
                        //    });
                        CreateOriginalValueBinding(d,bindingPath+"OriginalValue");

                    }           
                }
                else
                {
                    BindingOperations.ClearBinding(d,IsChangedProperty);
                    BindingOperations.ClearBinding(d,OriginalValueProperty);
                }
            }
        }

        //Because OriginalValueConverter is set in style, the IsActive property is set before this style, so valueconverter is null
        private static void OnOriginalValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var originalValueBinding = BindingOperations.GetBinding(d, OriginalValueProperty);
            if (originalValueBinding != null)
            {
                //var newBinding = new Binding(originalValueBinding.Path.Path)
                //{
                //    Converter = GetOriginalValueConverter(d)
                //};
                //BindingOperations.SetBinding(d, OriginalValueProperty, newBinding);
                CreateOriginalValueBinding(d, originalValueBinding.Path.Path);
            }

        }

        private static void CreateOriginalValueBinding(DependencyObject d, string originalValueBindingPath)
        {
            var newBinding = new Binding(originalValueBindingPath)
            {
                Converter = GetOriginalValueConverter(d),
                ConverterParameter = d
            };
            BindingOperations.SetBinding(d, OriginalValueProperty, newBinding);
        }
    }
}
