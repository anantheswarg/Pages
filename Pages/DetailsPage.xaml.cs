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
using Pages.Helpers;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using System.Collections;
using Pages.Controls;
using Pages.Model.Helpers;

namespace Pages
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private string ID = string.Empty;

        private DetailsViewModel detailsViewModel;

        private Object pageDataLock = new Object();

        
        public DetailsPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DetailsPage_Loaded);
        }

        #region Post Scrolling
        void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {

            
        }


        
        #endregion


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                this.CntlPosts.isLinkSelected = false;
                this.CntlPosts.isImageSelected = false;
                this.CntlPosts.isLinkOpened = false;

                if (this.detailsViewModel == null)
                {
                    if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["ID"]))
                    {
                        this.ID = this.NavigationContext.QueryString["ID"];
                    }

                    if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["TITLE"]))
                    {
                        this.pvtDetails.Title = this.NavigationContext.QueryString["TITLE"].ToUpper();
                    }

                    string id = App.PinnedPageIDData.PinnedPageIDs.FirstOrDefault<string>(t => t.Equals(this.ID));

                    if (!string.IsNullOrEmpty(id))
                    {
                        DetailsViewModel model = App.PinnedPagesData.PinnedPages.FirstOrDefault<DetailsViewModel>(t => t.ID.Equals(this.ID));

                        if (model == null)
                        {
                            this.detailsViewModel = new DetailsViewModel(this.ID, this.pvtDetails.Title.ToString());
                            App.CurrentDetailsModel = this.detailsViewModel;

                            LittleWatson.CheckForPreviousException();
                            LittleWatsonBakground.CheckForPreviousException();

                            if (App.ViewModel.IsNetWorkAvailable)
                            {
                                txtLoadingPosts.Visibility = Visibility.Visible;
                                    
                                this.LoadModelData();
                            }
                            else
                            {
                                App.HandleNoConnection();
                            }
                        }
                        else
                        {
                            model.UpdatePostsFromISO();

                            this.detailsViewModel = model;

                            App.CurrentDetailsModel = this.detailsViewModel;

                            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + model.ID));

                            if (TileToFind != null)
                            {
                                StandardTileData NewTileData = new StandardTileData
                                {
                                    BackContent = model.Title,
                                    Count = 0,
                                    BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", model.ID), UriKind.Absolute)
                                    
                                };
                                TileToFind.Update(NewTileData);
                            }
                            else
                            {
 
                            }

                            model.LiveTileCount = 0;
                            //this.DataContext = detailsViewModel;
                            if (App.ViewModel.IsNetWorkAvailable)
                            {
                                if (!this.detailsViewModel.IsDataLoaded)
                                {
                                    txtLoadingPosts.Visibility = Visibility.Visible;
                                }

                                this.UpdateModelData();
                            }                            
                            else
                            {
                                App.HandleNoConnection();
                            }

                        }
                    }
                    else
                    {
                        this.detailsViewModel = new DetailsViewModel(this.ID, this.pvtDetails.Title.ToString());
                        App.CurrentDetailsModel = this.detailsViewModel;

                        if (App.ViewModel.IsNetWorkAvailable)
                        {
                            txtLoadingPosts.Visibility = Visibility.Visible;

                            this.LoadModelData();
                        }
                        else
                        {
                            App.HandleNoConnection();
                        }

                    }

                    this.DataContext = detailsViewModel;
                }

            }
            catch(Exception ex)
            {
                txtLoadingPosts.Visibility = Visibility.Visible;

                this.LoadModelData();
                Debug.WriteLine("Crashed in navigated to");
            }

            base.OnNavigatedTo(e);
        }

        private void LoadModelData()
        {
            
            detailsViewModel.DataLoaded += new DataLoadedHandler(detailsViewModel_DataLoad);

            this.progressBarNew.Visibility = Visibility.Visible;

            this.detailsViewModel.UpdatePosts(0, 25);


            //this.detailsViewModel.LoadData();
        }

        private void UpdateModelData()
        {
            this.LoadModelData();
            this.GetAlbumsInfo();
            this.LoadPageInfo();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            //bool isFound = false;

            try
            {
                if (this.detailsViewModel != null && App.PinnedPageIDData.PinnedPageIDs != null)
                {
                    string id = App.PinnedPageIDData.PinnedPageIDs.FirstOrDefault<string>(t => t.Equals(this.detailsViewModel.ID));

                    if (!string.IsNullOrEmpty(id))
                    {
                        DetailsViewModel model = App.PinnedPagesData.PinnedPages.FirstOrDefault<DetailsViewModel>(t => t.ID.Equals(this.detailsViewModel.ID));
                        
                        this.detailsViewModel.IsPageInfoLoaded = false;
                        this.detailsViewModel.IsAlbumsInfoLoaded = false;
                            
                        if (model == null)
                        {
                            App.PinnedPagesData.PinnedPages.Add(this.detailsViewModel);
                                                        
                        }
                        else
                        {
                            //int index = 0;
                            //foreach (PostViewModel post in model.Posts)
                            //{
                            //    if (index++ >= 25)
                            //    {
                            //        model.Posts.Remove(post);
                            //    }
                            //}

                            while (model.Posts.Count > 25)
                            {
                                model.Posts.RemoveAt(25);
                            }

                            App.PinnedPagesData.PinnedPages.Remove(model);
                            
                            App.PinnedPagesData.PinnedPages.Add(this.detailsViewModel);                            
                        }

                        lock (pageDataLock)
                        {
                            try
                            {
                                if (App.PinnedPagesData != null)
                                {
                                    Utilities.Save<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE, App.PinnedPagesData);

                                    PostsHelper helper = new PostsHelper(this.detailsViewModel.ID, this.detailsViewModel.Title);

                                    helper.Posts = this.detailsViewModel.Posts;
                                    helper.LiveTileCount = 0;
                                    
                                    Utilities.Save<PostsHelper>(this.detailsViewModel.ID + Constatnts.POSTS_HELPER_NAME, helper);

                                    EventLogger.Log("PinnedPagesData Saved");
                                }
                            }
                            catch
                            {
                                EventLogger.Log("Error in saving application data");
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine("exception in on navigated from");
            }

                
            base.OnNavigatedFrom(e);
        }

        void detailsViewModel_DataLoad(object o, LoadedEventArgs e)
        {
            // to be filled
            this.detailsViewModel.LiveTileCount = 0;
            this.txtLoadingPosts.Visibility = Visibility.Collapsed;
            this.progressBarNew.Visibility = Visibility.Collapsed;

            //App.PostUserControl.HookUpScroll();

            this.detailsViewModel.MorePostsDownloading = false;

            if (!this.detailsViewModel.IsDataLoaded)
            {
                if (!string.IsNullOrEmpty(e.message))
                {
                    MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
                }
            }
        }

        private void pvtDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.pvtDetails.SelectedIndex == 2)
                {
                    if (Utilities.IsInternetConnectionEnabled())
                    {
                        if (!this.detailsViewModel.IsPageInfoLoaded)
                        {
                            this.LoadPageInfo();
                        }
                    }
                }
                if (this.pvtDetails.SelectedIndex == 1)
                {
                    if (Utilities.IsInternetConnectionEnabled())
                    {
                        if (!this.detailsViewModel.IsAlbumsInfoLoaded)
                        {
                            this.GetAlbumsInfo();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        #region AlbumsInfo

        private void GetAlbumsInfo()
        {
            if (this.detailsViewModel != null )
            {
                this.progressBarNew.Visibility = Visibility.Visible;

                this.detailsViewModel.AlbumsInfoLoaded += new DataLoadedHandler(detailsViewModel_AlbumsInfoLoaded);
                if(!this.detailsViewModel.IsAlbumsInfoLoaded)
                {
                    this.txtLoadingAlbums.Visibility = Visibility.Visible;
                }

                this.detailsViewModel.GetAlbumsInfo();
            }
        }

        void detailsViewModel_AlbumsInfoLoaded(object o, LoadedEventArgs e)
        {
            this.progressBarNew.Visibility = Visibility.Collapsed;
            this.txtLoadingAlbums.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region PageInfo

        private void LoadPageInfo()
        {
            if (this.detailsViewModel != null)
            {
                this.progressBarNew.Visibility = Visibility.Visible;

                this.detailsViewModel.PageInfoLoaded += new DataLoadedHandler(detailsViewModel_PageInfoLoaded);
                    
                if (!this.detailsViewModel.IsPageInfoLoaded)
                {
                    this.txtInfoLoading.Visibility = Visibility.Visible;
                }
                this.detailsViewModel.GetPageInfo();
                
            }
        }

        void detailsViewModel_PageInfoLoaded(object o, LoadedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.detailsViewModel.PageInfo.Description))
                {
                    this.pnlDescription.Visibility = Visibility.Visible;
                }

                if (!string.IsNullOrEmpty(this.detailsViewModel.PageInfo.About))
                {
                    this.pnlAbout.Visibility = Visibility.Visible;
                }

                if (!string.IsNullOrEmpty(this.detailsViewModel.PageInfo.Website))
                {
                    this.pnlWebSite.Visibility = Visibility.Visible;

                    this.pnlWebSite.Children.Clear();

                    TextBlock text = new TextBlock();
                    text.Text = "website";
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.Margin = new Thickness(0,10,0,10);
                    text.FontSize = 26;
                    text.Width = 450;
                    text.TextWrapping = TextWrapping.Wrap;

                    this.pnlWebSite.Children.Add(text);

                    this.AddHyperLinks(this.detailsViewModel.PageInfo.Website);

                }


                this.progressBarNew.Visibility = Visibility.Collapsed;
                this.txtInfoLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void AddHyperLinks(string websitestring)
        {
            if (!string.IsNullOrEmpty(websitestring))
            {
                foreach (var word in websitestring.Split(' '))
                {
                    if (word.StartsWith("www.") || word.StartsWith("http://"))
                    {
                        HyperlinkButton link = new HyperlinkButton { Content = word, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, Width = 450 };

                        
                        //TextBlock text = new TextBlock();
                             
                        //text.Text = word;
                        //text.Width = 450;

                        //link.Content = text;
                        //
                        //link.Style = this.Resources["HyperlinkButtonStyle"] as Style;
                        link.Foreground = new SolidColorBrush(Colors.Blue);
                        link.Click += new RoutedEventHandler(link_Click);
                        //link.Width = 440;
                        
                        if (word.StartsWith("www."))
                        {
                            link.Tag = "http://" + word;
                        }
                        else
                        {
                            link.Tag = word;
                        }

                        this.pnlWebSite.Children.Add(link);
                    }
                    else
                    {
                        TextBlock text = new TextBlock { Text = word, Foreground= new SolidColorBrush(Colors.Blue) };

                        this.pnlWebSite.Children.Add(text);
                    }
                }
            }
        }

        void link_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = sender as HyperlinkButton;

            WebBrowserTask task = new WebBrowserTask();

            task.Uri = new Uri((string)link.Tag, UriKind.Absolute);

            task.Show();
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.lsbAlbums.SelectedIndex != -1)
                {
                    if (App.ViewModel.IsNetWorkAvailable)
                    {
                        AlbumViewModel album = e.AddedItems[0] as AlbumViewModel;

                        //if (!isLinkOpened)
                        //{
                        this.lsbAlbums.SelectedIndex = -1;
                        App.NavigateToPage(PageReferences.AlbumPhotosPage + string.Format("?NAME={0}&ID={1}", album.Name, album.Id));
                        //}
                    }
                }
            }
            catch (Exception )
            {
                MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
            }

            this.lsbAlbums.SelectedIndex = -1;

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