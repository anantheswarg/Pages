using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Pages.Helpers;

namespace Pages.Model
{
    [DataContract]
    public class AlbumPhotosViewModel: ViewModelBase
    {
        public event DataLoadedHandler OnPhotosLoaded;

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<PhotoViewModel> Photos { get; set; }

        public AlbumPhotosViewModel()
        {
            this.Photos = new ObservableCollection<PhotoViewModel>();
        }

        public void LoadData(string id)
        {
            WebClient clientPhotos = new WebClient();

            clientPhotos.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientPhotos_DownloadStringCompleted);

            string url = FBHelper.GetAlbumPhotosURL(id);

            clientPhotos.DownloadStringAsync(new Uri(url, UriKind.Absolute));
        }

        void clientPhotos_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken data = (JToken)responeJObject["data"];

                    JArray array = (JArray)data;

                    if (array.Count > 0)
                    {
                        for (int i = 0; i < array.Count; i++)
                        {
                            PhotoViewModel photo = PhotoViewModel.FromJson(array[i].ToString());
                            this.Photos.Add(photo);
                        }
                    }

                    LoadedEventArgs e1 = new LoadedEventArgs("");

                    OnPhotosInfoLoaded((object)this, e1);
                }
            }
        }

        void OnPhotosInfoLoaded(object o, LoadedEventArgs e)
        {
            if (OnPhotosLoaded != null)
            {
                OnPhotosLoaded(o, e);
            }
        }
    }
}
