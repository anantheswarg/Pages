// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Media;
using Pages.Helpers;

namespace Pages
{
    /// <summary>
    /// Provides access to the Image.UriSource attached property which allows
    /// Images to be loaded by Windows Phone with less impact to the UI thread.
    /// </summary>
    public static class LowProfileImageLoader
    {
        private const int WorkItemQuantum = 3;
        private const int ThreadSleepTime = 10;
        private const int DefferedSleepTime = 300;
        private const long CacheSize = 8 * 1024 * 1024; //8MB
        private const int MaxCachedImageHeight = 200;
        private const int MaxCachedImageWidth = 200;

        private static readonly Thread _thread = new Thread(WorkerThreadProc);
        private static readonly Dictionary<Image, Uri> _pausedRequests = new Dictionary<Image, Uri>();
        private static readonly Queue<PendingRequest> _pendingRequests = new Queue<PendingRequest>();
        private static readonly Queue<IAsyncResult> _pendingResponses = new Queue<IAsyncResult>();
        private static readonly object _syncBlock = new object();
        private static bool _exiting;

        private static WebBitmapSourceCache bitmapSourceCache = null;
        public static WebBitmapSourceCache webBitmapSourceCache
        {
            get
            {
                if (bitmapSourceCache == null)
                {
                    try
                    {
                        bitmapSourceCache = Utilities.Load<WebBitmapSourceCache>(Constatnts.BITMAPCACHE_FILE);
                                            
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("bitmap cache read error. may be due to first time");
                        bitmapSourceCache = new WebBitmapSourceCache(CacheSize);
                    }
                }

                return bitmapSourceCache;
            }

            set
            {
                bitmapSourceCache = value;
            }

        }

        private static void UpdateImageCache()
        {
            //if (webBitmapSourceCache.imageCache != null && webBitmapSourceCache.imageCache.Count > 0)
            //{
            //    foreach
            //}
        }

        /// <summary>
        /// Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static Uri GetUriSource(Image obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (Uri)obj.GetValue(UriSourceProperty);
        }

        /// <summary>
        /// Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static void SetUriSource(Image obj, Uri value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(UriSourceProperty, value);
        }

        /// <summary>
        /// Identifies the UriSource attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.RegisterAttached(
            "UriSource", typeof(Uri), typeof(LowProfileImageLoader), new PropertyMetadata(OnUriSourceChanged));

        /// <summary>
        /// Gets or sets a value indicating whether low-profile image loading is enabled.
        /// </summary>
        public static bool IsEnabled { get; set; }

        public static bool IsPaused { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static constructor performs additional tasks.")]
        static LowProfileImageLoader()
        {
            // Start worker thread
            _thread.IsBackground = true;
            _thread.Start();
            Application.Current.Exit += new EventHandler(HandleApplicationExit);
            IsEnabled = true;
        }

        private static void HandleApplicationExit(object sender, EventArgs e)
        {
            // Tell worker thread to exit
            _exiting = true;
            if (Monitor.TryEnter(_syncBlock, 100))
            {
                Monitor.Pulse(_syncBlock);
                Monitor.Exit(_syncBlock);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Relevant exceptions don't have a common base class.")]
        private static void WorkerThreadProc(object unused)
        {
            Queue<PendingRequest> pendingRequests = new Queue<PendingRequest>();
            Queue<IAsyncResult> pendingResponses = new Queue<IAsyncResult>();

            while (!_exiting)
            {
                while (IsPaused)
                {
                    //Debug.WriteLine("clearing {0}", pendingRequests.Count);
                    //pendingRequests.Clear();
                    Thread.Sleep(DefferedSleepTime);
                    pendingRequests = new Queue<PendingRequest>();
                }

                lock (_syncBlock)
                {
                    // Wait for more work if there's nothing left to do
                    if ((0 == _pendingRequests.Count) && (0 == _pendingResponses.Count) && (0 == pendingRequests.Count) && (0 == pendingResponses.Count))
                    {
                        Monitor.Wait(_syncBlock);
                        if (_exiting)
                        {
                            return;
                        }
                    }

                    // Copy work items to private collections
                    while (0 < _pendingRequests.Count)
                    {
                        pendingRequests.Enqueue(_pendingRequests.Dequeue());
                    }
                    while (0 < _pendingResponses.Count)
                    {
                        pendingResponses.Enqueue(_pendingResponses.Dequeue());
                    }
                }

                Queue<PendingCompletion> pendingCompletions = new Queue<PendingCompletion>();
                // Process pending requests
                Debug.WriteLine("pending requests: {0}", pendingRequests.Count);
                for (var i = 0; (i < pendingRequests.Count) && (i < WorkItemQuantum); i++)
                {
                    var pendingRequest = pendingRequests.Dequeue();
                    if (pendingRequest.Uri.IsAbsoluteUri)
                    {
                        // Download from network
                        var webRequest = HttpWebRequest.CreateHttp(pendingRequest.Uri);
                        webRequest.AllowReadStreamBuffering = true; // Don't want to block this thread or the UI thread on network access
                        webRequest.BeginGetResponse(HandleGetResponseResult, new ResponseState(webRequest, pendingRequest.Image, pendingRequest.Uri));
                    }
                    else
                    {
                        // Load from application (must have "Build Action"="Content")
                        var originalUriString = pendingRequest.Uri.OriginalString;
                        // Trim leading '/' to avoid problems
                        var resourceStreamUri = originalUriString.StartsWith("/", StringComparison.Ordinal) ? new Uri(originalUriString.TrimStart('/'), UriKind.Relative) : pendingRequest.Uri;
                        // Enqueue resource stream for completion
                        var streamResourceInfo = Application.GetResourceStream(resourceStreamUri);
                        if (null != streamResourceInfo)
                        {
                            pendingCompletions.Enqueue(new PendingCompletion(pendingRequest.Image, pendingRequest.Uri, streamResourceInfo.Stream));
                        }
                    }
                    // Yield to UI thread
                    Thread.Sleep(ThreadSleepTime);
                }
                // Process pending responses
                for (var i = 0; (i < pendingResponses.Count) && (i < WorkItemQuantum); i++)
                {
                    var pendingResponse = pendingResponses.Dequeue();
                    var responseState = (ResponseState)pendingResponse.AsyncState;
                    try
                    {
                        var response = responseState.WebRequest.EndGetResponse(pendingResponse);
                        pendingCompletions.Enqueue(new PendingCompletion(responseState.Image, responseState.Uri, response.GetResponseStream()));
                    }
                    catch (WebException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        // Ignore web exceptions (ex: not found)
                    }
                    // Yield to UI thread
                    Thread.Sleep(ThreadSleepTime);
                }
                // Process pending completions
                if (0 < pendingCompletions.Count)
                {
                    //TODO: shoudl save stream to disk here

                    // Get the Dispatcher and process everything that needs to happen on the UI thread in one batch
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        while (0 < pendingCompletions.Count)
                        {
                            // Decode the image and set the source
                            var pendingCompletion = pendingCompletions.Dequeue();
                            var bitmap = new BitmapImage();

                            try
                            {
                                bitmap.SetSource(pendingCompletion.Stream);

                                //if (bitmap != null &&
                                //    bitmap.PixelWidth < MaxCachedImageWidth &&
                                //    bitmap.PixelHeight < MaxCachedImageHeight &&
                                //    !webBitmapSourceCache.ContainsKey(pendingCompletion.Uri.OriginalString))
                                //{
                                //    webBitmapSourceCache.Add(pendingCompletion.Uri.OriginalString, bitmap);

                                //}

                                var filename = pendingCompletion.Uri.AbsoluteUri.Substring(27).Split('/')[0] + "small";

                                if (bitmap != null &&
                                bitmap.PixelWidth < MaxCachedImageWidth &&
                                bitmap.PixelHeight < MaxCachedImageHeight &&
                                !webBitmapSourceCache.ContainsKey(filename))
                                {
                                    webBitmapSourceCache.Add(filename, bitmap);

                                    IsoStore.SaveToIsoStore(filename, pendingCompletion.Stream);
                                }

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                // Ignore image decode exceptions (ex: invalid image)
                            }

                            if (GetUriSource(pendingCompletion.Image) == pendingCompletion.Uri)
                            {
                                pendingCompletion.Image.Source = bitmap;
                            }
                            else
                            {
                                // Uri mis-match; do nothing
                            }

                            // Dispose of response stream
                            pendingCompletion.Stream.Dispose();
                        }
                    });
                }
            }
        }

        public static void DeferDownloads()
        {
            IsPaused = true;
        }

        public static void ResumeDownloads()
        {
            lock (_syncBlock)
            {
                foreach (var kvp in _pausedRequests)
                {
                    _pendingRequests.Enqueue(new PendingRequest(kvp.Key, kvp.Value));
                }
                _pausedRequests.Clear();
                Monitor.Pulse(_syncBlock);
            }
            IsPaused = false;
        }

        private static void OnUriSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)o;
            var uri = (Uri)e.NewValue;

            if (!IsEnabled || DesignerProperties.IsInDesignTool)
            {
                // Avoid handing off to the worker thread (can cause problems for design tools)
                image.Source = new BitmapImage(uri);
            }
            else
            {
                BitmapSource bitmapSource = null;
                var filename = uri.AbsoluteUri.Substring(27).Split('/')[0] + "small";

                if (webBitmapSourceCache.TryGetCachedBitmapSource(filename, out bitmapSource))
                {                                        
                    image.Source = bitmapSource;
                                        Debug.WriteLine("Image cache hit");
                }
                else
                {
                    image.Source = null;

                    if (IsPaused)
                    {
                        _pausedRequests[image] = uri;
                    }
                    else
                    {
                        lock (_syncBlock)
                        {
                            // Enqueue the request
                            Debug.WriteLine("Request queued");
                            _pendingRequests.Enqueue(new PendingRequest(image, uri));
                            Monitor.Pulse(_syncBlock);
                        }
                    }
                }
            }
        }

        private static void HandleGetResponseResult(IAsyncResult result)
        {
            lock (_syncBlock)
            {
                // Enqueue the response
                _pendingResponses.Enqueue(result);
                Monitor.Pulse(_syncBlock);
            }
        }

        private class PendingRequest
        {
            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
            public PendingRequest(Image image, Uri uri)
            {
                Image = image;
                Uri = uri;
            }
        }

        private class ResponseState
        {
            public WebRequest WebRequest { get; private set; }
            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
            public ResponseState(WebRequest webRequest, Image image, Uri uri)
            {
                WebRequest = webRequest;
                Image = image;
                Uri = uri;
            }
        }

        private class PendingCompletion
        {
            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
            public Stream Stream { get; private set; }
            public PendingCompletion(Image image, Uri uri, Stream stream)
            {
                Image = image;
                Uri = uri;
                Stream = stream;
            }
        }
    }
}
