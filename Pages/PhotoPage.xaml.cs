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
using Microsoft.Phone.Controls;
using Pages.Model;
using Pages.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.Threading;
using Microsoft.Phone.Shell;
using Pages.Controls;

namespace Pages
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        private string albumId = string.Empty;
        private string photoId = string.Empty;
        private string albumName = string.Empty;

        private AlbumPhotosViewModel viewModel;

        public PhotoViewModel currentselectedmodel = null;

        private bool canLoadLargeImage = false;

        ApplicationBarIconButton button1 = new ApplicationBarIconButton();
        ApplicationBarIconButton button2 = new ApplicationBarIconButton();
        ApplicationBarIconButton button3 = new ApplicationBarIconButton();
        ApplicationBarIconButton button4 = new ApplicationBarIconButton();
        

        public PhotoPage()
        {
            InitializeComponent();

            this.viewModel = new AlbumPhotosViewModel();

            
            this.Loaded += new RoutedEventHandler(PhotoPage_Loaded);

        }

        void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddAppBar();
        }


        private void AddAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.ForegroundColor = Colors.White;
            //Color color = new Color();
            //color.A = 0;
            //color.R = 216;
            //color.G = 48;
            //color.B = 44;
            //ApplicationBar.BackgroundColor = Utilities.GetColorFromHexString(;
            ApplicationBar.BackgroundColor = Utilities.GetColorFromHexString("#d8302c");

            button1.IconUri = new Uri("/Static/AppBarIcons/appbar.back.rest.png", UriKind.Relative);
            button1.Text = "previous";
            button1.IsEnabled = true;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += BarButtonPrevious_Click;

            button2.IconUri = new Uri("/Static/AppBarIcons/appbar.comment.text.rest.png", UriKind.Relative);
            button2.Text = "comment";
            button2.IsEnabled = false;
            ApplicationBar.Buttons.Add(button2);
            button2.Click += BarButtonComment_Click;


            button3.IconUri = new Uri("/Static/AppBarIcons/appbar.social.like.rest.png", UriKind.Relative);
            button3.Text = "like";
            ApplicationBar.Buttons.Add(button3);
            button3.IsEnabled = false;
            button3.Click += BarButtonLike_Click;

            button4.IconUri = new Uri("/Static/AppBarIcons/appbar.next.rest.png", UriKind.Relative);
            button4.Text = "next";
            button4.IsEnabled = true;
            ApplicationBar.Buttons.Add(button4);
            button4.Click += BarButtonNext_Click;

            //if (App.ViewModel.IsNetWorkAvailable)
            //{
            //    this.CheckPostLike();
            //}

            this.ApplicationBar = ApplicationBar;
        }

        void viewModel_OnPhotosLoaded(object o, LoadedEventArgs e)
        {
            this.txtLoadingPhotos.Visibility = Visibility.Collapsed;
            this.progressBarNew.Visibility = Visibility.Collapsed;
            
            var result = from c in this.viewModel.Photos where c.Id.Equals(this.photoId) select c;

            try
            {
                canLoadLargeImage = true;
                int index = this.viewModel.Photos.IndexOf(result.First<PhotoViewModel>());
                
                this.pvtPhotos.SelectedIndex = index;
                button2.IsEnabled = true;
            
            }
            catch
            {
 
            }

            
            //if (this.viewModel.Photos.Count > 0)
            //{
            //    foreach (PhotoViewModel photo in this.viewModel.Photos)
            //    {
            //        if(!string.IsNullOrEmpty(photo.Picture))
            //        {
            //            PivotItem item = new PivotItem();
            //            Grid grid = new Grid();

            //            Image image = new Image();
            //            image.Source = new BitmapImage(new Uri(photo.Picture, UriKind.Absolute));
            //            //image.Stretch = Stretch.UniformToFill;
                    
            //            grid.Children.Add(image);

            //            item.Content = grid;
            //            this.pvtPhotos.Items.Add(item);
            //        }
            //    }
            //}
        }

        private void CheckPostLike()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                button3.IsEnabled = false;
                button2.IsEnabled = false;

                WebClient clientCheckLike = new WebClient();
                string url = FBHelper.GetCheckPostLikeUrl(currentselectedmodel.Id);
                clientCheckLike.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientCheckLike_DownloadStringCompleted);
                clientCheckLike.DownloadStringAsync(new Uri(url, UriKind.Absolute));
            });
        }

        void clientCheckLike_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken resultString = (JToken)responeJObject["data"];
                    JArray resultArray = (JArray)resultString;

                    if (resultArray.Count == 0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            button3.IsEnabled = true;
                            button3.Text = "like";
                            button3.IconUri = new Uri("/Static/AppBarIcons/appbar.social.like.rest.png", UriKind.Relative);
                        }
                        );
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            button3.IsEnabled = true;
                            button3.Text = "unlike";
                            button3.IconUri = new Uri("/Static/AppBarIcons/appbar.social.unlike.rest.png", UriKind.Relative);
                        }
                        );
                    }
                }
            }
            button2.IsEnabled = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["PHOTOID"]))
            {
                this.photoId = this.NavigationContext.QueryString["PHOTOID"];
            }

            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["ALBUMID"]))
            {
                this.albumId = this.NavigationContext.QueryString["ALBUMID"];
            }

            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["ALBUMNAME"]))
            {
                this.albumName = this.NavigationContext.QueryString["ALBUMNAME"].ToUpper();
                
            }

            if (App.ViewModel.IsNetWorkAvailable)
            {
                this.viewModel.LoadData(albumId);
                this.viewModel.OnPhotosLoaded += new DataLoadedHandler(viewModel_OnPhotosLoaded);
                this.DataContext = viewModel;
                if (string.IsNullOrEmpty(this.albumName))
                {
                    this.LoadAlbumName();
                }
                else
                {
                    this.txtAlbumName.Text = this.albumName;
                }
            }
            else
            {
                App.HandleNoConnection();
            }

            base.OnNavigatedTo(e);
        }

        private void LoadAlbumName()
        {
            WebClient clientAlbum = new WebClient();

            clientAlbum.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientAlbum_DownloadStringCompleted);
            string url = FBHelper.GetAlbumNameURL(this.albumId);

            clientAlbum.DownloadStringAsync(new Uri(url, UriKind.Absolute));
        }

        void clientAlbum_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken token = (JToken)responeJObject;

                    string name = (string)token["name"];
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.txtAlbumName.Text = name.ToUpper();
                    });
                }
            }
        }

        private void pvtPhotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.pvtPhotos.SelectedIndex != -1 && canLoadLargeImage)
            {
                PhotoViewModel model = e.AddedItems[0] as PhotoViewModel;

                if (model != null)
                {
                    try
                    {

                        currentselectedmodel = model;

                        if (!model.IsLargeImageLoaded)
                        {
                            WebClient wc = new WebClient();
                            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);

                            if (model.Source != null)
                            {
                                wc.OpenReadAsync(new Uri(model.Source), model);
                            }

                        }

                        this.CheckPostLike();

                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                    }
                }
            }
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                PhotoViewModel model = e.UserState as PhotoViewModel;
                try
                {
                    BitmapImage image = new BitmapImage();

                    image.SetSource(e.Result);

                    var result = from c in this.viewModel.Photos where c.Id.Equals(model.Id) select c;

                    int index = this.viewModel.Photos.IndexOf(result.First<PhotoViewModel>());

                    PivotItem item = (PivotItem)this.pvtPhotos.ItemContainerGenerator.ContainerFromIndex(index);

                    //Image imageControl = (Image)FindElementRecursive(item, typeof(Image));
                    PhotoUserControl control = (PhotoUserControl)FindElementRecursive(item, typeof(PhotoUserControl));

                    control.imgPhoto.Source = image;
                    //Image imageControl = (Image)FindElementRecursive(control, typeof(Image));

                    //if (imageControl != null)
                    //{
                    //    imageControl.Source = image;
                    //}

                    model.IsLargeImageLoaded = true;
                }
                catch (Exception ex)
                {
                    //Exception handle 
                }
            }
            else
            {
                //Either cancelled or 
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

        private void BarButtonComment_Click(object sender, EventArgs e)
        {

        }

        private void BarButtonLike_Click(object sender, EventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {

                    if (currentselectedmodel == null)
                    {
                        currentselectedmodel = this.viewModel.Photos[this.pvtPhotos.SelectedIndex];
                    }

                    var clientLike = new WebClient();

                    clientLike.UploadStringCompleted += new UploadStringCompletedEventHandler(clientLike_UploadStringCompleted);

                    if (this.button3.Text.Equals("like"))
                    {                            
                        clientLike.UploadStringAsync(FBHelper.GetPostLikeURI(currentselectedmodel.Id), "POST", FBHelper.GetPostParameters(App.AccesToken));
                    }

                    if (this.button3.Text.Equals("unlike"))
                    {
                        clientLike.UploadStringAsync(FBHelper.GetPostLikeURI(currentselectedmodel.Id), "DELETE", FBHelper.GetPostParameters(App.AccesToken));
                    }
                    

                });
            }
            else
            {
                App.HandleNoConnection();
            }
        }

        void clientLike_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (this.button3.Text.Equals("like"))
                        {
                            MessageBox.Show("You have liked this post!");
                            button3.Text = "unlike";
                            button3.IconUri = new Uri("/Static/AppBarIcons/appbar.social.unlike.rest.png", UriKind.Relative);

                        }

                        else if (this.button3.Text.Equals("unlike"))
                        {
                            MessageBox.Show("You have unliked this post!");
                            button3.Text = "like";
                            button3.IconUri = new Uri("/Static/AppBarIcons/appbar.social.like.rest.png", UriKind.Relative);

                        }
                    });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                });
            }
        }


        private void BarButtonNext_Click(object sender, EventArgs e)
        {
            if (this.pvtPhotos.SelectedIndex == this.pvtPhotos.Items.Count - 1)
            {
                this.pvtPhotos.SelectedIndex = 0;
            }
            else
            {
                this.pvtPhotos.SelectedIndex++;
            }
            
        }

        private void BarButtonPrevious_Click(object sender, EventArgs e)
        {
            if (this.pvtPhotos.SelectedIndex == 0)
            {
                this.pvtPhotos.SelectedIndex = this.pvtPhotos.Items.Count - 1;
            }
            else
            {
                this.pvtPhotos.SelectedIndex--;
            }
        }

        private void txtComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(this.txtComment.Text))
                {
                    if (App.ViewModel.IsNetWorkAvailable)
                    {
                        string comment = this.txtComment.Text;
                        this.pvtPhotos.Focus();

                        this.txtComment.Text = string.Empty;

                        if (currentselectedmodel == null)
                        {
                            currentselectedmodel = this.viewModel.Photos[this.pvtPhotos.SelectedIndex];
                        }

                        ThreadPool.QueueUserWorkItem(delegate
                        {

                            var clientPost = new WebClient();

                            clientPost.UploadStringCompleted += new UploadStringCompletedEventHandler(clientPost_UploadStringCompleted);

                            clientPost.UploadStringAsync(FBHelper.GetPostMessageURI(currentselectedmodel.Id), "POST", FBHelper.GetPostParameters(App.AccesToken, comment));
                        });
                    }
                    else
                    {
                        App.HandleNoConnection();
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a comment and try again.");
                }
            }
        }

        void clientPost_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Your comment has been submitted!");
                    });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                });
            }
        }

    }
}