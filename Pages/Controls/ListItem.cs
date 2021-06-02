using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using Pages.Helpers;
using System.Windows.Media.Imaging;
using Pages.Model;

namespace Pages
{
    public class ListItem : ContentControl
    {
        private static int count;

        private TextBlock txtName;
        private Image imgPhoto;
        private Image imgPhoto1;
        private Canvas root;
        private Border border;

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(PageItemModel), typeof(ListItem), new PropertyMetadata(null, OnDataChanged));
        //public TwitterUser Data
        //{
        //    get { return (TwitterUser)GetValue(DataProperty); }
        //    set { SetValue(DataProperty, value); }
        //}

        public PageItemModel Data
        {
            get { return (PageItemModel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public ListItem()
        {
            Debug.WriteLine("Constructor called {0}", (++count).ToString()); ;

            root = new Canvas()
            {
                Margin = new Thickness(0, 6, 0, 6),
                Height = 55
            };

            border = new Border()
            {
                Height = 55,
                Width = 55,
                Background = Application.Current.Resources["PhoneAccentBrush"] as Brush
            };

            imgPhoto = new Image()
            {
                Height = 55,
                Width = 55,
                Stretch = Stretch.UniformToFill
            };

            imgPhoto1 = new Image()
            {
                Height = 55,
                Width = 55,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("/Satic/Images/icon_173x173.png", UriKind.Relative))
            };


            txtName = new TextBlock()
            {
                FontSize = 36,
                Margin = new Thickness(10,0,0,0),
                //Style = Application.Current.Resources["PhoneTextExtraLargeStyle"] as Style,
                TextTrimming = TextTrimming.WordEllipsis,
                //Foreground = new SolidColorBrush(Color.FromArgb(0, 216, 48, 44))
                //Foreground = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(txtName, 55);

            root.Children.Add(border);
            root.Children.Add(imgPhoto);
            root.Children.Add(imgPhoto1);
            root.Children.Add(txtName);

            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.Content = root;
            //this.DefaultStyleKey = typeof(FriendListItem);
        }

        //public override void OnApplyTemplate()
        //{
        //    txtName = GetTemplateChild("txtName") as TextBlock;
        //    imgPhoto = GetTemplateChild("imgPhoto") as Image;

        //    TemplateApplied = true;

        //    if (Data != null)
        //    {
        //        UpdateData();
        //    }
        //}

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListItem c = d as ListItem;
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                c.UpdateData();
            }
        }

        public void UpdateData()
        {
            txtName.Text = Data.Name;
            var filename = Data.Id + "small";

            //if (App.IsInternetEnabled)
            //{
            System.Diagnostics.Debug.WriteLine("Setting image content");
                LowProfileImageLoader.SetUriSource(imgPhoto, new Uri(FBHelper.GetImageUrl(Data.Id), UriKind.Absolute));
            //}

            //if (IsoStore.FileExists(filename))
            //{
            //    try
            //    {
            //        var result = new BitmapImage();
            //        result.SetSource(IsoStore.StreamFileFromIsoStore(filename));
            //        imgPhoto.Source = result;

            //    }
            //    catch
            //    {
            //        if (App.ViewModel.IsNetWorkAvailable)
            //        {
            //            ThreadPool.QueueUserWorkItem(delegate
            //            {
            //                Deployment.Current.Dispatcher.BeginInvoke(() =>
            //                {
            //                    LowProfileImageLoader.SetUriSource(imgPhoto, new Uri(FBHelper.GetImageUrl(Data.Id), UriKind.Absolute));
            //                });
            //            });

            //        }
            //    }
            //}
            //else
            //{
            //    if (App.ViewModel.IsNetWorkAvailable)
            //    {
            //        Deployment.Current.Dispatcher.BeginInvoke(() =>
            //        {
            //            LowProfileImageLoader.SetUriSource(imgPhoto, new Uri(FBHelper.GetImageUrl(Data.Id), UriKind.Absolute));
            //        });
            //    }
            //}
        }
    }
}
