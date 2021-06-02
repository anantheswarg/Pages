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
    public class Constatnts
    {
        public static string ACCESS_TOKEN = "AcessToken";

//        public static string URL_GET_PAGES = "https://graph.facebook.com/me/likes?access_token={0}";
        public static string URL_GET_PAGES = "https://graph.facebook.com/me/likes?access_token={0}";

        public static string PAGES_FILE_NAME = "allpages";

        public static string POSTS_HELPER_NAME = "_POSTS";

        public static string RECENT_FILE_NAME = "recentpages";

        public static string LOAD = "Load";

        public static string APPEND = "Append";

        public static string PNM_INDEX = "PNMIndex";

        public static string NA = "Not Available";

        public static string MAINVIEWMODEL_FILE = "mainviewmodel";

        public static string BITMAPCACHE_FILE = "BitmapCache";

        public static string TOKEN_FILE = "AccessTokenFile";

        public static string PINNED_PAGES_FILE = "PinnedPages";

        public static string PINNED_PAGES_ID_FILE = "PinnedPageIDs";

        public static string FIRST_LAUNCH = "FirstLaunch";

        public static string LOGOUT_URL = "Logouturl";

        public static string HOME_PAGE_URL = "https://m.facebook.com/home.php";

        public static string LOGOUT_PAGE_URL = "http://m.facebook.com/logout.php{0}&pos=1";

        public static int MESSAGE_LENGTH = 250;
    }
}
