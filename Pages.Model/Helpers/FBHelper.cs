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

namespace Pages.Helpers
{
    public class FBHelper
    {
        private static string DEFAULT_URL = "https://graph.facebook.com";

        public static string ACCESS_TOKEN_CURENT;

        public static string GetAllPagesURL()
        {
            //return string.Format(DEFAULT_URL + "/me/likes?access_token={0}&limit=20", ACCESS_TOKEN_CURENT);
            return string.Format(DEFAULT_URL + "/me/likes?access_token={0}", ACCESS_TOKEN_CURENT);
        }

        public static string GetSearchPagesURL(string text)
        {
            //return string.Format(DEFAULT_URL + "/me/likes?access_token={0}&limit=20", ACCESS_TOKEN_CURENT);
            return string.Format(DEFAULT_URL + "/search?q={0}&type=page&access_token={1}", text, ACCESS_TOKEN_CURENT);
        }

        public static string GetImageUrl(string id, string type = "small" )
        {
            return string.Format(DEFAULT_URL + "/{0}/picture?type={1}&access_token={2}",id, type, ACCESS_TOKEN_CURENT);
        }

        public static string GetPageInfoUrl(string id)
        {
            return string.Format(DEFAULT_URL + "/{0}?access_token={1}", id, ACCESS_TOKEN_CURENT);
        }

        public static string GetFeaturedUrl()
        {
            return "http://alphaapps.co.in/data/featured.txt";
        }

        public static string GetCheckLikeUrl(string id)
        {
            return string.Format(DEFAULT_URL + "/me/likes/{0}&access_token={1}", id, ACCESS_TOKEN_CURENT);
        }

        public static string GetListOfAlbumsURL(string id)
        {
            return string.Format(DEFAULT_URL + "/{0}/albums?fields=name&access_token={1}", id, ACCESS_TOKEN_CURENT); 
        }

        public static string GetAlbumPhotosURL(string id)
        {
            return string.Format(DEFAULT_URL + "/{0}/photos?fields=picture,name,source,height,width&access_token={1}&limit=50", id, ACCESS_TOKEN_CURENT);
        }

        public static string GetPostsURL(string id, int index = 0, int limit = 25)
        {
            return string.Format(DEFAULT_URL + "/{0}/posts?fields=id,picture,from,likes,comments,created_time,type,story,message,caption,link,name&access_token={1}&index={2}&limit={3}", id, ACCESS_TOKEN_CURENT, index.ToString(), limit.ToString());
        }

        public static string GetPostCommentsURL(string id)
        {
            return string.Format(DEFAULT_URL + "/{0}/comments?access_token={1}&offset=0&limit=25", id, ACCESS_TOKEN_CURENT);
        }

        public static string GetAlbumNameURL(string id)
        {
            return string.Format(DEFAULT_URL + "/{0}?fields=name&access_token={1}", id, ACCESS_TOKEN_CURENT);
        }


        public static Uri GetPostMessageURI(string id)
        {
            string url = string.Format(DEFAULT_URL +  "/{0}/comments", id);
         
            return new Uri(url, UriKind.Absolute);
        }

        public static Uri GetPostLikeURI(string id)
        {
            string url = string.Format(DEFAULT_URL + "/{0}/likes", id);
        
            return new Uri(url, UriKind.Absolute);
        }

        public static string GetCheckPostLikeUrl(string id)
        {
            return string.Format(DEFAULT_URL + "/fql?q=SELECT user_id FROM like WHERE object_id={0} AND  user_id=me()&access_token={1}", id, ACCESS_TOKEN_CURENT);
        }

        public static string GetPostParameters(string strAccessToken, 
            string message = null,
            string caption = null, 
            string description = null,
            string link = null,
            string name = null,
            string picturelink = null)
        {
            try
            {
                string strRet = "access_token=" + strAccessToken;
                if (!string.IsNullOrEmpty(caption))
                {
                    strRet += "&caption=" + HttpUtility.UrlEncode(caption);
                }
                if (!string.IsNullOrEmpty(description))
                {
                    strRet += "&description=" + HttpUtility.UrlEncode(description);
                }
                if (!string.IsNullOrEmpty(link))
                {
                    strRet += "&link=" + HttpUtility.UrlEncode(link);
                }
                if (!string.IsNullOrEmpty(message))
                {
                    strRet += "&message=" + HttpUtility.UrlEncode(message);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    strRet += "&name=" + HttpUtility.UrlEncode(name);
                }
                if (!string.IsNullOrEmpty(picturelink))
                {
                    strRet += "&picture=" + HttpUtility.UrlEncode(picturelink);
                }
                return (strRet);
            }
            catch { return (""); }
        }

    }
}
