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
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;
using System.Windows.Media.Imaging;
using System.Text;
using Pages.Model.Helpers;

namespace Pages.Helpers
{
    public class Utilities
    {
        /// <summary>
        /// Check for internet connection
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetConnectionEnabled()
        {
            NetworkInterfaceType networkInterface = NetworkInterface.NetworkInterfaceType;

            // If the connection is None, return false otherwise return true.
            if (networkInterface.Equals(NetworkInterfaceType.None))
            {
                return false;
            }

            return true;
        }


        public static void HandleNoConnection()
        {
            //App.NavigateToPage(PageReferences.ConnectionFailurePage);
        }

        public static bool WriteToISO<T>(string key, T value)
        {
            if(string.IsNullOrEmpty(key))
            {
                return false;
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            
            settings[key] = value;

            return true;
        }

        public static T ReadFromISO<T>(string key)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            
            if (settings.Contains(key))
            {
                return (T)settings[key];
            }

            return default(T);
        }

        public static void RemoveFromISO(string key)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains(key))
            {
               settings.Remove(key);
            }

        }

        public static void Save<T>(string fileName, T item)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(fileStream, item);
                    }
                }
            }catch(Exception e)
            {
                EventLogger.Log("Error saving app data with exception: " + e.ToString());
            }
        }

        public static T Load<T>(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = storage.OpenFile(fileName, FileMode.Open))
                    //                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        return (T)serializer.ReadObject(fileStream);
                    }
                }
            }
            catch
            {
                throw;
            }            
        }

        public static void Deletefile(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    storage.DeleteFile(fileName);
                }
            }
            catch
            {
                throw;
            }
        }

        public static bool WriteTileToISO(string filename, WriteableBitmap WriteableBMP)
        {
            string imageFolder = @"\Shared\ShellContent";


            try
            {
                //Using Isolated storage for storage
                using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoFile.DirectoryExists(imageFolder))
                    {
                        isoFile.CreateDirectory(imageFolder);
                    }
                    string filePath = System.IO.Path.Combine(imageFolder, filename);
                    using (var stream = isoFile.CreateFile(filePath))
                    {
                        WriteableBMP.SaveJpeg(stream, WriteableBMP.PixelWidth, WriteableBMP.PixelHeight, 0, 100);
                    }
                }
            }
            catch
            {
                throw;
            }

            return true;
        }

        /// <summary>
        /// Converts the phrase to specified convention.
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="cases">The cases.</param>
        /// <returns>string</returns>
        public static string ConvertCaseString(string phrase)
        {
//            string[] splittedPhrase = phrase.Split(' ', '-', '.');
            string[] splittedPhrase = phrase.Split(' ', '-');
            var sb = new StringBuilder();


            //sb.Append(splittedPhrase[0].ToLower());
            //splittedPhrase[0] = string.Empty;
            //}
            //else if (cases == Case.PascalCase)
            sb = new StringBuilder();

            foreach (String s in splittedPhrase)
            {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private static Wp7Theme theme;

        public enum Wp7Theme
        {
            Dark,
            Light
        }

        public static string GetImagePath(string fileName)
        {
            var resource = Application.Current.Resources["PhoneForegroundColor"];
            
            if (resource != null)
            {
                theme = (Color)resource == Color.FromArgb(222, 0, 0, 0) ? Wp7Theme.Light : Wp7Theme.Dark;
            }
            else
            {
                theme = Wp7Theme.Dark;
            }

            var folder = theme == Wp7Theme.Dark ? "/Static/Images/dark/" : "/Static/Images/light/";
            return System.IO.Path.Combine(folder, fileName);
        }

        public static string GetAlbumIdFromLink(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return string.Empty;
            }



            return null;
        }

        public static Color GetColorFromHexString(string s)
        {
            // remove artifacts
            s = s.Trim().TrimStart('#');

            // only 8 (with alpha channel) or 6 symbols are allowed
            if (s.Length != 8 && s.Length != 6)
                throw new ArgumentException("Unknown string format!");

            int startParseIndex = 0;
            bool alphaChannelExists = s.Length == 8; // check if alpha canal exists            

            // read alpha channel value
            byte a = 255;
            if (alphaChannelExists)
            {
                a = System.Convert.ToByte(s.Substring(0, 2), 16);
                startParseIndex += 2;
            }

            // read r value
            byte r = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);
            startParseIndex += 2;
            // read g value
            byte g = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);
            startParseIndex += 2;
            // read b value
            byte b = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
