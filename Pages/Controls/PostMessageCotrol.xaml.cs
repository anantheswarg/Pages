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
using Microsoft.Phone.Tasks;
using Pages.Model;

namespace Pages.Controls
{
    public partial class PostMessageCotrol : UserControl
    {
        public PostMessageCotrol()
        {
            InitializeComponent();
        }

        private void txtLinkName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                TextBlock text = sender as TextBlock;

                PostViewModel model = text.DataContext as PostViewModel;

                if (model.Link != null)
                {
                    WebBrowserTask task = new WebBrowserTask();

                    task.Uri = new Uri(model.Link, UriKind.Absolute);
                    task.Show();
                }
            }
            else
            {
                App.HandleNoConnection();
            }
        }



        private void imgPost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (App.ViewModel.IsNetWorkAvailable)
                {
                    Image img = sender as Image;

                    PostViewModel model = img.DataContext as PostViewModel;

                    if (model.Link != null)
                    {
                        WebBrowserTask task = new WebBrowserTask();

                        string url = model.Link + "&access_token=" + App.AccesToken;

                        task.Uri = new Uri(url, UriKind.Absolute);
                        task.Show();
                    }
                }
                else
                {
                    App.HandleNoConnection();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtLinkName_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                TextBlock text = sender as TextBlock;

                PostViewModel model = text.DataContext as PostViewModel;

                if (model.Link != null)
                {
                    WebBrowserTask task = new WebBrowserTask();

                    task.Uri = new Uri(model.Link, UriKind.Absolute);
                    task.Show();
                }
            }
            else
            {
                App.HandleNoConnection();
            }
        }

        private void StackPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };

        }

        private void StackPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
            
        }
    }
}
