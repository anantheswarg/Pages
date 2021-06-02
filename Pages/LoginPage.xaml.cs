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
using Facebook;
using System.Text;
using Pages.Helpers;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using Pages.Model;
using System.Diagnostics;
using Pages.Model.Helpers;

namespace Pages
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public string accessToken;

        public string logouturl = string.Empty;

        public const string APP_ID = "452723181428075";
        private const string ExtendedPermissions = "publish_stream";
        //private WebBrowser webBrowser;

        public LoginPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(LoginPage_Loaded);

            //this.webBrowser = new WebBrowser();
            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(webBrowser_LoadCompleted);

            webBrowser.Navigating += new EventHandler<NavigatingEventArgs>(webBrowser_Navigating);
        }

        void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains("https://m.facebook.com/home.php") || e.Uri.AbsoluteUri.Contains("http://m.facebook.com/home.php"))
            {
                webBrowser.Visibility = Visibility.Collapsed;
                //this.ClearLoginDetails();
                //webBrowser.Navigate(this.GetLoginURI(APP_ID));
            }

            if (e.Uri.AbsoluteUri.Contains("https://www.facebook.com/home.php") || e.Uri.AbsoluteUri.Contains("http://www.facebook.com/home.php"))
            {
                webBrowser.Navigate(new Uri("http://m.facebook.com/home.php"));
                webBrowser.Visibility = Visibility.Collapsed;
            }


            if (e.Uri.AbsoluteUri.Contains("https://www.facebook.com/dialog/permissions.request?_fb_noscript=1"))
            {
                webBrowser.Visibility = Visibility.Collapsed;
                if (MessageBox.Show("An unknown server error occurred. Please exit the app and try again later.") == MessageBoxResult.OK)
                {
                    this.ErrorPanel.Visibility = Visibility.Visible;
                }
            }

            if (e.Uri.AbsoluteUri.Contains("https://m.facebook.com/index.php") || e.Uri.AbsoluteUri.Contains("http://m.facebook.com/index.php"))
            {
                webBrowser.Navigate(this.GetLoginURI(APP_ID));
            }

            if (e.Uri.AbsoluteUri.Contains("https://m.facebook.com/logout.php") || e.Uri.AbsoluteUri.Contains("http://m.facebook.com/logout.php"))
            {

            }

            if (e.Uri.AbsoluteUri.Contains("http://m.facebook.com/?refsrc=http%3A%2F%2Fwww.facebook.com"))
            {
                this.ClearLoginDetails();
                webBrowser.Navigate(this.GetLoginURI(APP_ID));
            }
        }


        private void ClearLoginDetails()
        {
            if (App.ViewModel.SearchedPages != null)
            {
                App.ViewModel.SearchedPages.Clear();
            }

            if (App.ViewModel.PagesOld != null)
            {
                App.ViewModel.PagesOld.Clear();
            }

            if (App.ViewModel.PagesNew != null)
            {
                App.ViewModel.PagesNew.Clear();
            }

            if (App.ViewModel.Recent != null)
            {
                App.ViewModel.Recent.Clear();
            }

            if (App.ViewModel.Pages != null)
            {
                App.ViewModel.Pages.Clear();
            }

            App.ViewModel.IsDataLoaded = false;
            App.ViewModel.IsFeaturedDataLoaded = false;
            App.ViewModel.IsRecentDataLoaded = false;

            try
            {
                if (App.ViewModel != null)
                {
                    Utilities.Save<MainViewModel>("mainviewmodel", App.ViewModel);
                }
            }
            catch
            {
                Debug.WriteLine("write error");
            }

            Utilities.RemoveFromISO(Constatnts.ACCESS_TOKEN);

            //App.IsFirstLaunch = true;
            Utilities.WriteToISO<bool>(Constatnts.FIRST_LAUNCH, false);
        }

        void webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains("https://m.facebook.com/home.php") || e.Uri.AbsoluteUri.Contains("http://m.facebook.com/home.php"))
            //if (e.Uri.AbsoluteUri.Contains("https://www.facebook.com/home.php") || e.Uri.AbsoluteUri.Contains("http://www.facebook.com/home.php"))
            {
                string fbLogoutDoc = webBrowser.SaveToString();

                try
                {
                    this.logouturl = string.Format(Constatnts.LOGOUT_PAGE_URL, new Regex("/logout.php?(.*)\\\" data").Matches(fbLogoutDoc)[0].Groups[1].ToString());

                    //Utilities.WriteToISO<string>(Constatnts.LOGOUT_URL, this.logouturl);

                    Utilities.WriteToISO<bool>(Constatnts.FIRST_LAUNCH, false);

                    this.ClearLoginDetails();

                    webBrowser.Navigate(new Uri(this.logouturl, UriKind.Absolute));

                }
                catch
                {
                    MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                }
            }


            //if (e.Uri.AbsoluteUri.Contains("https://www.facebook.com/login.php?api_key=") || e.Uri.AbsoluteUri.Contains("http://www.facebook.com/login.php?api_key="))
            if (e.Uri.AbsoluteUri.Contains("https://m.facebook.com/login.php?app_id=") || e.Uri.AbsoluteUri.Contains("http://m.facebook.com/login.php?app_id="))
            
            {
                this.loginPanel.Visibility = Visibility.Visible;
                this.loggedOutPanel.Visibility = Visibility.Collapsed;

                // home page or logout success page
                this.imgLogin.Source = new BitmapImage(new Uri("/static/images/loginbutton.png", UriKind.Relative));
                this.imgLogin.IsHitTestVisible = true;
                this.LoadingProgressPnl.Visibility = Visibility.Collapsed;
            }


        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {

        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            //if (!string.IsNullOrEmpty(Utilities.ReadFromISO(Constatnts.ACCESS_TOKEN)))
            //{
            //    //this.NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
            //    App.NavigateToPage(PageReferences.MainPage);
            //}
            //else
            //{
            //    //WebBrowser webBrowser = new WebBrowser();
            //}

            string AccessToken = string.Empty;
            if (string.IsNullOrEmpty(App.AccesToken))
            {
                try
                {
                    AccessToken = Utilities.Load<string>(Constatnts.TOKEN_FILE);
                    FBHelper.ACCESS_TOKEN_CURENT = AccessToken;
                }
                catch (Exception ex)
                {
                    EventLogger.Log("Access token could not be loaded.");
                    // file not found
                }
            }

            if (App.IsUserLoggedIn)
            {
                App.NavigateToPage(PageReferences.MainPage);
            }
            else
            {
                if (App.IsFirstLaunch)
                {
                    if (App.ViewModel.IsNetWorkAvailable)
                    {
                        webBrowser.Navigate(this.GetLoginURI(APP_ID));
                        //webBrowser.Navigate(new Uri("http://www.facebook.com/dialog/oauth/?response_type=token&display=touch&scope=publish_stream%2coffline_access&client_id=138948912823371&redirect_uri=http%3a%2f%2fwww.facebook.com%2fconnect%2flogin_success.html&num=23956", UriKind.Absolute));
                    }
                    else
                    {
                        this.LoadingProgressPnl.Visibility = Visibility.Collapsed;
                        this.pnlNoConnection.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    webBrowser.Visibility = Visibility.Collapsed;

                    this.loggedInPanel.Visibility = Visibility.Collapsed;
                    this.loggedOutPanel.Visibility = Visibility.Visible;

                    this.imgLogin.Source = new BitmapImage(new Uri("/static/images/loginbuttondisabled.png", UriKind.Relative));
                    this.imgLogin.IsHitTestVisible = false;

                    if (App.ViewModel.IsNetWorkAvailable)
                    {
                        this.LoadingProgressPnl.Visibility = Visibility.Visible;

                        webBrowser.Navigate(this.GetLoginURI(APP_ID, true));
//                        webBrowser.Navigate(new Uri(string.Format("https://m.facebook.com/logout.php?confirm=1&access_token={0}", App.AccesToken), UriKind.Absolute));
                    }
                    else
                    {
                        this.LoadingProgressPnl.Visibility = Visibility.Collapsed;
                        this.pnlNoConnection.Visibility = Visibility.Visible;
                    }


                    //if (string.IsNullOrEmpty(this.logouturl))
                    //{
                    //    this.logouturl = Utilities.ReadFromISO<string>(Constatnts.LOGOUT_URL);
                    //}

                    //if (!string.IsNullOrEmpty(this.logouturl))
                    //{

                    //    webBrowser.Navigate(new Uri(this.logouturl, UriKind.Absolute));
                    //}
                }
            }

            base.OnNavigatedTo(e);
        }

        private Uri GetLoginURI(string appId, bool logout = false)
        {
            ////Get this from the facebook
            ////string appId = "277785558932803";
            //string appId = "138015002973416";
            
            ////List of all the permissions you want to have access to
            ////You can get the list of possiblities from here http://developers.facebook.com/tools/explorer/
            //string[] extendedPermissions = new[] { "publish_stream", "offline_access" };

            //var oauth = new FacebookOAuthClient { ClientId = appId };
            ////Telling the Facebook that we want token as response
            ////and we are using touch enabled device
            //var parameters = new Dictionary<string, object>
            //        {
            //            { "client_id", appId},
            //            { "response_type", "token" },
            //            { "display", "popup" },
            //            //{ "random", new Random().Next(1000,10000).ToString() },
            //            {"redirect_uri", "http://www.facebook.com/connect/login_success.html"}
            //        };


            ////If there's extended permissions build the string and set it up
            //if (extendedPermissions != null && extendedPermissions.Length > 0)
            //{
            //    var scope = new StringBuilder();
            //    scope.Append(string.Join(",", extendedPermissions));
            //    parameters["scope"] = scope.ToString();
            //}

            ////Create the login url
            //return oauth.GetLoginUrl(parameters);


            //string[] extendedPermissions = new[] { "publish_stream", "offline_access" };

            //var parameters = new Dictionary<string, object>
            //        {
            //            { "client_id", appId},
            //            { "response_type", "token" },
            //            { "display", "touch" },
            //            //{ "random", new Random().Next(1000,10000).ToString() },
            //            {"redirect_uri", "http://www.facebook.com/connect/login_success.html"}
            //        };
            ////If there's extended permissions build the string and set it up
            //if (extendedPermissions != null && extendedPermissions.Length > 0)
            //{
            //    var scope = new StringBuilder();
            //    scope.Append(string.Join(",", extendedPermissions));
            //    parameters["scope"] = scope.ToString();
            //}

            //// add the 'scope' parameter only if we have extendedPermissions.
            //if (!string.IsNullOrWhiteSpace(extendedPermissions))
            //    parameters.scope = extendedPermissions;

            // generate the login url
            var fb = new FacebookClient();

            Uri uri = null;
            var parameters = new Dictionary<string, object>();

            if (!logout)
            {

                parameters["client_id"] = appId;
                parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
                parameters["response_type"] = "token";
                parameters["display"] = "touch";

                // add the 'scope' only if we have extendedPermissions.
                if (!string.IsNullOrEmpty(ExtendedPermissions))
                {
                    // A comma-delimited list of permissions
                    parameters["scope"] = ExtendedPermissions;
                }
           
                uri = fb.GetLoginUrl(parameters);
            }
            else
            {
                parameters["client_id"] = appId;

                uri = fb.GetLogoutUrl(parameters);
            }

            return uri;
        }

        private void imgLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ShowFacebookLogin.Begin();

            webBrowser.Visibility = Visibility.Visible;

            this.loginPanel.Visibility = Visibility.Collapsed;
            this.loggedInPanel.Visibility = Visibility.Visible;
        }


        void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //FacebookOAuthResult result;
            var fb = new FacebookClient();
            FacebookOAuthResult oauthResult;
            if (fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                // The url is the result of OAuth 2.0 authentication
                if (oauthResult.IsSuccess)
                {
                    
                    App.AccesToken = oauthResult.AccessToken;

                    FBHelper.ACCESS_TOKEN_CURENT = oauthResult.AccessToken;

                    Utilities.Save<string>(Constatnts.TOKEN_FILE, App.AccesToken);

                    if (Utilities.WriteToISO(Constatnts.ACCESS_TOKEN, App.AccesToken))
                    {
                        App.IsFirstLaunch = false;
                        Utilities.WriteToISO<bool>(Constatnts.FIRST_LAUNCH, true);

                        EventLogger.Log("User logged in");

                        App.NavigateToPage(PageReferences.MainPage);
                    }
                    else
                    {
                        MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                    }

                    webBrowser.Visibility = System.Windows.Visibility.Collapsed;

                    this.loginPanel.Visibility = Visibility.Collapsed;
                    this.loggedInPanel.Visibility = Visibility.Visible;

                }
                else
                {
                    var errorDescription = oauthResult.ErrorDescription;
                    var errorReason = oauthResult.ErrorReason;
                    MessageBox.Show(ErrorMessages.LOGIN_FAILURE);
                }
            }
            else
            {
                // The url is NOT the result of OAuth 2.0 authentication.
            }

            ////if(e.Uri.AbsoluteUri.StartsWith("http://m.facebook.com/logout.php"))
            ////{
            ////    Utilities.RemoveFromISO(Constatnts.ACCESS_TOKEN);

            ////    App.IsFirstLaunch = true;
            ////    Utilities.WriteToISO<bool>(Constatnts.FIRST_LAUNCH, false);                    
            ////}
            ////else
            ////{
            ////Try because there might be cases when user input wrong password
            //if (FacebookOAuthResult.TryParse(e.Uri.AbsoluteUri, out result))
            //{
            //    if (result.IsSuccess && !string.IsNullOrEmpty(result.AccessToken))
            //    {
            //        accessToken = result.AccessToken;

            //        App.AccesToken = accessToken;
                    
            //        FBHelper.ACCESS_TOKEN_CURENT = accessToken;

            //        if (Utilities.WriteToISO(Constatnts.ACCESS_TOKEN, accessToken))
            //        {
            //            App.IsFirstLaunch = false;
            //            Utilities.WriteToISO<bool>(Constatnts.FIRST_LAUNCH, true);

            //            EventLogger.Log("User logged in");

            //            App.NavigateToPage(PageReferences.MainPage);
            //        }
            //        else
            //        {
            //            MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
            //        }

            //        webBrowser.Visibility = System.Windows.Visibility.Collapsed;

            //        this.loginPanel.Visibility = Visibility.Collapsed;
            //        this.loggedInPanel.Visibility = Visibility.Visible;

            //    }
            //    else
            //    {
            //        var errorDescription = result.ErrorDescription;
            //        var errorReason = result.ErrorReason;

            //        MessageBox.Show(ErrorMessages.LOGIN_FAILURE);
            //    }
            //}
            //else
            //{
            //    //enable image click

            //    //this.imgLogin.Source = new BitmapImage(new Uri("/static/images/loginbutton.png", UriKind.Relative));
            //    //this.imgLogin.IsHitTestVisible = true;
            //    //this.LoadingProgressPnl.Visibility = Visibility.Collapsed;

            //}

            if (e.Uri.AbsoluteUri.Contains("https://www.facebook.com/connect/login_success.html#access_token="))
            {
                EventLogger.Log("User logged in");

                App.NavigateToPage(PageReferences.MainPage);
            }

            //if (!string.IsNullOrEmpty(e.Uri.Fragment))
            //{
            //    if (e.Uri.AbsoluteUri.Replace(e.Uri.Fragment, "") == "http://www.facebook.com/connect/login_success.html")
            //    {
            //        string text = HttpUtility.HtmlDecode(e.Uri.Fragment).TrimStart('#');

            //        var pairs = text.Split('&');

            //        foreach (var pair in pairs)
            //        {
            //            var kvp = pair.Split('=');
            //            if (kvp.Length == 2)
            //            {
            //                if (kvp[0] == "access_token")
            //                {
            //                    //AccessToken = kvp[1];
            //                    MessageBox.Show("Access granted");
            //                }
            //            }
            //        }

            //    }
            //}
            //}     
        }

        private void UIElement_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void UIElement_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

    }
}