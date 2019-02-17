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
using System.Windows.Shapes;

namespace FriendStorage.UI.View.Services
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        private MessageDialogResult _result;
        public MessageDialog(string title, string text, MessageDialogResult defaultResult, params MessageDialogResult[] buttons)
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = text;
            InitializeButtons(buttons);
            _result = defaultResult;
        }

        private void InitializeButtons(MessageDialogResult[] buttons)
        {
            if (buttons == null || buttons.Length == 0)
            {
                buttons = new[] {MessageDialogResult.Ok};
            }

            foreach (var button in buttons)
            {
                var btn = new Button(){Content = button, Tag = button};
                ButtonsPanel.Children.Add(btn);
                btn.Click += ButtonClick;
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                _result = (MessageDialogResult) button.Tag;
                this.Close();
            }
        }

        public new MessageDialogResult ShowDialog()
        {
            base.ShowDialog();
            return _result;
        }
    }
}
