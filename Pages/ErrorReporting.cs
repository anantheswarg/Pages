﻿using System;
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
using Microsoft.Phone.Tasks;

namespace Pages
{
    public class LittleWatson
    {
        const string filename = "ExceptionLog.txt";

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    SafeDeleteFile(store);
                    using (TextWriter output = new StreamWriter(store.CreateFile(filename)))
                    {
                        output.WriteLine(extra);
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(filename))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }
                        SafeDeleteFile(store);
                    }
                }
                if (contents != null)
                {
                    if (MessageBox.Show("A problem occurred the last time you ran this application. Would you like to send an email to report it?", "Crash Report", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = "developer@alphaapps.co.in";
                        email.Subject = "Pages auto-generated crash report";
                        email.Body = contents;
                        SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication()); // line added 1/15/2011
                        email.Show();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(filename);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class LittleWatsonBakground
    {
        const string filename = "BackgroundThreadLog.txt";

        //internal static void ReportException(Exception ex, string extra)
        //{
        //    try
        //    {
        //        using (var store = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            SafeDeleteFile(store);
        //            using (TextWriter output = new StreamWriter(store.CreateFile(filename)))
        //            {
        //                output.WriteLine(extra);
        //                output.WriteLine(ex.Message);
        //                output.WriteLine(ex.StackTrace);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        internal static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(filename))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }
                        SafeDeleteFile(store);
                    }
                }
                if (contents != null)
                {
                    if (MessageBox.Show("A problem occurred the last time you ran this application. Would you like to send an email to report it?", "Crash Report", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = "developer@alphaapps.co.in";
                        email.Subject = "Pages auto-generated Background Agent crash report";
                        email.Body = contents;
                        SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication()); // line added 1/15/2011
                        email.Show();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(filename);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
