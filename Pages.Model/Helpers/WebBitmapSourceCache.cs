using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using Pages.Helpers;

namespace Pages
{
    /// <summary>
    /// An image cache based on Uri lookups that allows you to explicitly set
    /// your cache size and automatically performs evictions via MRU lifetime management.
    /// </summary>
    [DataContract]

    public class WebBitmapSourceCache
    {
        /// <summary>
        /// Synchronizes access to the cache since its a pretty mainline scenario to
        /// access this from background threads.
        /// </summary>
        private object syncLock = new object();

        // Track cache hits/misses so that we can track the efficiency of the cache
        // in the debugger.
        //[DataMember]
        public long cacheHits { get; set; }
        //[DataMember]
        public long cacheMisses { get; set; }

        // Track the capacity and current size of the cache.
        [DataMember]
        public long cacheCapacityInBytes { get; set; }

        [DataMember]
        public bool isCreationDone { get; set; }

        //[DataMember]
        public long cacheCurrentSizeInBytes { get; set; }
        public const int bytesPerPixel = 4;

        /// <summary>
        /// This is the actual cache that this class manages.
        /// </summary>
        [DataMember]
        public Dictionary<string /* AbsoluteUri */, ImageCacheRecord> imageCache = new Dictionary<string, ImageCacheRecord>();

        /// <summary>
        /// Computes the efficiency of the cache and returns it as a percentage 0-100 based percentage.
        /// </summary>
        internal double CacheEfficacy { get { return (cacheHits / (double)(cacheHits + cacheMisses)) * 100.0; } }

        /// <summary>
        /// Image cache record.
        /// </summary>
        [DataContract]
        public class ImageCacheRecord
        {
            /// <summary>
            /// Gets the BitmapSource that this cache record represents.
            /// </summary>
            //[DataMember]
            public BitmapImage BitmapSource { get;  set; }

            /// <summary>
            /// Save the last time that the cache record was accessed so we know what to evict.
            /// </summary>
            [DataMember]
            public DateTime LastAccessed { get;  set; }

            /// <summary>
            /// Gets the size in bytes of the cached bitmap so we can compute how
            /// close to capacity the cache is.
            /// </summary>
            [DataMember]
            public long SizeInBytes { get;  set; }

            /// <summary>
            /// Initializes a new instance of the ImageCacheRecord class.
            /// </summary>
            /// <param name="bitmapSource">The bitmap source to cache.</param>
            public ImageCacheRecord(BitmapImage bitmapSource)
            {
                this.BitmapSource = bitmapSource;
                this.LastAccessed = DateTime.UtcNow;
                this.SizeInBytes = bitmapSource.PixelWidth * bitmapSource.PixelHeight * bytesPerPixel;
            }

            /// <summary>
            /// Updates the last time that this cache record was accessed so that we can
            /// do MRU based cache management.
            /// </summary>
            public void UpdateLastAccessed()
            {
                this.LastAccessed = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WebBitmapSourceCache class with a given
        /// capacity in bytes.
        /// </summary>
        /// <param name="cacheCapacityInBytes">The capacity of the cache in bytes.</param>
        public WebBitmapSourceCache(long cacheCapacityInBytes)
        {
            this.cacheCapacityInBytes = cacheCapacityInBytes;
        }

        /// <summary>
        /// Tries to get a cached bitmap for the given absolute Uri.
        /// </summary>
        /// <param name="absoluteUri">The Uri to check the cache for.</param>
        /// <param name="bitmapSource">A reference where the cache hit can be returned.</param>
        /// <returns>True if a bitmap was retrieved from the cache, false if the cache missed.</returns>
        public bool TryGetCachedBitmapSource(string filename, out BitmapSource bitmapSource)
        {
            bitmapSource = null;

            ImageCacheRecord cacheRecord = null;
            if (imageCache.TryGetValue(filename, out cacheRecord))
            {
                if (cacheRecord.BitmapSource != null)
                {
                    bitmapSource = cacheRecord.BitmapSource;
                }
                else
                {
                    if (IsoStore.FileExists(filename))
                    {
                        try
                        {
                            var result = new BitmapImage();
                            result.SetSource(IsoStore.StreamFileFromIsoStore(filename));
                            bitmapSource = result;

                            cacheRecord.BitmapSource = result;
                        }
                        catch { }
                    }

                }
                cacheRecord.UpdateLastAccessed();
                cacheHits++;
            }
            else
            {
                cacheMisses++;
            }

            return bitmapSource != null;
        }

        /// <summary>
        /// Gets whether the given key is contained in the cache.
        /// </summary>
        /// <param name="absoluteUri">The key to check for.</param>
        /// <returns>True if the key was contained in the cache, false otherwise.</returns>
        public bool ContainsKey(string absoluteUri)
        {
            bool isContained = false;

            lock (this.syncLock)
            {
                isContained = imageCache.ContainsKey(absoluteUri);
            }

            return isContained;
        }

        /// <summary>
        /// Adds a new bitmap to the cache for the given absolute Uri.
        /// </summary>
        /// <param name="absoluteUri">The Uri to insert a cached bitmap for.</param>
        /// <param name="bitmapSource">The bitmap to cache.</param>
        public void Add(string absoluteUri, BitmapImage bitmapSource)
        {
            lock (this.syncLock)
            {
                var newCacheRecord = new ImageCacheRecord(bitmapSource);

                // If we're going to blow our cacheSize we need to evict some entries.
                if (cacheCurrentSizeInBytes + newCacheRecord.SizeInBytes > cacheCapacityInBytes)
                {
                    var mruCache = from entry in imageCache
                                   orderby entry.Value.LastAccessed ascending
                                   select entry;

                    // We have to convert this to a list since Linq will delay access to the imageCache
                    // dictionary while we're simultaneously modifying it.
                    var mruCacheList = mruCache.ToList();

                    for (int i = 0; i < mruCacheList.Count && cacheCurrentSizeInBytes + newCacheRecord.SizeInBytes > cacheCapacityInBytes; i++)
                    {
                        UnsynchronizedRemoveRecordFromCache(mruCacheList[i].Key);
                    }
                }

                imageCache.Add(absoluteUri, newCacheRecord);
                cacheCurrentSizeInBytes += newCacheRecord.SizeInBytes;
            }
        }

        /// <summary>
        /// Removes the cached bitmap at the given absolute Uri.
        /// </summary>
        /// <param name="absoluteUri">The Uri to remove a cached bitmap for.</param>
        /// <returns>True if an entry was removed, false if it didn't exist.</returns>
        public bool Remove(string absoluteUri)
        {
            bool removed = false;

            lock (this.syncLock)
            {
                removed = UnsynchronizedRemoveRecordFromCache(absoluteUri);
            }

            return removed;
        }

        /// <summary>
        /// Allows unsynchronized removal of entries from the cache.  This is used by the Add method
        /// which already has the lock.
        /// </summary>
        /// <param name="absoluteUri">The absolute Uri for the cache entry to remove.</param>
        /// <returns>True if an entry was removed, false if it didn't exist.</returns>
        private bool UnsynchronizedRemoveRecordFromCache(string absoluteUri)
        {
            bool removed = false;

            ImageCacheRecord cacheRecord = null;
            if (imageCache.TryGetValue(absoluteUri, out cacheRecord))
            {
                this.cacheCurrentSizeInBytes -= cacheRecord.SizeInBytes;
                removed = imageCache.Remove(absoluteUri);
                Debug.Assert(removed == true);
            }

            return removed;
        }

        /// <summary>
        /// Clears the image cache.
        /// </summary>
        private void Clear()
        {
            lock (this.syncLock)
            {
                imageCache.Clear();
                this.cacheCurrentSizeInBytes = 0;
            }
        }
    }
}