
#define DEBUG_AGENT
#define BACKGROUNDAGENT
using System.Windows;
using Microsoft.Phone.Scheduler;
using System;
using Microsoft.Phone.Net.NetworkInformation;
using System.Threading;
using Microsoft.Phone.Info;
using Pages.Model;
using Pages.Model.Helpers;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Linq;

namespace LiveTilesAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        private int modelcount = 0;
        private int processedmodelcount = 0;

        private string name;


        //private MainViewModel viewModel;

        private PinnedPagesData PinnedPagesData;

        private PinnedPageIDData PinnedPageIDData;

        private static Object pageProcessLock = new object();


        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        //~ScheduledAgent()
        //{
        //    Dispose();
        //}

        //public void Dispose()
        //{
        //    _agentWaitHandle.Dispose();
        //    _stepWaitHandle.Dispose();
        //    GC.SuppressFinalize(this);
        //}

        #region Threading management
        EventWaitHandle _agentWaitHandle = new AutoResetEvent(false);
        //EventWaitHandle _stepWaitHandle = new AutoResetEvent(false);
        //int _threads;

        //private void SignalThreadStart()
        //{
        //    Interlocked.Increment(ref _threads);
        //}

        //private void SignalThreadEnd()
        //{
        //    Interlocked.Decrement(ref _threads);
        //    if (_threads <= 1)
        //        _stepWaitHandle.Set();
        //    if (_threads <= 0)
        //        _agentWaitHandle.Set();
        //}

        //private void WaitForTaskToEnd()
        //{
        //    if (_threads > 1)
        //        _stepWaitHandle.WaitOne(5000);
        //}

        //bool IsMemoryUsageHigh()
        //{
        //    double highPercentage = 0.98;
        //    double highMemory = DeviceStatus.ApplicationMemoryUsageLimit * highPercentage;
        //    return DeviceStatus.ApplicationCurrentMemoryUsage > highMemory;
        //}

        //void TerminateAll()
        //{
        //    // Allow the scheduled agent to terminate without taking into account other threads.
        //    _threads = 0;
        //    _agentWaitHandle.Set();
        //}
        #endregion

        void WriteMemUsage(string message)
        {
//#if DEBUG
//            long used = DeviceStatus.ApplicationCurrentMemoryUsage / 1024;
//            long percentage = DeviceStatus.ApplicationCurrentMemoryUsage * 100 / DeviceStatus.ApplicationMemoryUsageLimit;
//            string toWrite = string.Format("{3}: {0} - {1} KB ({2}% of available memory)",
//                message, used, percentage, DateTime.Now.ToString("HH:mm:ss.ff"));
//            Debug.WriteLine(toWrite);
//#endif
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //EventLogger.Log("Background thread crashed", false);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }


            try
            {
                LittleWatsonBakground.ReportException(e.ExceptionObject, "Background Agent Crash");

                //if (viewModel != null)
                //{
                //    Utilities.Save<MainViewModel>("mainviewmodel", viewModel);

                    //EventLogger.Log("Background thread crashed but data saved");

                //}

                //if (PinnedPagesData != null)
                //{
                //    Utilities.Save<PinnedPagesData>(Constatnts.PINNED_PAGES_FILE, PinnedPagesData);

                //    EventLogger.Log("Background thread crashed but pages data saved", false);

                //}
            }
            catch
            {

            }

            //DebugOutputMemoryUsage("Final Memory Snapshot:");
        }

        protected void DebugOutputMemoryUsage(string label = null)
        {
            //var limit = DeviceStatus.ApplicationMemoryUsageLimit;
            //var current = DeviceStatus.ApplicationCurrentMemoryUsage;
            //var remaining = limit - current;
            //var peak = DeviceStatus.ApplicationPeakMemoryUsage;
            //var safetyMargin = limit - peak;

            //if (label != null)
            //{
            //    EventLogger.Log(label, false);
            //}


            //EventLogger.Log("Memory limit (KB): " + limit/ 1024, false);
            //EventLogger.Log(string.Format("Current memory usage: {0} KB ({1} KB remaining)", current/ 1024, remaining/ 1024), false);
            //EventLogger.Log(string.Format("Peak memory usage: {0} KB ({1} KB safety margin)", peak/ 1024, safetyMargin/1024), false);
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //DebugOutputMemoryUsage("Initial Memory Snapshot:");

            //DateTime start, end;
            //start = DateTime.Now;
            //SignalThreadStart();
            try
            {
                //WriteMemUsage("Initial Capture");

                //DebugOutputMemoryUsage("Final Memory Snapshot:");
                DoWork1();
            }
            catch (Exception)
            {
                //TerminateAll();
            }

            //SignalThreadEnd();

            //end = DateTime.Now;
            //int maxTimeout = 24000 - (int)((end - start).TotalMilliseconds);

            //if (maxTimeout < 0)
            //    maxTimeout = 1000;

            //_agentWaitHandle.WaitOne(maxTimeout);
            //WriteMemUsage("Exit with " + _threads.ToString() + " running");
            //NotifyComplete();

            ////TODO: Add code to perform your task in background

            
            //this.name = task.Name;


            //var request = HttpWebRequest.Create(new Uri("http://www.alphaapps.co.in/data/featured.txt", UriKind.Absolute));

            //var result = (IAsyncResult)request.BeginGetResponse(ResponseCallback, request);

            //Thread.Sleep(2000);

            
            
        }

        //void DoWork()
        //{
        //    CompleteAction(UpdateLiveTileCounts);
        //    CompleteAction(ProcessModelCounts);
        //}
        void DoWork1()
        {
            NetworkInterfaceType CurrentNetworkType = NetworkInterface.NetworkInterfaceType;
            //string s = CurrentNetworkType.ToString();

            string AccesToken = Utilities.ReadFromISO<string>(Constatnts.ACCESS_TOKEN);
            FBHelper.ACCESS_TOKEN_CURENT = AccesToken;

            try
            {
                if (Utilities.IsInternetConnectionEnabled())
                {
                    try
                    {
                        PinnedPageIDData = ApplicationData.ReadPinnedPageIDs();

                        //WriteMemUsage("After  Page IDs loaded");

                    }
                    catch
                    { }

                    if (PinnedPageIDData != null)
                    {
                        _agentWaitHandle.Reset();
                        foreach (string id in PinnedPageIDData.PinnedPageIDs)
                        {
                            //DebugOutputMemoryUsage("Before GC Memory Snapshot:");
                            //WriteMemUsage("Before GC Collect");
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            //WriteMemUsage("After GC Collect");
                            //DebugOutputMemoryUsage("After GC Memory Snapshot:");

                            PostsHelper helper = null;
                            try
                            {
                                helper = Utilities.Load<PostsHelper>(id + Constatnts.POSTS_HELPER_NAME);
                                helper.DataLoaded += new DataLoadedHandler(helper_DataLoaded);
                            }
                            catch { }

                            if (helper != null)
                            {
                                helper.OldLiveTileCount = helper.LiveTileCount;
                                helper.LoadData();

                                //WriteMemUsage("After  Page Posts loaded");
                            }
                            _agentWaitHandle.WaitOne();
                            // update pages data
                        }
                    }
                    else
                    {
                        NotifyComplete();
                    }

                }
                else
                {
                    NotifyComplete();
                }
            }
            catch (Exception e)
            {
                NotifyComplete();
            }
        }

        void helper_DataLoaded(object o, LoadedEventArgs e)
        {
            PostsHelper helper = (PostsHelper)o;

            processedmodelcount++;
            ProcessModelCounts1(helper);

            //DebugOutputMemoryUsage("After helper processed Snapshot:");
            //CheckmemoryUsage();


            if (processedmodelcount == PinnedPageIDData.PinnedPageIDs.Count)
            {
                //WriteMemUsage("Final Snapshot");
                _agentWaitHandle.Set();
                //ApplicationData.SavePinnedPageIDs(PinnedPageIDData);
                NotifyComplete();
            }

            _agentWaitHandle.Set();
        }

        

        //void UpdateLiveTileCounts()
        //{
        //    EventLogger.Log("Background thread invoked", false);

        //    NetworkInterfaceType CurrentNetworkType = NetworkInterface.NetworkInterfaceType;
        //    string s = CurrentNetworkType.ToString();

        //    EventLogger.Log("Background thread: Connectivity type: " + s, false);

        //    SignalThreadStart();

        //    try
        //    {
        //        if (Utilities.IsInternetConnectionEnabled())
        //        {
        //            PinnedPagesData = ApplicationData.ReadPinnedPages();
        //            ShellToast toast = new ShellToast();
        //            toast.Title = "Agent run";
        //            toast.Content = "test run";
        //            toast.Show();

        //            string AccesToken = Utilities.ReadFromISO<string>(Constatnts.ACCESS_TOKEN);
        //            FBHelper.ACCESS_TOKEN_CURENT = AccesToken;

        //            DebugOutputMemoryUsage("Memory Snapshot with pages in memory:");

        //            Debug.WriteLine("Pinned page count: " + PinnedPagesData.PinnedPages.Count.ToString());

        //            foreach (DetailsViewModel model in PinnedPagesData.PinnedPages)
        //            {
        //                modelcount++;

        //                EventLogger.Log("Processing Model:  " + model.Title);
        //                Debug.WriteLine("Processing Model:  " + model.Title);

        //                EventLogger.Log("OldLiveTileCount:  " + model.OldLiveTileCount);

        //                model.OldLiveTileCount = model.LiveTileCount;
        //                model.LoadData();
        //                model.DataLoaded += new DataLoadedHandler(model_DataLoaded);

        //                CheckmemoryUsage();
        //                //model.GetAlbumsInfo();
        //                //model.GetPageInfo();
        //            }

        //            EventLogger.Log("Background thread: Pinned Page Count: " + modelcount.ToString(), false);

        //            if (modelcount == 0)
        //            {
        //                DebugOutputMemoryUsage("Final Memory Snapshot:");
        //                SignalThreadEnd();
        //                NotifyComplete();
        //            }
        //        }
        //        else
        //        {
        //            EventLogger.Log("Background thread invoked. No Connectivity", false);
        //            SignalThreadEnd();
        //            NotifyComplete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.Log("Background thread: ViewModel read exception", false);
        //        SignalThreadEnd();
        //        NotifyComplete();
        //    }
        //}

        //void CompleteAction(Action action)
        //{
        //    WriteMemUsage("Start " + action.Method.Name);
        //    action.Invoke();
        //    WaitForTaskToEnd();
        //    WriteMemUsage("End " + action.Method.Name);

            
        //}

        //void CheckmemoryUsage()
        //{
        //    if (IsMemoryUsageHigh())
        //    {
        //        WriteMemUsage("High memory usage. Recovering memory...");
        //        Thread.Sleep(100);
        //        //this.Dispose();
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        WriteMemUsage("Memory recovery completed");

        //        if (IsMemoryUsageHigh())
        //        {
        //            WriteMemUsage("Not enough memory to continue. Terminating...");
        //            throw new OutOfMemoryException("Not enough memory to continue with the ScheduledAgent");
        //        }
        //    }
        //}

        //private void ResponseCallback(IAsyncResult result)
        //{
        //    var request = (HttpWebRequest)result.AsyncState;
        //    var response = request.EndGetResponse(result);

        //    using (var stream = response.GetResponseStream())
        //    using (var reader = new StreamReader(stream))
        //    {
        //        // FAIL - httpWebRequestTextBlock.Text = reader.ReadToEnd(); //UnauthorizedAccessException was Unhandled - Invalid cross-thread access.

        //        // FAIL - Dispatcher.BeginInvoke(() => { httpWebRequestTextBlock.Text = reader.ReadToEnd(); }); //ObjectDisposedException unhandled - Cannot read from a closed TextReader.

        //        var contents = reader.ReadToEnd();
        //        //Dispatcher.BeginInvoke(() => { httpWebRequestTextBlock.Text = contents; });

        //    }
        //}

        //void model_DataLoaded(object o, LoadedEventArgs e)
        //{
        //    DetailsViewModel model = (DetailsViewModel)o;

        //    EventLogger.Log("NewLiveTileCount for model  " + model.Title + " " + model.LiveTileCount, false);

        //    Debug.WriteLine("Data loading processed for model " + model.Title);

        //    processedmodelcount++;

        //    CheckmemoryUsage();

        //    if (processedmodelcount == PinnedPagesData.PinnedPages.Count)
        //    {
        //        EventLogger.Log("Background thread: Processed all pinned", false);

        //        //try
        //        //{
        //        //    if (PinnedPagesData != null)
        //        //    {
        //        //        ApplicationData.SavePinnedPages(PinnedPagesData);
        //        //    }

        //        //}
        //        //catch
        //        //{
        //        //    EventLogger.Log("Background thread: Error saving pinned pages", false);
        //        //}

                

        //        DebugOutputMemoryUsage("Memory Snapshot after models are processed");

        //        ProcessModelCounts();

        //        SignalThreadEnd();
        //        //NotifyComplete();
        //    }
            
        //}

//        void ProcessModelCounts()
//        {
//            //SignalThreadStart();
//            foreach (DetailsViewModel model in PinnedPagesData.PinnedPages)
//            {
//                if (model.LiveTileCount > 0 && model.LiveTileCount > model.OldLiveTileCount)
//                {
//                    EventLogger.Log("Data updated for model: " + model.Title, false);

//                    Debug.WriteLine("Data updated for model: " + model.Title);

//                    ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + model.ID));

//                    if (TileToFind != null)
//                    {
//                        StandardTileData NewTileData = new StandardTileData
//                        {
//                            BackContent = model.Title,
//                            Count = model.LiveTileCount,
//                            BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", model.ID), UriKind.Absolute)

//                        };

//                        if (model.Posts[0].Message != null)
//                        {
//                            ShellToast toast = new ShellToast();
//                            toast.Title = model.Title;
//                            toast.Content = model.Posts[0].Message;
//                            toast.Show();
//                        }

//                        TileToFind.Update(NewTileData);
//                        Debug.WriteLine("Live tile updated for model: " + model.Title);

//                    }
//                    else
//                    {
//                        PinnedPagesData.PinnedPages.Remove(model);
//                    }
//                }
//            }

//            DebugOutputMemoryUsage("Final Memory Snapshot");


//            //SignalThreadEnd();

//#if DEBUG_AGENT
//            ScheduledActionService.LaunchForTest(name, TimeSpan.FromSeconds(10));
//#endif



//        }

        void ProcessModelCounts1(PostsHelper helper)
        {

            if (helper.LiveTileCount > 0 && helper.LiveTileCount > helper.OldLiveTileCount)
            {
                ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("ID=" + helper.ID));

                if (TileToFind != null)
                {
                    StandardTileData NewTileData = new StandardTileData
                    {
                        BackContent = helper.Posts[0].Message,
                        Count = helper.LiveTileCount,
                        BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}", helper.ID), UriKind.Absolute)
                    };

                    if (helper.Posts[0].Message != null)
                    {
                        ShellToast toast = new ShellToast();
                        toast.Title = helper.Title;
                        toast.Content = helper.Posts[0].Message;
                        toast.Show();
                    }

                    Utilities.Save<PostsHelper>(helper.ID + Constatnts.POSTS_HELPER_NAME, helper);

                    try
                    {

                        TileToFind.Update(NewTileData);
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine("Unable to update the live for " + helper.Title);
                    }
                }
                else
                {

                    this.PinnedPageIDData.PinnedPageIDs.Remove(helper.ID);

                    ApplicationData.SavePinnedPageIDs(this.PinnedPageIDData);
                }
            }
        }

        
    }
}