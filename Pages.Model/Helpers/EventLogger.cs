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
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;

namespace Pages.Model.Helpers
{
    public class EventLogger
    {
        private static string filename = "EventLog.txt";
        private static string EXCEPTION_LOG_FILE = "ExceptionLog.txt";
        
        public static void Log(string message, bool isFile = true, bool isCrash = false)
        {
            if (isCrash)
            {
                if (isFile)
                {
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                    StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(filename, FileMode.Append, file));

                    sw.WriteLine(DateTime.Now.ToString() + ": " + message); //Wrting to the file
                    sw.Close();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
        }

        public static void LogException(string message, Exception e)
        {
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(EXCEPTION_LOG_FILE, FileMode.Append, file));

                sw.WriteLine(DateTime.Now.ToString() + ": " + message); //Wrting to the file
                sw.WriteLine("StackTrace: " + e.StackTrace);
                sw.Close();
            }
            catch
            { }
        }

        public static string GetLog()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(filename, FileMode.Open, file));
            string rawData = reader.ReadToEnd();
            reader.Close();

            return rawData;
        }

        public static void ClearLog()
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    storage.DeleteFile(filename);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
