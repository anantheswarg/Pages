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
using System.Diagnostics;

namespace Pages.Model.Helpers
{
    public class ApplicationData
    {
        private static Object viewmodelLock = new object();
        private static Object pinnedPagesLock = new object();
        private static Object pinnedPageIDsLock = new object();
        private static Object BitmapCacheLock = new object();


        #region Pinned Pages
        public static void SavePinnedPages(PinnedPagesData PinnedPagesData)
        {
            lock (pinnedPagesLock)
            {
                try
                {
                    Utilities.Save<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE, PinnedPagesData);
                    //EventLogger.Log("PinnedPagesData Saved");
                }
                catch
                {
                    //EventLogger.Log("Error saving PinnedPagesData");
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

                    //EventLogger.Log("PinnedPagesData  loaded from iso");
                }
                catch
                {
                    //EventLogger.Log("Error Loading PinnedPagesData from iso");
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
                    //EventLogger.Log("PinnedPagesData Saved");
                }
                catch
                {
                    //EventLogger.Log("Error saving PinnedPagesData");
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

                    //EventLogger.Log("PinnedPageIDData  loaded from iso");
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    pinnedPageIDData = new PinnedPageIDData();
                }
            }

            return pinnedPageIDData;
        }
        #endregion

        

    }
}
