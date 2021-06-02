#define DEBUG_AGENT

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pages.Helpers;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Pages.Model;
using Microsoft.Phone.Scheduler;
using Pages.Model.Helpers;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using Pages.Controls;

namespace Pages
{
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;

        public static string AccesToken = null;

        public static bool IsUserLoggedIn = false;

        public static bool IsFirstLaunch ;

        private static int processedmodelcount;

        public static bool IsInternetEnabled = false;

        public static bool agentsAreEnabled = true;

        public static PostViewModel SelectedPostModel = null;

        public static DetailsViewModel CurrentDetailsModel = null;

        public static PostUserControl PostUserControl = null;

        private static PinnedPagesData pinnedPagesData = null;

        private static PinnedPageIDData pinnedPageIDData = null;

        private static Object appliationDataLock = new Object();

        public static PinnedPagesData PinnedPagesData
        {
            get
            {
                if (pinnedPagesData == null)
                {
                    try
                    {
                        pinnedPagesData = ApplicationData.ReadPinnedPages();

                        if (pinnedPagesData == null)
                        {
                            pinnedPagesData = new PinnedPagesData();
                            EventLogger.Log("New Pages Data object created"); 
                        }
                    }
                    catch 
                    {
                        pinnedPagesData = new PinnedPagesData();
                        EventLogger.Log("New Pages Data object created");
                    }
                }

                return pinnedPagesData;
            }

            set
            {
                pinnedPagesData = value;
            }

        }

        public static PinnedPageIDData PinnedPageIDData
        {
            get
            {
                if (pinnedPageIDData == null)
                {
                    try
                    {
                        pinnedPageIDData = ApplicationData.ReadPinnedPageIDs();

                        if (pinnedPageIDData == null)
                        {
                            pinnedPageIDData = new PinnedPageIDData();
                            EventLogger.Log("New Pages Data object created");
                        }
                    }
                    catch
                    {
                        pinnedPageIDData = new PinnedPageIDData();
                        EventLogger.Log("New Pages Data object created");
                    }
                }

                return pinnedPageIDData;
            }

            set
            {
                pinnedPageIDData = value;
            }

        }


        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {                    
                    try
                    {
                        viewModel = ApplicationData.ReadMainViewModel();

                        if (viewModel != null)
                        {
                            App.ViewModel.IsNetWorkAvailable = Utilities.IsInternetConnectionEnabled();

                            if (!string.IsNullOrEmpty(AccesToken))
                            {
                                viewModel.AccessToken = AccesToken;
                            }
                        }
                        else
                        {
                            viewModel = new MainViewModel();
                            EventLogger.Log("New View model created");
                            App.ViewModel.IsNetWorkAvailable = Utilities.IsInternetConnectionEnabled();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("read error. may be due to first time");
                        viewModel = new MainViewModel();
                        EventLogger.Log("New View model created");
                        App.ViewModel.IsNetWorkAvailable = Utilities.IsInternetConnectionEnabled();
                    }                    
                }

                return viewModel;
            }

            set
            {
                viewModel = value;
            }
        }


        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            AccesToken = Utilities.ReadFromISO<string>(Constatnts.ACCESS_TOKEN);
            FBHelper.ACCESS_TOKEN_CURENT = AccesToken;

            //SetupNetworkChange();

            if (string.IsNullOrEmpty(AccesToken))
            {
                IsUserLoggedIn = false;
            }
            else
            {
                IsUserLoggedIn = true;
            }

            IsFirstLaunch = !Utilities.ReadFromISO<bool>(Constatnts.FIRST_LAUNCH);

            if (IsFirstLaunch)
            {
            }

            //IsInternetEnabled = Utilities.IsInternetConnectionEnabled();

            //AccesToken = "AAAD8pP2QXUMBANGNhEG6FN7QUhojghenRSZBCoFB4n9qZCJJsq4dPETYZCqSjYKMjtJyQSqIqkERSWCzsZAAvzqeQ4seDOwSXA9etZCo9ggZDZD";
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }


        private static void SetupNetworkChange()
        {
            // Get current network availalability and store the 
            // initial value of the online variable
            if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                App.ViewModel.IsNetWorkAvailable = true;
                // do what is needed to GoOnline();
            }
            else
            {
                App.ViewModel.IsNetWorkAvailable = false;
                // do what is needed to GoOffline();
            }

            // Now add a network change event handler to indicate
            // network availability 
            NetworkChange.NetworkAddressChanged +=
               new NetworkAddressChangedEventHandler(OnNetworkChange);
        }

        static void OnNetworkChange(object sender, EventArgs e)
        {
            if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                if (!App.ViewModel.IsNetWorkAvailable)
                {
                    App.ViewModel.IsNetWorkAvailable = true;
                    // do what is needed to GoOnline();

                    UpdatesPinnedData();
                    EventLogger.Log("Network connected");
                }
            }
            else
            {
                if (App.ViewModel.IsNetWorkAvailable)
                {
                    App.ViewModel.IsNetWorkAvailable = false;
                    EventLogger.Log("Network disconnected");

                    // do what is needed to GoOffline();
                }
            }
        }

        void ChangeDetected(object sender, NetworkNotificationEventArgs e)
        {
            string change = string.Empty;
            switch (e.NotificationType)
            {
                case NetworkNotificationType.InterfaceConnected:

                    EventLogger.Log("Network connected");

                    change = "Connected to ";
                    break;
                case NetworkNotificationType.InterfaceDisconnected:
                    change = "Disconnected from ";

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Network disconnected");
                        EventLogger.Log("Network disconnected");

                    });
                    break;
                case NetworkNotificationType.CharacteristicUpdate:
                    change = "Characteristics changed for ";
                    break;
                default:
                    change = "Unknown change with ";
                    break;
            }


        }

        private static void UpdatesPinnedData()
        {
            try
            {
                App.PinnedPagesData = ApplicationData.ReadPinnedPages();

                foreach (DetailsViewModel model in PinnedPagesData.PinnedPages)
                {
                    if (Utilities.IsInternetConnectionEnabled())
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("Network change detected");

                        });

                        model.OldLiveTileCount = model.LiveTileCount;
                        model.LoadData();
                        model.DataLoaded += new DataLoadedHandler(model_DataLoaded);
                        model.GetAlbumsInfo();
                        model.GetPageInfo();
                    }
                    else
                    {
                        EventLogger.Log("Background thread invoked. No Connectivity");

                    }
                }

            }
            catch
            { }
        }

        static void model_DataLoaded(object o, LoadedEventArgs e)
        {
            DetailsViewModel model = (DetailsViewModel)o;

            if (model.LiveTileCount > 0 && model.LiveTileCount > model.OldLiveTileCount)
            {

                ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + model.ID));

                if (TileToFind != null)
                {
                    StandardTileData NewTileData = new StandardTileData
                    {
                        BackContent = model.Title,
                        Count = model.LiveTileCount,
                        BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", model.ID), UriKind.Absolute)

                    };

                    if (model.Posts[0].Message != null)
                    {
                        ShellToast toast = new ShellToast();
                        toast.Title = model.Title;
                        toast.Content = model.Posts[0].Message;
                        toast.Show();
                    }

                    TileToFind.Update(NewTileData);
                }
                else
                {
                    PinnedPagesData.PinnedPages.Remove(model);
                }

            }

            processedmodelcount++;

            if (processedmodelcount == PinnedPagesData.PinnedPages.Count)
            {
                try
                {
                    if (PinnedPagesData != null)
                    {
                        ApplicationData.SavePinnedPages(PinnedPagesData);
                    }

                }
                catch
                {
                    EventLogger.Log("Error saving pinned pages");
                }

            }
        }
        

        public static void HandleNoConnection()
        {
            MessageBox.Show(ErrorMessages.NETWORK_FAILURE);
        }

        public static void StartAgent()
        {
            PeriodicTask periodicTask = new PeriodicTask("PagesPeriodicAgent");
            
            periodicTask.Description = "Updates data and live tiles for pinned pages";
            //periodicTask.ExpirationTime = System.DateTime.Now.AddDays(1);

            // If the agent is already registered with the system,
            if (ScheduledActionService.Find(periodicTask.Name) != null)
            {
                if (!periodicTask.IsScheduled || !periodicTask.IsEnabled)
                {
                    ScheduledActionService.Remove("PagesPeriodicAgent");
                    AddPeriodicTask(periodicTask);
                }
            }
            else
            {
                AddPeriodicTask(periodicTask);
            }
            
        }

        private static void AddPeriodicTask(PeriodicTask periodicTask)
        {
            //only can be called when application is running in foreground
            try
            {
                ScheduledActionService.Add(periodicTask);
#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by you.");
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }

            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            catch { }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            //using (var stream = new IsolatedStorageFileStream("data.txt", FileMode.OpenOrCreate, FileAccess.Read, store))
            //using (var reader = new StreamReader(stream))
            //{
            //    if (!reader.EndOfStream)
            //    {s
            //        var serializer = new XmlSerializer(typeof(MainViewModel));
            //        viewModel = (MainViewModel)serializer.Deserialize(reader);
            //    }
            //}

            

            //// if the view model is not loaded, create a new one
            //if (viewModel == null)
            //{
            //    viewModel = new MainViewModel();
            //    //ViewModel.LoadData();
            //}

            EventLogger.Log("Application Launched");

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}

            //if (App.ViewModel == null)
            //{
            //    App.ViewModel = new MainViewModel();

            //    App.ViewModel.LoadData();
            //}
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            //using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            //using (var stream = new IsolatedStorageFileStream("data.txt",
            //                                                FileMode.Create,
            //                                                FileAccess.Write,
            //                                                store))
            //{
            //    var serializer = new XmlSerializer(typeof(MainViewModel));
            //    serializer.Serialize(stream, ViewModel);
            //}

            SaveData();

            //App.ViewModel.SaveDataToISO();
        }

        public static void SaveData()
        {
            try
            {
                if (App.ViewModel != null)
                {
                    ApplicationData.SaveMainViewModel(ViewModel);
                }

                if (App.PinnedPagesData != null)
                {
                    ApplicationData.SavePinnedPages(PinnedPagesData);
                }

                if (App.PinnedPageIDData != null)
                {
                    ApplicationData.SavePinnedPageIDs(PinnedPageIDData);
                }

                ApplicationData.SaveBitmapCache();
            }
            catch
            {
                EventLogger.Log("Error in saving application data");
            }            
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            LittleWatson.ReportException(e.Exception, "Navigation Failed");

            SaveData();
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            MessageBox.Show("Crash", e.ExceptionObject.Message, MessageBoxButton.OK);
            LittleWatson.ReportException(e.ExceptionObject, "Application Crash");
            //EventLogger.Log("Application crashed with exception " + e.ToString(), true, true);

            SaveData();
        }

        /// <summary>
        /// Method to navigate to a screen
        /// </summary>
        /// <param name="navigatePage">To which screen navigate to</param>
        public static void NavigateToPage(string navigatePage)
        {
            try
            {
                Uri targetUri = NonlinearNavigationService._NavHelper.CurrentUri;
                if (targetUri != NonlinearNavigationService._NavHelper.TargetUri && !NonlinearNavigationService._NavHelper.NavMode.Equals(NavigationMode.Back))
                {

                }

                var root = App.Current.RootVisual as PhoneApplicationFrame;
                root.Navigate(new System.Uri(navigatePage, System.UriKind.RelativeOrAbsolute));
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }


        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}