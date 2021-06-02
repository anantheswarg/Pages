
#define DEBUG_AGENT

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
using Bewise.Phone;
using Microsoft.Phone.Shell;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;
using Pages.Model.Helpers;


namespace Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool isLaunch = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            App.ViewModel.IsFeaturedDataLoaded = false;

            if (App.ViewModel.Featured != null && App.ViewModel.Featured.Count > 0)
            {
                App.ViewModel.Featured.Clear();
            }

            this.progressBarFeatured.Visibility = Visibility.Collapsed;

            App.ViewModel.FeaturedDataLoaded += new DataLoadedHandler(ViewModel_FeaturedDataLoaded);

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void ViewModel_FeaturedDataLoaded(object o, LoadedEventArgs e)
        {
            this.progressBarFeatured.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Utilities.ReadFromISO(Constatnts.PNM_INDEX)))
            //{
            //    this.PnmPages.DefaultItem = this.PnmPages.Items[Convert.ToInt32(Utilities.ReadFromISO(Constatnts.PNM_INDEX))];
            //}

            System.Diagnostics.Debug.WriteLine("OnNavigatedTo mainpage");

            if (App.ViewModel.Pages != null && App.ViewModel.Pages.Count > 0)
            {
                this.txtNoAllPosts.Visibility = Visibility.Collapsed;
            }

            this.longListSelectorVaultEntries.SelectedItem = null;

            base.OnNavigatedTo(e);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (isLaunch)
            {
                LittleWatson.CheckForPreviousException();
                LittleWatsonBakground.CheckForPreviousException();

                App.StartAgent();
        
                if (!App.ViewModel.IsDataLoaded)
                {
                    this.pnlProgress.Visibility = Visibility.Visible;
                    App.ViewModel.DataLoaded += new DataLoadedHandler(ViewModel_DataLoaded);

                    if (App.ViewModel.IsNetWorkAvailable)
                    {
                        //App.ViewModel.DataLoaded += new DataLoadedHandler(ViewModel_DataLoaded);
                        App.ViewModel.LoadData();
                    }
                    else
                    {
                        App.HandleNoConnection();
                    }
                }
                else
                {
                    App.ViewModel.DataLoaded += new DataLoadedHandler(ViewModel_DataLoaded);

                    App.ViewModel.UpdateData();
                }

                //if (App.ViewModel.Featured.Count == 0)
                //{
                //    App.ViewModel.LoadFeatured();
                //}

                isLaunch = false;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.NavigationService.BackStack.Count() > 0)
            {
                var a = this.NavigationService.BackStack.ElementAt(0);

                if (a.Source.ToString().Contains("/LoginPage.xaml") == true)
                {
                    this.NavigationService.RemoveBackEntry();
                }
            }

            //Utilities.WriteToISO(Constatnts.PNM_INDEX, this.PnmPages.SelectedIndex.ToString());
        }
        
        private void TileButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                TileButton tile = sender as TileButton;

                PageItemModel model = tile.DataContext as PageItemModel;

                model.Created_time = String.Format("{0:dd MMM yyyy h:mm tt}", DateTime.UtcNow);
                App.ViewModel.AddRecentPage(model);

                App.NavigateToPage(PageReferences.DetailsPage + string.Format("?ID={0}&TITLE={1}", model.Id, model.Name));
            }
            else
            {
                MessageBox.Show(ErrorMessages.NETWORK_FAILURE);
            }
        }


        private void TileButton_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TileButton tile = sender as TileButton;

            PageItemModel model = tile.DataContext as PageItemModel;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                MenuItem item = sender as MenuItem;

                PageItemModel model = item.DataContext as PageItemModel;

                string id = App.PinnedPageIDData.PinnedPageIDs.FirstOrDefault<string>(t => t.Equals(model.Id));

                if (string.IsNullOrEmpty(id))
                {
                    App.PinnedPageIDData.PinnedPageIDs.Add(model.Id);

                    try
                    {
                        Utilities.Save<PinnedPageIDData>(Constatnts.PINNED_PAGES_ID_FILE, App.PinnedPageIDData);
                    }
                    catch
                    {
                        EventLogger.Log("New Page pinned but error saving page IDs data");
                        Debug.WriteLine("write error");
                    }

                    try
                    {
                        Utilities.Save<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE, App.PinnedPagesData);
                    }
                    catch
                    {
                        EventLogger.Log("New Page pinned but error saving pages data");
                        Debug.WriteLine("write error");
                    }

                    this.progressBarNew.Visibility = Visibility.Visible;
                    //this.pnlProgress.Visibility = Visibility.Visible;

                    this.DownloadTile(model);

                }
                else
                {
                    MessageBox.Show("You already have this page pinned.");
                }

            }
            else
            {
                MessageBox.Show(ErrorMessages.NETWORK_FAILURE);
            }
        }

        private void DownloadTile(PageItemModel model)
        {
            //var wc = new WebClient();
            //wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            // start the caching call (web async)
            //wc.OpenReadAsync(new Uri(model.ImageURLLarge), model);

            Uri imageUri = new Uri(model.ImageURLLarge, UriKind.Absolute);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.UriSource = imageUri;

            bitmapImage.ImageOpened += (sender, e) =>
            {
            //All transformation code goes here
                var aspect = Math.Max(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                var ratio = (decimal)aspect / (decimal)173;
                var width = decimal.ToInt32(bitmapImage.PixelWidth / ratio);
                var height = decimal.ToInt32(bitmapImage.PixelHeight / ratio);

                WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);
                var resized = writeableBitmap.Resize(width, height, WriteableBitmapExtensions.Interpolation.Bilinear);

                if (Utilities.WriteTileToISO(model.Id, resized))
                {
                    ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(string.Format("Id={0}", model.Id)));

                    StandardTileData tile = new StandardTileData();

                    if (SecondaryTile == null)
                    {
                        tile.BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", model.Id), UriKind.Absolute);

                        //tile.Title = model.Name;
                        //                tile.BackBackgroundImage = new Uri("images/darthvader.png", UriKind.Relative);
                        tile.BackContent = model.Name;

                        try
                        {
                            App.SaveData();
                            ShellTile.Create(new Uri(string.Format("/DetailsPage.xaml?ID={0}&TITLE={1}", model.Id, model.Name), UriKind.Relative), tile);
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show("Unable to create your tile.");
                        }
                    }
                    else
                    {
                        this.progressBarNew.Visibility = Visibility.Collapsed;
                
                        MessageBox.Show("You already have this page pinned.");
                    }
                }

            };

            bitmapImage.ImageFailed += (sender, e) =>
            {
            //All code regarding image download failure goes here
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    { 
                        MessageBox.Show("Unable to create your tile."); 
                    });
            };

        }



        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                try
                {
//                    WriteableBitmap WriteableBMP = new WriteableBitmap(173, 173);
                    WriteableBitmap WriteableBMP = new WriteableBitmap(172, 172);

                    WriteableBMP.LoadJpeg(e.Result);

                    WriteableBMP = WriteableBMP.Resize(173, 173, WriteableBitmapExtensions.Interpolation.Bilinear);

                    PageItemModel model = (PageItemModel)e.UserState;

                    if (Utilities.WriteTileToISO(model.Id, WriteableBMP))
                    {
                        ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(string.Format("Id={0}", model.Id)));

                        StandardTileData tile = new StandardTileData();

                        if (SecondaryTile == null)
                        {
                            tile.BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", model.Id), UriKind.Absolute);

                            //tile.Title = model.Name;
                            //                tile.BackBackgroundImage = new Uri("images/darthvader.png", UriKind.Relative);
                            tile.BackContent = model.Name;

                            try
                            {
                                ShellTile.Create(new Uri(string.Format("/DetailsPage.xaml?ID={0}&TITLE={1}", model.Id, model.Name), UriKind.Relative), tile);
                            }
                            catch (InvalidOperationException)
                            {
                                MessageBox.Show("Unable to create your tile.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("You already have this page pinned.");
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Unable to create your tile.");
                }
            }
        }

        private void longListSelectorVaultEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                if (this.longListSelectorVaultEntries.SelectedItem != null)
                {
                    LongListSelector list = sender as LongListSelector;

                    PageItemModel model = list.SelectedItem as PageItemModel;
                    //model.AccessedTime = DateTime.UtcNow;

                    model.Created_time = String.Format("{0:dd MMM yyyy h:mm tt}", DateTime.UtcNow);

                    App.ViewModel.AddRecentPage(model);

                    App.NavigateToPage(PageReferences.DetailsPage + string.Format("?ID={0}&TITLE={1}", model.Id, model.Name));
                }
            }
            else
            {
                App.HandleNoConnection();
            }
        }

        private void barSearch_Click(object sender, EventArgs e)
        {
            App.NavigateToPage(PageReferences.SearchPagePage);
        }

        private void barRefresh_Click(object sender, EventArgs e)
        {
            if (App.ViewModel.IsNetWorkAvailable)
            {
                App.ViewModel.Pages = null;
                App.ViewModel.Recent = null;
                App.ViewModel.IsDataLoaded = false;

                Utilities.Deletefile(Constatnts.MAINVIEWMODEL_FILE);

                App.ViewModel = null;

                //App.ViewModel.Pages = null;
                //App.ViewModel.PagesOld = null;

                //App.ViewModel.PagesNew = null;

                //App.ViewModel.Recent = null;

                //App.ViewModel.IsDataLoaded = false;


                App.ViewModel.LoadFeatured();

                DataContext = App.ViewModel;

                this.pnlProgress.Visibility = Visibility.Visible;

                this.ApplicationBar = null;

                App.ViewModel.DataLoaded += new DataLoadedHandler(ViewModel_DataLoaded);

                App.ViewModel.LoadData();
            }
            else
            {
                App.HandleNoConnection();
            }

        }

        void ViewModel_DataLoaded(object o, LoadedEventArgs e)
        {
            this.pnlProgress.Visibility = Visibility.Collapsed;

            if (App.ViewModel.IsDataLoaded)
            {
                if (App.ViewModel.PagesOld != null && App.ViewModel.PagesOld.Count == 0)
                {
                    this.txtNoAllPosts.Visibility = Visibility.Visible;
                }
                // needed to be changed. Dont forget
                //App.StartAgent();
            }
            else
            {
                if (!string.IsNullOrEmpty(e.message))
                {
                    MessageBox.Show(e.message);
                }
            }

            //ApplicationBar = new ApplicationBar();

            //ApplicationBar.Mode = ApplicationBarMode.Default;
            //ApplicationBar.Opacity = 1.0;
            //ApplicationBar.IsVisible = true;
            //ApplicationBar.IsMenuEnabled = true;

            //ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            //button1.IconUri = new Uri(Utilities.GetImagePath("refresh.png"), UriKind.Relative);
            //button1.Text = "refresh";
            //ApplicationBar.Buttons.Add(button1);
            //button1.Click+=new EventHandler(barRefresh_Click);

            //ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            //button2.IconUri = new Uri(Utilities.GetImagePath("search.png"), UriKind.Relative);
            //button2.Text = "search";
            //ApplicationBar.Buttons.Add(button2);
            //button2.Click += new EventHandler(barSearch_Click);


            //ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            //menuItem1.Text = "logout";
            //ApplicationBar.MenuItems.Add(menuItem1);
            
            //menuItem1.Click+=new EventHandler(menuLogout_Click);

            //this.ApplicationBar = ApplicationBar;
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
        
        //    System.IO.IsolatedStorage.IsolatedStorageSettings _appSetting =
        //System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
        //    _appSetting.Clear();
        //    _appSetting.Save();

            App.IsUserLoggedIn = false;

            this.NavigationService.GoBack();
        }

        private void menuReview_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void menuFeedback_Click(object sender, EventArgs e)
        {
            var devicename = string.Empty;
            object result;
            if (DeviceExtendedProperties.TryGetValue("DeviceName", out result))
            {
                devicename = result.ToString();
            }

            var task = new EmailComposeTask();
            task.To = "feedback@alphaapps.co.in";
            task.Subject = string.Format("Hey AlphaApps! Here is feedback on Pages through my WP7 device ({0})", devicename);
            task.Show();
        }

        private void menuShowLog_Click(object sender, EventArgs e)
        {
            MessageBox.Show(EventLogger.GetLog());
        }

        private void menuClearLog_Click(object sender, EventArgs e)
        {
            try
            {
                EventLogger.ClearLog();
            }
            catch
            {
                
            }
        }

        private void PnmPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PnmPages.SelectedIndex == 1)
            {
                if (App.ViewModel.IsDataLoaded)
                {
                    if (!App.ViewModel.IsRecentDataLoaded)
                    {
                        App.ViewModel.LoadRecentPages();

                        if (App.ViewModel.Recent.Count == 0)
                        {
                            this.txtNoRecentPages.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }
            else if (this.PnmPages.SelectedIndex == 2)
            {
                if (!App.ViewModel.IsFeaturedDataLoaded)
                {
                    
                    this.progressBarFeatured.Visibility = Visibility.Visible;
                    App.ViewModel.LoadFeatured();
                }
            }
        }

        private void UIElement_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void UIElement_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }


        //private readonly Wp7Theme theme;

        //var resource = Application.Current.Resources["PhoneForegroundColor"];
        //if (resource != null) {
        //this.theme = (Color)resource == Color.FromArgb(222, 0, 0, 0) ? Wp7Theme.Light : Wp7Theme.Dark;
        //}
        //else {
        //this.theme = Wp7Theme.Dark;
        //}
        //}
        //  public string SearchIcon {
        //get {
        //return GetImagePath("appbar.feature.search.rest.png");
        //}
        //}       private string GetImagePath(string fileName) {
        //var folder = this.theme == Wp7Theme.Dark ? "Images/dark/" : "Images/light/";
        //return Path.Combine(folder, fileName);
        //}
        ////private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        ////{

        ////}
    }
}