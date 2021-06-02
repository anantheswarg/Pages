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
using Pages.Helpers;

namespace Pages.Model.Helpers
{
    public class ApplicationData
    {
        private static Object viewmodelLock = new object();
        private static Object pinnedPagesLock = new object();
        private static Object pinnedPageIDsLock = new object();
        private static Object BitmapCacheLock = new object();


        #region MainViewModel
        public static void SaveMainViewModel(MainViewModel ViewModel)
        {
            lock (viewmodelLock)
            {
                try
                {
                    Utilities.Save<MainViewModel>(Constatnts.MAINVIEWMODEL_FILE, ViewModel);
                    EventLogger.Log("MainViewModel Saved");
                }
                catch
                {
                    EventLogger.Log("Error saving MainViewModel"); 
                }
            }
        }

        public static MainViewModel ReadMainViewModel()
        {
            MainViewModel viewModel = null;
            lock (viewmodelLock)
            {
                try
                {
                    viewModel = Utilities.Load<MainViewModel>(Constatnts.MAINVIEWMODEL_FILE);

                    EventLogger.Log("MainView Model loaded from iso");                            
                }
                catch
                {
                    EventLogger.Log("Error Loading MainViewModel from iso");
                }
            }

            return viewModel;
        }
        #endregion


        #region Pinned Pages
        public static void SavePinnedPages(PinnedPagesData PinnedPagesData)
        {
            lock (pinnedPagesLock)
            {
                try
                {
                    Utilities.Save<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE, PinnedPagesData);
                    EventLogger.Log("PinnedPagesData Saved");
                }
                catch
                {
                    EventLogger.Log("Error saving PinnedPagesData");
                }
            }
        }

        public static PinnedPagesData ReadPinnedPages()
        {
            PinnedPagesData pinnedPageData = null;

            lock (pinnedPagesLock)
            {
                try
                {
                    pinnedPageData = Utilities.Load<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE);

                    EventLogger.Log("PinnedPagesData  loaded from iso");
                }
                catch
                {
                    EventLogger.Log("Error Loading PinnedPagesData from iso");
                }
            }

            return pinnedPageData;
        }
        #endregion


        #region PinnedPageIDs
        public static void SavePinnedPageIDs(PinnedPageIDData PinnedPageIDData)
        {
            lock (pinnedPageIDsLock)
            {
                try
                {
                    Utilities.Save<PinnedPageIDData>(Constatnts.PINNED_PAGES_ID_FILE, PinnedPageIDData);
                    EventLogger.Log("PinnedPagesData Saved");
                }
                catch
                {
                    EventLogger.Log("Error saving PinnedPagesData");
                }
            }
        }

        public static PinnedPageIDData ReadPinnedPageIDs()
        {
            PinnedPageIDData pinnedPageIDData = null;

            lock (pinnedPagesLock)
            {
                try
                {
                    pinnedPageIDData = Utilities.Load<PinnedPageIDData>(Constatnts.PINNED_PAGES_ID_FILE);

                    EventLogger.Log("PinnedPageIDData  loaded from iso");
                }
                catch
                {
                    EventLogger.Log("Error Loading PinnedPageIDData from iso");
                }
            }

            return pinnedPageIDData;
        }
        #endregion

        #region BitmapCache
        public static void SaveBitmapCache()
        {
            //lock (BitmapCacheLock)
            //{
                try
                {
                    if (LowProfileImageLoader.webBitmapSourceCache != null && LowProfileImageLoader.webBitmapSourceCache.imageCache != null && LowProfileImageLoader.webBitmapSourceCache.imageCache.Count > 0)
                    {
                        if (!LowProfileImageLoader.webBitmapSourceCache.isCreationDone)
                        {
                            LowProfileImageLoader.webBitmapSourceCache.isCreationDone = true;
                        }

                        Utilities.Save<WebBitmapSourceCache>(Constatnts.BITMAPCACHE_FILE, LowProfileImageLoader.webBitmapSourceCache);
                        EventLogger.Log("BitmapCache Saved");
                    }

                }
                catch
                {
                    EventLogger.Log("Error saving BitmapCache");
                }
            //}
        }

        #endregion

    }
}
