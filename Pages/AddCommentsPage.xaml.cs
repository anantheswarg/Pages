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
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using Pages.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Facebook;
using Pages.Controls;
using Microsoft.Phone.Shell;
using System.Threading;

namespace Pages
{
    public partial class AddCommentsPage : PhoneApplicationPage
    {
        private ObservableCollection<object> comments = new ObservableCollection<object>();
        ApplicationBarIconButton button1 = new ApplicationBarIconButton();
        ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            
        public AddCommentsPage()
        {
            InitializeComponent();
            this.DataContext = App.SelectedPostModel;
            this.Loaded += new RoutedEventHandler(AddCommentsPage_Loaded);
        }

        void AddCommentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddAppBar();

            this.comments.Add(App.SelectedPostModel);

            if (App.ViewModel.IsNetWorkAvailable)
            {
                this.LoadComments();
            }
            else
            {
                App.HandleNoConnection();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void AddAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.ForegroundColor = Colors.White;
            
            ApplicationBar.BackgroundColor = Utilities.GetColorFromHexString("#d8302c");


            button1.IconUri = new Uri("/Static/AppBarIcons/appbar.comment.text.rest.png", UriKind.Relative);
            button1.Text = "comment";
            button1.IsEnabled = false;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += BarButtonComment_Click;

            button2.IconUri = new Uri("/Static/AppBarIcons/appbar.social.like.rest.png", UriKind.Relative);
            button2.Text = "like";
            ApplicationBar.Buttons.Add(button2);
            button2.IsEnabled = false;
            button2.Click += BarButtonLike_Click;

            if (App.ViewModel.IsNetWorkAvailable)
            {
                this.CheckPostLike();
            }
            
            this.ApplicationBar = ApplicationBar;
        }

        private void CheckPostLike()
        {
            
                ThreadPool.QueueUserWorkItem(delegate
                {
                    WebClient clientCheckLike = new WebClient();
                    string id = App.SelectedPostModel.Id.Split('_')[1];
                    string url = FBHelper.GetCheckPostLikeUrl(id);
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
                            button2.IsEnabled = true;
                        }
                        );
                    }   
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            button2.IsEnabled = true;
                            button2.Text = "unlike";
                            button2.IconUri = new Uri("/Static/AppBarIcons/appbar.social.unlike.rest.png", UriKind.Relative);
                        }
                        );
                    }
                }
            }
        }

        private void LoadComments()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                WebClient clientComments = new WebClient();

                string url = FBHelper.GetPostCommentsURL(App.SelectedPostModel.Id);
                clientComments.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientComments_DownloadStringCompleted);
                clientComments.DownloadStringAsync(new Uri(url, UriKind.Absolute));
            });
        }

        void clientComments_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken resultString = (JToken)responeJObject["data"];
                    JArray resultArray = (JArray)resultString;

                    for (int i = 0; i < resultArray.Count; i++)
                    {
                        JToken postToken = (JToken)resultArray[i];
                        this.ProcessPost(postToken);
                    }

                    //this.comments.Add("Add Comment");
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.lsbComments.Visibility = Visibility.Visible;
                        this.pnlProgress.Visibility = Visibility.Collapsed;
                        this.lsbComments.ItemsSource = this.comments;

                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.lsbComments.Visibility = Visibility.Visible;
                        this.pnlProgress.Visibility = Visibility.Collapsed;
                        //this.lsbComments.ItemsSource = this.comments;
                        MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                    });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.lsbComments.Visibility = Visibility.Visible;
                    this.pnlProgress.Visibility = Visibility.Collapsed;
                    //this.lsbComments.ItemsSource = this.comments;
                    MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);

                });
            }
        }

        private void ProcessPost(JToken postToken)
        {
            if (postToken == null)
            {
                return;
            }

            CommenterViewModel post = new CommenterViewModel();

            JToken fromToken = (JToken)postToken["from"];

            if (fromToken != null)
            {
                post.From = (string)fromToken["name"];
            }
            
            post.Id = (string)postToken["id"];

            post.Message = (string)postToken["message"];

            if (!string.IsNullOrEmpty(post.Message) && post.Message.Length > Constatnts.MESSAGE_LENGTH)
            {
                post.Message = post.Message.Substring(0, Constatnts.MESSAGE_LENGTH) + "...";
            }

            post.Created_time = (string)postToken["created_time"];

            this.comments.Add(post);
        }

        private void BarButtonComment_Click(object sender, EventArgs e)
        {
            //AddCommentUserControl control = this.lsbComments[this.comments.Count - 1] as AddCommentUserControl;

            try
            {
                //ListBoxItem item = this.lsbComments.ItemContainerGenerator.ContainerFromIndex(this.comments.Count - 1) as ListBoxItem;
                //AddCommentUserControl control = FindFirstElementInVisualTree<AddCommentUserControl>(item);

                //TextBox box = FindFirstElementInVisualTree<TextBox>(control);

                string comment = this.txtComment.Text;

                if (!string.IsNullOrEmpty(comment))
                {
                    if (!comment.Equals("add a comment"))
                    {
                        if (App.ViewModel.IsNetWorkAvailable)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                var clientPost = new WebClient();

                                clientPost.UploadStringCompleted += new UploadStringCompletedEventHandler(clientPost_UploadStringCompleted);

                                clientPost.UploadStringAsync(FBHelper.GetPostMessageURI(App.SelectedPostModel.Id), "POST", FBHelper.GetPostParameters(App.AccesToken, comment));
                            });
                        }
                        else
                        {
                            App.HandleNoConnection();
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Please enter a comment and try again");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a comment and try again");
                }
            }
            catch (Exception)
            {
                MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
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

        private void BarButtonLike_Click(object sender, EventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    var clientLike = new WebClient();

                    clientLike.UploadStringCompleted += new UploadStringCompletedEventHandler(clientLike_UploadStringCompleted);

                    if (this.button2.Text.Equals("like"))
                    {
                        clientLike.UploadStringAsync(FBHelper.GetPostLikeURI(App.SelectedPostModel.Id), "POST", FBHelper.GetPostParameters(App.AccesToken));
                    }

                    if (this.button2.Text.Equals("unlike"))
                    {
                        clientLike.UploadStringAsync(FBHelper.GetPostLikeURI(App.SelectedPostModel.Id), "DELETE", FBHelper.GetPostParameters(App.AccesToken));
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
                        if (this.button2.Text.Equals("like"))
                        {
                            MessageBox.Show("You have liked this post!");
                            button2.Text = "unlike";
                            button2.IconUri = new Uri("/Static/AppBarIcons/appbar.social.unlike.rest.png", UriKind.Relative);

                        }

                        else if (this.button2.Text.Equals("unlike"))
                        {
                            MessageBox.Show("You have unliked this post!");
                            button2.Text = "like";
                            button2.IconUri = new Uri("/Static/AppBarIcons/appbar.social.like.rest.png", UriKind.Relative);

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

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtComment.Text))
            {
                this.button1.IsEnabled = true;
            }
            else
            {
                this.button1.IsEnabled = false;
            }
        }

        

    }
}