using System;
using System.Net;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Pages.Model.Helpers;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pages.Model
{
    [DataContract]
    public class DetailsViewModel : INotifyPropertyChanged, IEquatable<DetailsViewModel>
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Title { get; set; }

        public event DataLoadedHandler DataLoaded;

        public event DataLoadedHandler PageInfoLoaded;

        public event DataLoadedHandler AlbumsInfoLoaded;


        [DataMember]
        public string NextPostsUrl = string.Empty;

        //public readonly DelegateCommand fetchMorePosts;
        //public ICommand FetchMorePosts
        //{
        //    get
        //    {
        //        return fetchMorePosts;
        //    }
        //}

        [DataMember]
        public int LiveTileCount { get; set; }

        public int OldLiveTileCount { get; set; }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<AlbumViewModel> Albums { get; set; }

        /// <summary>
        /// A collection for PostViewModel objects.
        /// </summary>
        //[DataMember]        
        public ObservableCollection<PostViewModel> Posts { get; set; }

        private bool _moreFloatsDownloading;
        public bool MoreFloatsDownloading
        {
            get
            {
                return _moreFloatsDownloading;
            }
            set
            {
                _moreFloatsDownloading = value;
                NotifyPropertyChanged("MoreFloatsDownloading");
            }
        }

        private PageInfoModel _pageinfo;

        [DataMember]
        public PageInfoModel PageInfo
        {
            get
            {
                return _pageinfo;
            }
            set
            {
                if (value != _pageinfo)
                {
                    _pageinfo = value;
                    NotifyPropertyChanged("PageInfo");
                }
            }
        }

        public DetailsViewModel(string id, string Title)
        {
            this.ID = id;
            this.Title = Title;

            this.IsPageInfoLoaded = false;

            this.Albums = new ObservableCollection<AlbumViewModel>();

            PostsHelper helper = null;
            try
            {
                helper = Utilities.Load<PostsHelper>(this.ID + Constatnts.POSTS_HELPER_NAME);
            }
            catch { }

            if (helper != null)
            {
                this.Posts = helper.Posts;
            }

            if (this.Posts == null)
            {
                this.Posts = new ObservableCollection<PostViewModel>();
            }

            //fetchMorePosts = new DelegateCommand(
            //    obj =>
            //    {
            //        ThreadPool.QueueUserWorkItem(
            //        delegate
            //        {
            //            Deployment.Current.Dispatcher.BeginInvoke(() =>
            //            {

            //                if (MoreFloatsDownloading)
            //                {
            //                    return;
            //                }

            //                MoreFloatsDownloading = true;
            //                this.UpdatePosts(this.Posts.Count, 25);
            //            });
            //        });


            //    });
        }

        private bool _isDataLoaded;

        [DataMember]
        public bool IsDataLoaded
        {
            get
            {
                return _isDataLoaded;
            }
            set
            {
                if (value != _isDataLoaded)
                {
                    _isDataLoaded = value;
                    NotifyPropertyChanged("IsDataLoaded");
                }
            }
        }

        private bool _isPageInfoLoaded;

        [DataMember]
        public bool IsPageInfoLoaded
        {
            get
            {
                return _isPageInfoLoaded;
            }
            set
            {
                if (value != _isPageInfoLoaded)
                {
                    _isPageInfoLoaded = value;
                    NotifyPropertyChanged("IsPageInfoLoaded");
                }
            }
        }

        private bool _isAlbumsInfoLoaded;


        [DataMember]
        public bool IsAlbumsInfoLoaded
        {
            get
            {
                return _isAlbumsInfoLoaded;
            }
            set
            {
                if (value != _isAlbumsInfoLoaded)
                {
                    _isAlbumsInfoLoaded = value;
                    NotifyPropertyChanged("IsAlbumsInfoLoaded");
                }
            }
        }

        #region Get Posts

        public void LoadData()
        {
            // to be filled

            WebClient clientPosts = new WebClient();

            clientPosts.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientPosts_DownloadStringCompleted);
            string url = FBHelper.GetPostsURL(this.ID);

            clientPosts.DownloadStringAsync(new Uri(url, UriKind.Absolute));


        }

        public void UpdatePostsFromISO()
        {
            PostsHelper helper = null;
            try
            {
                helper = Utilities.Load<PostsHelper>(this.ID + Constatnts.POSTS_HELPER_NAME);
            }
            catch { }

            if (helper != null)
            {
                this.Posts = helper.Posts;
            }
        }

        public void UpdatePosts(int index, int limit)
        {
            // to be filled

            WebClient clientPosts = new WebClient();

            clientPosts.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientPosts_DownloadStringCompleted);
            string url = string.Empty;

            if (index != 0)
            {
                if (!string.IsNullOrEmpty(this.NextPostsUrl))
                {
                    url = this.NextPostsUrl;
                    //url = FBHelper.GetPostsURL(this.ID, index, limit);
                }
            }
            else
            {
                url = FBHelper.GetPostsURL(this.ID, index, limit);
            }

            if (!string.IsNullOrEmpty(url))
            {
                clientPosts.DownloadStringAsync(new Uri(url, UriKind.Absolute));
            }


        }

        void clientPosts_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken pagingToken = (JToken)responeJObject["paging"];

                    if (pagingToken != null)
                    {
                        this.NextPostsUrl = (string)pagingToken["next"];
                    }

                    JToken data = (JToken)responeJObject["data"];

                    JArray array = (JArray)data;

                    if (array.Count > 0)
                    {
                        //this.LiveTileCount = 0;
                        for (int i = array.Count - 1; i >= 0; i--)
                        {
                            JToken postToken = (JToken)array[i];
                            this.ProcessPost(postToken);
                        }

                    }

                    this.IsDataLoaded = true;

                    LoadedEventArgs e1 = new LoadedEventArgs("");

                    OnDataLoad((object)this, e1);
                }
            }
            else
            {

                LoadedEventArgs e1 = null;

                if (e.Error.Message.Contains("The remote server returned an error"))
                {
                    e1 = new LoadedEventArgs("Could not connect to the server. Please try later.");
                }

                OnDataLoad((object)this, e1);

            }

            MoreFloatsDownloading = false;
        }

        private void ProcessPost(JToken postToken)
        {
            if (postToken == null)
            {
                return;
            }

            PostViewModel post = new PostViewModel();

            JToken likesToken = (JToken)postToken["likes"];

            if (likesToken != null)
            {
                post.Likes = (int)likesToken["count"];
            }

            JToken commentsToken = (JToken)postToken["comments"];

            if (commentsToken != null)
            {
                post.Comments = (int)commentsToken["count"];
            }

            JToken fromToken = (JToken)postToken["from"];

            if (fromToken != null)
            {
                post.From = (string)fromToken["name"];
            }


            post.Id = (string)postToken["id"];

            if (postToken["picture"] != null)
            {
                post.Picture = (string)postToken["picture"];
            }
            else
            {
                post.Picture = null;
            }

            post.Link = (string)postToken["link"];

            post.Message = (string)postToken["message"];

            if (!string.IsNullOrEmpty(post.Message) && post.Message.Length > Constatnts.MESSAGE_LENGTH)
            {
                post.Message = post.Message.Substring(0, Constatnts.MESSAGE_LENGTH) + "...";
            }

            post.Name = (string)postToken["name"];

            if (!string.IsNullOrEmpty(post.Name) && post.Name.Length > Constatnts.MESSAGE_LENGTH)
            {
                post.Name = post.Name.Substring(0, Constatnts.MESSAGE_LENGTH) + "...";
            }

            post.Story = (string)postToken["story"];

            if (!string.IsNullOrEmpty(post.Story) && post.Story.Length > Constatnts.MESSAGE_LENGTH)
            {
                post.Story = post.Story.Substring(0, Constatnts.MESSAGE_LENGTH) + "...";
            }

            post.Type = (string)postToken["type"];

            post.Created_time = (string)postToken["created_time"];

            var result = from c in this.Posts where c.Id.Equals(post.Id) select c;

            if (result.Count<PostViewModel>() <= 0)
            {
                if (this.Posts.Count > 0)
                {
                    bool isPostAdded = false;
                    DateTime createdtime1;
                    if (DateTime.TryParse(post.Created_time as string, out createdtime1))
                    {
                        DateTime createdtime2;
                        if (DateTime.TryParse(this.Posts[0].Created_time as string, out createdtime2))
                        {
                            if (createdtime1 > createdtime2)
                            {
                                this.Posts.Insert(0, post);
                                isPostAdded = true;
                                this.LiveTileCount++;

                            }
                        }
                    }

                    if (!isPostAdded)
                    {
                        this.Posts.Add(post);
                    }
                }
                else
                {
                    this.Posts.Insert(0, post);
                    this.LiveTileCount++;
                }

            }
        }

        void OnDataLoad(object o, LoadedEventArgs e)
        {
            if (DataLoaded != null)
            {
                DataLoaded(o, e);
            }
        }

        #endregion

        #region Get Albums Info

        public void GetAlbumsInfo()
        {
            WebClient clientAlbums = new WebClient();

            clientAlbums.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientAlbums_DownloadStringCompleted);

            string url = FBHelper.GetListOfAlbumsURL(this.ID);

            clientAlbums.DownloadStringAsync(new Uri(url, UriKind.Absolute));
        }

        void clientAlbums_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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
                        for (int i = array.Count - 1; i >= 0; i--)
                        {
                            AlbumViewModel album = AlbumViewModel.FromJson(array[i].ToString());

                            var result = from c in this.Albums where c.Id.Equals(album.Id) select c;

                            if (result.Count<AlbumViewModel>() <= 0)
                            {
                                this.Albums.Insert(0, album);
                            }
                        }

                    }

                    this.IsAlbumsInfoLoaded = true;

                    LoadedEventArgs e1 = new LoadedEventArgs("");

                    OnAlbumsInforLoaded((object)this, e1);

                }
            }
        }

        void OnAlbumsInforLoaded(object o, LoadedEventArgs e)
        {
            if (AlbumsInfoLoaded != null)
            {
                AlbumsInfoLoaded(o, e);
            }
        }


        #endregion

        #region Get Page Info
        public void GetPageInfo()
        {
            WebClient clientPageInfo = new WebClient();

            string url = FBHelper.GetPageInfoUrl(this.ID);

            clientPageInfo.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientPageInfo_DownloadStringCompleted);
            clientPageInfo.DownloadStringAsync(new System.Uri(url, System.UriKind.Absolute));
        }

        void clientPageInfo_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);

                    this.PageInfo = PageInfoModel.FromJson(responeJObject.ToString());

                    if (!String.IsNullOrEmpty(this.PageInfo.Talking_about_count))
                    {
                        this.PageInfo.Talking_about_count += " talking about this";
                    }

                    if (!String.IsNullOrEmpty(this.PageInfo.Likes))
                    {
                        WebClient clientCheckLike = new WebClient();

                        string url = FBHelper.GetCheckLikeUrl(this.PageInfo.Id);

                        clientCheckLike.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientCheckLike_DownloadStringCompleted);
                        clientCheckLike.DownloadStringAsync(new Uri(url, UriKind.Absolute));
                    }
                }
            }
        }

        void clientCheckLike_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);

                    JToken data = (JToken)responeJObject["data"];

                    JArray array = (JArray)data;

                    if (array.Count != 0)
                    {
                        this.PageInfo.Likes = "you and " + (Convert.ToInt32(this.PageInfo.Likes) - 1) + " like this";
                    }
                    else
                    {
                        this.PageInfo.Likes += " like this";
                    }
                }
            }

            LoadedEventArgs e1 = new LoadedEventArgs("");

            this.IsPageInfoLoaded = true;


            OnPageInforLoaded((object)this, e1);
        }

        void OnPageInforLoaded(object o, LoadedEventArgs e)
        {
            if (PageInfoLoaded != null)
            {
                PageInfoLoaded(o, e);
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(DetailsViewModel o)
        {
            return o.ID.Equals(ID);
        }

    }
}
