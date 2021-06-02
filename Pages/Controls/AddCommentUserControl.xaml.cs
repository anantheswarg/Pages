using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Pages.Controls
{
    public partial class AddCommentUserControl : UserControl
    {
        public static readonly DependencyProperty UserControlTextProperty = DependencyProperty.Register(
        "UserControlText",
        typeof(string),
        typeof(AddCommentUserControl),
        new PropertyMetadata("add a comment", null)
        );

        public string UserControlText
        {
            get { return (string)GetValue(UserControlTextProperty); }
            set { SetValue(UserControlTextProperty, value); }
        }

        public AddCommentUserControl()
        {
            InitializeComponent();
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.UserControlText = txtComment.Text;
        }

        private void txtComment_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtComment.Text.Equals("add a comment"))
            {
                this.txtComment.Text = "";

                Color color = 
            (Color)Application.Current.Resources["PhoneTextBoxForegroundColor"];

                this.txtComment.Foreground = new SolidColorBrush(color);
            }
        }

        private void txtComment_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtComment.Text))
            {               
                this.txtComment.Foreground = new SolidColorBrush(Colors.Gray);

                this.txtComment.Text = "add a comment";
            }
        }
    }
}
