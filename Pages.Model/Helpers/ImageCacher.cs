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
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Pages.Helpers
{
    /// <summary>
    /// Caches web images
    /// </summary>
    public class ImageCacher
    {
        public static string type = string.Empty;

        // Just an overload
        public static BitmapImage GetCacheImage(Uri uri)
        {
            return GetCacheImage(uri.OriginalString);
        }

        //public string GetLargeImagePath(string url)
        //{
        //    if (!url.StartsWith("https://"))
        //        throw new ArgumentException("ImageCacher only works with http:// images", url);

        //    string filename = url.Substring(27, 15);

        //    //if (type.Equals("large"))
        //    //{
        //        filename += "large";
        //    //}

        //        return filename;
        //}

        public static BitmapImage GetCacheImage(string id, string type = "small")
        {
            string url = FBHelper.GetImageUrl(id, type);

            if (!url.StartsWith("https://"))
                throw new ArgumentException("ImageCacher only works with http:// images", url);

            var cacheFile = url.Replace("https://", "cache/");

            var result = new BitmapImage();

            //string filename = url.Substring(27, 15);

            //filename += url.Substring(51,5);

            string filename = id + type;

            if (IsoStore.FileExists(filename))
            {
                try
                {

                    result.SetSource(IsoStore.StreamFileFromIsoStore(filename));
                }
                catch
                {
                    //exception due to non availability
                    //CacheImageAsync(url, result);
                    CacheImageAsync(url, id, type);
                    
                    result.UriSource = new Uri(url, UriKind.Absolute);

                }
            }
            else
            {
                CacheImageAsync(url, id, type);

                //CacheImageAsync(url, result);

                result.UriSource = new Uri(url, UriKind.Absolute);
            }

            return result;
        }


        public static void CacheImageAsync(string url, string id, string type)
        {
            
            //var items = new KeyValuePair<string, BitmapImage>(url, image);

            var items1 = new Dictionary<string, string>();

            items1.Add("ID", id);
            items1.Add("URL", url);
            items1.Add("TYPE", type);
            
            var t = new Thread(CacheImage);
            t.Start(items1);
        }

        private static void CacheImage(object input)
        {
            // extract the url and BitmapImage from our intput object
            //var items = (KeyValuePair<string, BitmapImage>)input;
            //var url = items.Key;
            //var image = items.Value;
            var items = (Dictionary<string, string>) input;



            var cacheFile = items["URL"].Replace("https://", "cache/");

            //var waitHandle = new AutoResetEvent(false);
            //var fileNameAndWaitHandle = new KeyValuePair<string, AutoResetEvent>(cacheFile, waitHandle);

            var wc = new WebClient();
            wc.OpenReadCompleted += OpenReadCompleted;
            // start the caching call (web async)
            wc.OpenReadAsync(new Uri(items["URL"]), items);

            //// wait for the file to be saved, or timeout after 5 seconds
            //waitHandle.WaitOne(2000);

            //string filename = url.Substring(27, 15);

            //if (IsoStore.FileExists(filename))
            //{
            //    // ok, our file now exists! set the image source on the UI thread
            //    Deployment.Current.Dispatcher.BeginInvoke(() => image.SetSource(IsoStore.StreamFileFromIsoStore(cacheFile)));
            //}

        } 

        static void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.Error.Message);
                return;
            }

            // strip the http:// and store the image.
//            var state = (KeyValuePair<string, AutoResetEvent>)e.UserState;
            var state = (Dictionary<string, string>)e.UserState;

            //string filename = state.Key.Substring(25, 15);

            string filename = state["ID"] + state["TYPE"];

            try
            {

                IsoStore.SaveToIsoStore(filename, e.Result);
                //state.Value.Set();
            }
            catch
            {
 
            }
            
        }
    }

}
