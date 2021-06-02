using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using Pages.Helpers;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Linq;
using System.Net.NetworkInformation;

namespace Pages.Model
{
    [DataContract]
    public class MainViewModel : INotifyPropertyChanged
    {
        public event DataLoadedHandler DataLoaded;
        public event DataLoadedHandler FeaturedDataLoaded;


        public bool IsNetWorkAvailable = false;


        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.PagesOld = new ObservableCollection<PageItemModel>();
            this.SearchedPages = new ObservableCollection<PageItemModel>();
            this.Recent = new ObservableCollection<PageItemModel>();
            this.Featured = new ObservableCollection<PageItemModel>();
            //this.PinnedPages = new ObservableCollection<DetailsViewModel>();
            //this.PinnedPageIDs = new ObservableCollection<string>();

            IsNetWorkAvailable = Utilities.IsInternetConnectionEnabled();
            
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);

            this.IsDataLoaded = false;

            this.IsNewDataLoading = false;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            IsNetWorkAvailable = Utilities.IsInternetConnectionEnabled();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<ItemViewModel> Items { get; set; }

        [DataMember]
        public ObservableCollection<PageItemModel> PagesOld { get; set; }

        [DataMember]
        public ObservableCollection<PageItemModel> SearchedPages { get; set; }

        public ObservableCollection<PageItemModel> PagesNew { get; set; }

        //[DataMember]
        //public ObservableCollection<DetailsViewModel> PinnedPages { get; set; }

        //[DataMember]
        //public ObservableCollection<string> PinnedPageIDs { get; set; }

        private ObservableCollection<GroupedOC<PageItemModel>> _pages = new ObservableCollection<GroupedOC<PageItemModel>>();

        /// <summary>
        /// Gets the GroupedContacts property.
        ///
        /// Changes to that property's value raise the PropertyChanged event.
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ObservableCollection<GroupedOC<PageItemModel>> Pages
        {
            get
            {
                return _pages;
            }
            set
            {
                if (_pages == value)
                {
                    return;
                }
                var oldValue = _pages;
                _pages = value;
                // Update bindings, no broadcast
                NotifyPropertyChanged("Pages");
            }
        }

        ///// <summary>
        ///// A collection for ItemViewModel objects.
        ///// </summary>
        //[DataMember]
        //public ObservableCollection<PageItemModel> Pages { get; set; }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<PageItemModel> Recent { get; set; }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        [DataMember]
        public ObservableCollection<PageItemModel> Featured { get; set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        private bool _isRecentDataLoaded;

        [DataMember]
        public bool IsRecentDataLoaded
        {
            get
            {
                return _isRecentDataLoaded;
            }
            set
            {
                if (value != _isRecentDataLoaded)
                {
                    _isRecentDataLoaded = value;
                    NotifyPropertyChanged("IsRecentDataLoaded");
                }
            }
        }

        private bool _isFeaturedDataLoaded;

        [DataMember]
        public bool IsFeaturedDataLoaded
        {
            get
            {
                return _isFeaturedDataLoaded;
            }
            set
            {
                if (value != _isFeaturedDataLoaded)
                {
                    _isFeaturedDataLoaded = value;
                    NotifyPropertyChanged("IsFeaturedDataLoaded");
                }
            }
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

        private string _accessToken;

        [DataMember]
        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                if (value != _accessToken)
                {
                    _accessToken = value;
                    NotifyPropertyChanged("AccessToken");
                }
            }
        }

        private bool _isNewDataLoading;

        public bool IsNewDataLoading
        {
            get
            {
                return _isNewDataLoading;
            }
            set
            {
                if (value != _isNewDataLoading)
                {
                    _isNewDataLoading = value;
                    NotifyPropertyChanged("IsNewDataLoading");
                }
            }
        }

        #region Load Featured

        public void LoadFeatured()
        {
            //if (this.Featured.Count<PageItemModel>() == 0)
            //{
            //    this.Featured.Add(new PageItemModel() { Name = "Coca-Cola", Id = "40796308305", Category = "Food/beverages" });
            //    this.Featured.Add(new PageItemModel() { Name = "History", Id = "8429246183", Category = "Tv network" });
            //    this.Featured.Add(new PageItemModel() { Name = "Cristiano Ronaldo", Id = "81221197163", Category = "Athlete" });
            //    this.Featured.Add(new PageItemModel() { Name = "Manchester United", Id = "7724542745", Category = "Professional sports team" });
            //    this.Featured.Add(new PageItemModel() { Name = "Selena Gomez", Id = "7961985974", Category = "Actor/director" });
            //    this.Featured.Add(new PageItemModel() { Name = "espnstar.com", Id = "69860094544", Category = "Media/news/publishing" });
            //    this.Featured.Add(new PageItemModel() { Name = "How I Met Your Mother", Id = "7807422276", Category = "Tv show" });
            //    this.Featured.Add(new PageItemModel() { Name = "Audi USA", Id = "96585976469", Category = "Cars" });
            //}

            if (this.Featured.Count > 0)
            {
                this.Featured.Clear();
            }

            WebClient clientFeatured = new WebClient();

            string url = FBHelper.GetFeaturedUrl();

            clientFeatured.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientFeatured_DownloadStringCompleted);

            clientFeatured.DownloadStringAsync(new Uri(url, UriKind.Absolute));
        }

        void clientFeatured_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Cancelled != true)
                {
                    if (!string.IsNullOrEmpty(e.Result))
                    {
                        JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);

                        JToken resultString = (JToken)responeJObject["data"];
                        JArray resultArray = (JArray)resultString;

                        for (int i = 0; i < resultArray.Count; i++)
                        {
                            PageItemModel page = PageItemModel.FromJson(resultArray[i].ToString());
                            this.Featured.Add(page);
                        }
                    }
                }
            }
            catch
            {}

            LoadedEventArgs e1 = new LoadedEventArgs("");

            this.IsFeaturedDataLoaded = true;

            OnFeaturedDataLoad((object)this, e1);

        }

        void OnFeaturedDataLoad(object o, LoadedEventArgs e)
        {
            if (FeaturedDataLoaded != null)
            {
                FeaturedDataLoaded(o, e);
            }
        }

        #endregion

        #region Load Pages

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            try
            {
                // Sample data; replace with real data
                
                this.GetAllPages(Constatnts.LOAD);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.ToString());
                //MessageBox.Show(ErrorMessages.ERROR_UNKNOWN);
            }
        }



        private void GetAllPages(string type)
        {
            WebClient clientAllPages = new WebClient();

            string url = FBHelper.GetAllPagesURL();

            clientAllPages.DownloadStringCompleted += new DownloadStringCompletedEventHandler(clientAllPages_DownloadStringCompleted);
            
            clientAllPages.DownloadStringAsync(new System.Uri(url, System.UriKind.Absolute), type);

        }

        void clientAllPages_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled != true)
            {
                if (!string.IsNullOrEmpty(e.Result))
                {
                    JObject responeJObject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(e.Result);
                    
                    JToken resultString = (JToken)responeJObject["data"];
                    JArray resultArray = (JArray)resultString;

                    string type = (string)e.UserState;

                    if (type.Equals(Constatnts.APPEND))
                    {

                        this.PagesNew = new ObservableCollection<PageItemModel>();
                    }
                                            
                    
                    for (int i = 0; i < resultArray.Count; i++)
                    {
                        PageItemModel page = PageItemModel.FromJson(resultArray[i].ToString());

                        if (type.Equals(Constatnts.LOAD))
                        {
                            this.PagesOld.Add(page);

                        }

                        if (type.Equals(Constatnts.APPEND))
                        {
                            this.PagesNew.Add(page);
                        }

                        

                            //this.Recent.Clear();

                            //if (i < 8)
                            //{
                            //    this.AddRecentPage(page);
                            //}
                        
                    }


                    if (type.Equals(Constatnts.LOAD))
                    {
                        this.LoadPages();
                    }

                    if (type.Equals(Constatnts.APPEND))
                    {
                        this.AppendPages();
                    }

                    LoadedEventArgs e1 = new LoadedEventArgs("");

                    this.IsDataLoaded = true;

                    OnDataLoad((object)this, e1);
                }
            }
            else
            {
                LoadedEventArgs e1 = null;

                string type = (string)e.UserState;

                if (type.Equals(Constatnts.LOAD))
                {
                    if (e.Error.Message.Contains("The remote server returned an error"))
                    {
                        e1 = new LoadedEventArgs("Could not connect to the server. Please try later.");
                    }
                }

                OnDataLoad((object)this, e1);
            }

            this.IsNewDataLoading = false;

        }

        public void LoadPages()
        {
            Debug.WriteLine("Pages data created");
            this.Pages = CollectionHelpers.CreateGroupedOC(PagesOld);
        }

        public void AppendPages()
        {
            //this.Pages = CollectionHelpers.CreateGroupedOC(PagesNew);

            if (this.SearchedPages != null)
            {
                foreach (PageItemModel page in this.SearchedPages)
                {
                    this.PagesNew.Add(page);
                }
            }

            IEqualityComparer<PageItemModel> comparer = new PageItemModelComparer();
            
            IEnumerable<PageItemModel> enumerable1 = this.PagesNew.Except(this.PagesOld, comparer);

            IEnumerable<PageItemModel> enumerable2 = this.PagesOld.Except(this.PagesNew, comparer);

            if (enumerable1.Count<PageItemModel>() > 0 || enumerable2.Count<PageItemModel>() > 0)
            {
                this.Pages = CollectionHelpers.CreateGroupedOC(PagesNew);

                this.PagesOld = this.PagesNew;

            }

            // code to insert the pages not already available

//            foreach (PageItemModel page in this.PagesNew)
//            {

//                var q = this.PagesOld.Where(X => X.Id.Equals(page.Id)).FirstOrDefault();

//                if (q == null)
//                {
//                    // do stuff

//                    this.Pages = CollectionHelpers.CreateGroupedOC(PagesNew);

//                    this.PagesOld = this.PagesNew;

//                    break;
////                      

//                    //var goc = this.Pages.Where(X => X.Title.Equals(page.Name[0].ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

//                    //if (goc != null)
//                    //{
//                        //this.Pages.Remove(goc);

//                          //Deployment.Current.Dispatcher.BeginInvoke(() => {

//                            //goc.Add(page);

//                            //goc.OrderBy(i => i.Name);

//                            //List<PageItemModel> listOfPages = goc.ToList<PageItemModel>();

//                            //listOfPages.Add(page);

//                            //listOfPages = listOfPages.OrderBy(o => o.Name).ToList<PageItemModel>();

//                            //goc = new GroupedOC<PageItemModel>(goc.Title);

//                            //foreach (PageItemModel pagetoadd in listOfPages)
//                            //{
//                            //    goc.Add(pagetoadd);
//                            //}
                            
////                        });

//                        //this.Pages.Add(goc);
//                    //}
//                    //else
//                    //{
//                    //    goc = new GroupedOC<PageItemModel>(page.Name[0].ToString());

////                        this.Pages.Add(goc);
////                    }
//                    //                    MessageBox.Show("Not Found");
//                }
//            }
        }

        public void UpdateData()
        {
            this.LoadPages();
            if (this.IsNetWorkAvailable)
            {
                this.IsNewDataLoading = true;

                this.GetAllPages(Constatnts.APPEND);
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

        #region Load Recent

        public void LoadRecentPages()
        {
            if (!this.IsRecentDataLoaded)
            {
                int index = 0;
                
                if (this.PagesOld != null)
                {
                    foreach (PageItemModel page in this.PagesOld)
                    {
                        if (index++ < 8)
                        {
                            this.AddRecentPage(page);
                        }
                        else
                        {
                            break;
                        }
                    }

                    this.IsRecentDataLoaded = true;

                }
            }
        }

        public void AddRecentPage(PageItemModel page)
        {
            if (this.Recent.Count == 8)
            {
                bool isFound = false;
                foreach (PageItemModel page1 in this.Recent)
                {
                    if (page1.Equals(page))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    DateTime datetime1 = DateTime.Now;

                    try
                    {
                        DateTime.TryParse(this.Recent[0].Created_time, out datetime1);

                        DateTime datetime2 = DateTime.Now;

                        DateTime.TryParse(page.Created_time, out datetime2);
                        if (datetime2 > datetime1)
                        {
                            this.Recent.Insert(0, page);
                            this.Recent.RemoveAt(8);
                        }
                    }
                    catch
                    {

                    }


                }
            }
            else if (this.Recent.Count < 8)
            {
                this.Recent.Add(page);
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

        //public void SaveDataToISO()
        //{



        //    Utilities.Save<ObservableCollection<PageItemModel>>(Constatnts.PAGES_FILE_NAME, Pages);
        //    Utilities.Save<ObservableCollection<PageItemModel>>(Constatnts.RECENT_FILE_NAME, Recent); 
        //}



    }
}