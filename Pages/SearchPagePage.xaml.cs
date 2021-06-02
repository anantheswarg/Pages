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
using System.Collections.ObjectModel;
using Pages.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Pages
{
    public partial class SearchPagePage : PhoneApplicationPage
    {
        public ObservableCollection<PageItemModel> SearchedResults { get; set; }

        private bool isAdd = false;

        public SearchPagePage()
        {
            SearchedResults = new ObservableCollection<PageItemModel>();
            this.DataContext = this;
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.txtSearch.Focus();
            base.OnNavigatedTo(e);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isAdd = true;
            Image image = sender as Image;

            if (image != null)
            {
                PageItemModel page = image.DataContext as PageItemModel;

                if (page != null)
                {
                    App.ViewModel.SearchedPages.Add(page);
                    App.ViewModel.PagesOld.Add(page);
                    App.ViewModel.LoadPages();
                    this.NavigationService.GoBack();
                }
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                
                if (!string.IsNullOrEmpty(this.txtSearch.Text))
                {
                    this.SearchedResults.Clear();

                    this.pnlLoading.Visibility = System.Windows.Visibility.Visible;

                    this.lsbSearchResults.Focus();

                    WebClient clientSearch = new WebClient();

                    string url = FBHelper.GetSearchPagesURL(this.txtSearch.Text);

                    clientSearch.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientSearch_DownloadStringCompleted);
                    clientSearch.DownloadStringAsync(new System.Uri(url, System.UriKind.Absolute));
                }
            }
        }

        void clientSearch_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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
                        PageItemModel page = PageItemModel.FromJson(resultArray[i].ToString());
                        this.SearchedResults.Add(page);
                    }
                    //this.lsbSearchResults.ItemsSource = this.SearchedResults;
                }
            }

            this.pnlLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void StackPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };

        }

        private void StackPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
            
        }

        private void lsbSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isAdd)
            {
                PageItemModel model = this.lsbSearchResults.SelectedItem as PageItemModel;
                //model.AccessedTime = DateTime.UtcNow;

                model.Created_time = String.Format("{0:dd MMM yyyy h:mm tt}", DateTime.UtcNow);

                //App.ViewModel.AddRecentPage(model);

                App.NavigateToPage(PageReferences.DetailsPage + string.Format("?ID={0}&TITLE={1}", model.Id, model.Name));
            }
        }
    }
}