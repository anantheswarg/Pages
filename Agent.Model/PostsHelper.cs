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
using System.Linq;

namespace Pages.Model.Helpers
{
    public delegate void DataLoadedHandler(object o, LoadedEventArgs e);


    /// <summary>
    /// EventArg -> necesary to do corect your job
    /// </summary>
    public class LoadedEventArgs : EventArgs
    {
        public readonly string message;
        public Exception exception;

        public LoadedEventArgs(string s, Exception exception = null)
        {
            this.message = s;
            this.exception = exception;
        }
    }

    [DataContract]
    public class PostsHelper
    {
        private static string DEFAULT_URL = "https://graph.facebook.com";

        public static string ACCESS_TOKEN_CURENT;

        public static int MESSAGE_LENGTH = 250;

        /// <summary>
        /// A collection for PostViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<PostViewModel> Posts { get; set; }

        public event DataLoadedHandler DataLoaded;

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public int LiveTileCount { get; set; }

        public int OldLiveTileCount { get; set; }

        public PostsHelper(string ID, string title)
        {
            this.ID = ID;
            this.Title = title;

            this.Posts = new ObservableCollection<PostViewModel>();
        }

        private string GetPostsURL(string id, int index = 0, int limit = 25)
        {
            return string.Format(DEFAULT_URL + "/{0}/posts?fields=id,picture,from,likes,comments,created_time,type,story,message,caption,link,name&access_token={1}&index={2}&limit={3}", id, FBHelper.ACCESS_TOKEN_CURENT, index.ToString(), limit.ToString());
        }

        public void LoadData()
        {
            // to be filled

            WebClient clientPosts = new WebClient();

            clientPosts.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientPosts_DownloadStringCompleted);
            string url = this.GetPostsURL(this.ID);

            clientPosts.DownloadStringAsync(new Uri(url, UriKind.Absolute));


        }

        void clientPosts_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    try
                    {
                        JObject responeJObject = (JObject)JsonConvert.DeserializeObject(e.Result);

                        JToken pagingToken = (JToken)responeJObject["paging"];

                        //if (pagingToken != null)
                        //{
                        //    this.NextPostsUrl = (string)pagingToken["next"];
                        //}

                        JToken data = (JToken)responeJObject["data"];

                        JArray array = (JArray)data;

                        if (array.Count > 0)
                        {
                            for (int i = array.Count - 1; i >= 0; i--)
                            {
                                JToken postToken = (JToken)array[i];
                                this.ProcessPost(postToken);
                            }

                        }

                    }
                    catch
                    {

                    }

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
        }

        void OnDataLoad(object o, LoadedEventArgs e)
        {
            if (DataLoaded != null)
            {
                DataLoaded(o, e);
            }
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

            if (!string.IsNullOrEmpty(post.Message) && post.Message.Length > MESSAGE_LENGTH)
            {
                post.Message = post.Message.Substring(0, MESSAGE_LENGTH) + "...";
            }

            post.Name = (string)postToken["name"];

            if (!string.IsNullOrEmpty(post.Name) && post.Name.Length > MESSAGE_LENGTH)
            {
                post.Name = post.Name.Substring(0, MESSAGE_LENGTH) + "...";
            }

            post.Story = (string)postToken["story"];

            if (!string.IsNullOrEmpty(post.Story) && post.Story.Length > MESSAGE_LENGTH)
            {
                post.Story = post.Story.Substring(0, MESSAGE_LENGTH) + "...";
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
    }
}
