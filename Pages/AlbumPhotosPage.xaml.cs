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
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Pages.Model;
using Pages.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Pages
{
    public partial class AlbumPhotosPage : PhoneApplicationPage
    {
        private AlbumPhotosViewModel viewModel;

        private string albumId = string.Empty;

        public AlbumPhotosPage()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.viewModel == null)
            {
                if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["ID"]))
                {
                    albumId = this.NavigationContext.QueryString["ID"];
                }

                if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["NAME"]))
                {
                    this.txtAlbumName.Text = this.NavigationContext.QueryString["NAME"].ToUpper();
                }

                this.viewModel = new AlbumPhotosViewModel();

                if(App.ViewModel.IsNetWorkAvailable)                
                {                
                    this.viewModel.LoadData(albumId);
                    this.viewModel.OnPhotosLoaded += new DataLoadedHandler(viewModel_OnPhotosLoaded);
                }
                else
                {
                    App.HandleNoConnection();
                }

                this.DataContext = viewModel;
            }

            base.OnNavigatedTo(e);
        }

        void viewModel_OnPhotosLoaded(object o, LoadedEventArgs e)
        {
            this.txtLoadingPhotos.Visibility = Visibility.Collapsed;
            this.progressBarNew.Visibility = Visibility.Collapsed;
        }

        private void lsbPhotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.lsbPhotos.SelectedIndex != -1)
                {
                    PhotoViewModel photo = e.AddedItems[0] as PhotoViewModel;

                    App.NavigateToPage(PageReferences.PhotoPage + string.Format("?ALBUMNAME={0}&PHOTOID={1}&ALBUMID={2}", this.txtAlbumName.Text, photo.Id ,albumId));
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.lsbPhotos.SelectedIndex = -1;
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