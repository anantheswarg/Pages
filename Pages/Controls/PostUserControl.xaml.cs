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
using Pages.Model;
using Pages.Helpers;
using Microsoft.Phone.Tasks;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace Pages.Controls
{
    public partial class PostUserControl : UserControl
    {

        public bool isImageSelected = false;
        public bool isLinkSelected = false;
        public bool isLinkOpened = false;

        private ScrollBar sb = null;
        private ScrollViewer sv = null;
        private ScrollViewer ActiveScroll = null;
        private bool _isBouncy = false;
        private bool alreadyHookedScrollEvents = false;


        public PostUserControl()
        {
            InitializeComponent();
            
            this.Loaded += PostUserControl_Loaded;

        }

        void PostUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            App.PostUserControl = this;

            this.HookUpScroll();
        }

        public void HookUpScroll()
        {
            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;

            sv = (ScrollViewer)FindElementRecursive(this.lsbPosts, typeof(ScrollViewer));


            if (sv != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(sv, 0) as FrameworkElement;

                if (element != null)
                {
                    VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                    {
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroup_CurrentStateChanging);
                    }
                }
            }

        }

        private UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement returnElement = null;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Object element = VisualTreeHelper.GetChild(parent, i);
                    if (element.GetType() == targetType)
                    {
                        return element as UIElement;
                    }
                    else
                    {
                        returnElement = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                    }
                }
            }
            return returnElement;
        }
        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        private void vgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                if (Utilities.IsInternetConnectionEnabled())
                {
                    App.CurrentDetailsModel.MorePostsDownloading = true;
                    App.CurrentDetailsModel.UpdatePosts(App.CurrentDetailsModel.Posts.Count, 25);
                }
                else
                {
                    Utilities.HandleNoConnection();
                }
            }
        }


        private void lsbPosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.lsbPosts.SelectedIndex != -1)
                {
                    PostViewModel model = lsbPosts.SelectedItem as PostViewModel;

                    if (!isImageSelected && !isLinkSelected)
                    {
                        if (!isLinkOpened)
                        {
                            App.SelectedPostModel = model;

                            App.NavigateToPage(PageReferences.AddCommentsPage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.lsbPosts.SelectedIndex = -1;
        }

        private void txtLinkName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock text = sender as TextBlock;

            this.isLinkSelected = true;

            if (!isImageSelected)
            {
                PostViewModel model = text.DataContext as PostViewModel;

                if (model.Link != null)
                {
                    this.ProcessModelSelection(model);
                }
            }
        }

        private void imgPost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image img = sender as Image;

                this.isImageSelected = true;

                if (!isLinkSelected)
                {
                    PostViewModel model = img.DataContext as PostViewModel;

                    this.ProcessModelSelection(model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ProcessModelSelection(PostViewModel model)
        {
            if (model.Type.Equals("photo"))
            {
                App.SelectedPostModel = model;

                if (model.Link != null)
                {
                    int index = model.Link.IndexOf("set=a.");
                    string albumId = model.Link.Substring(index).Split('.')[1];

                    int index1 = model.Link.IndexOf("fbid=");
                    string photoId = model.Link.Substring(index1).Split('&')[0].Substring(5);

                    //App.NavigateToPage(PageReferences.AlbumPhotosPage + string.Format("?NAME={0}&ID={1}", "", albumId));
                    App.NavigateToPage(PageReferences.PhotoPage + string.Format("?ALBUMNAME={0}&PHOTOID={1}&ALBUMID={2}", "", photoId, albumId));

                }

                //this.lsbPosts.SelectedIndex = -1;
                //App.NavigateToPage(PageReferences.AddCommentsPage);
            }
            else if (model.Type.Equals("video") || model.Type.Equals("link"))
            {
                if (model.Link != null)
                {
                    WebBrowserTask task = new WebBrowserTask();

                    string url = model.Link + "&access_token=" + App.AccesToken;

                    task.Uri = new Uri(url, UriKind.Absolute);
                    this.isLinkOpened = true;
                    task.Show();
                }
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

        #region ScrollEndCommand Property
        public ICommand ScrollEndCommand
        {
            // to be changed for private floats
            get { return App.CurrentDetailsModel.FetchMorePosts; }
            set { SetValue(ScrollEndCommandProperty, value); }
        }

        public static readonly DependencyProperty ScrollEndCommandProperty =
            DependencyProperty.Register(
                                           "ScrollEndCommand",
                                           typeof(ICommand),
                                           typeof(PostUserControl),
                                           new PropertyMetadata(ScrollEndCommandPropertyChangedCallback));

        private static void ScrollEndCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var floatListBox = d as PostUserControl;
            if (floatListBox == null) return;

            Pages.Controls.ScrollViewerMonitor.SetAtEndCommand(floatListBox.lsbPosts, e.NewValue as ICommand);
            Pages.Controls.ScrollViewerMonitor.OnAtEndCommandChanged(floatListBox.lsbPosts, e);
        }
        #endregion
    }
}
