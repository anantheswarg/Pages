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
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using Pages.Model;
using Pages.Helpers;

namespace Pages.Converters
{
    public class PageNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string name = (string) value;

                name = name.Trim();

                if (name.Length > 16)
                {
                    name = name.Substring(0, 16);

                    name += "..";

                }

                return name;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        
    }

    public class AlbumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string name = (string)value;

                name = name.Trim();

                if (name.Length > 22)
                {
                    name = name.Substring(0, 22);

                    name += "..";

                }

                return name;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class UpperCaseConverer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {

                string name = (string)value;

                name = name.ToUpper(CultureInfo.InvariantCulture);

                return name;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class ImageCacheConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageCacher.GetCacheImage(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageCacheLargeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {

                BitmapImage image = ImageCacher.GetCacheImage(value.ToString(), "large");

                return image;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageURLLargeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return value.ToString() + "&type=large";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class BooleanToVisiblity : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool v = (bool)value;
                if (!v)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CountToVisiblity : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int v = (int)value;
                if (v > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StringToVisiblity : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string v = (string)value;

                if (string.IsNullOrEmpty(v))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CommentCountToString : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int v = (int)value;

                if (v > 0)
                {
                    return v.ToString() + " comments";
                }
                else
                {
                    return "";
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class LikeCountToString : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int v = (int)value;

                if (v > 0)
                {
                    return v.ToString() + " likes";
                }
                else
                {
                    return "";
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ReverseBooleanToVisiblity : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool v = (bool)value;
                if (v)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DateTimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is string)
            {
                DateTime createdtime;
                if(DateTime.TryParse(value as string, out createdtime))
                {

                    //DateTime temp = createdtime.ToLocalTime();
                    TimeSpan diffSpan = DateTime.Now.Subtract(createdtime);
                    if (diffSpan.TotalDays < 1)
                    {
                        if (Math.Floor(diffSpan.TotalHours) < 1)
                        {
                            if (diffSpan.TotalMinutes < 1)
                            {
                                return "just now";
                            }
                            else if (diffSpan.TotalMinutes == 1)
                            {
                                return "a minute ago";
                            }
                            else if (diffSpan.TotalMinutes < 60)
                            {
                                return Math.Floor(diffSpan.TotalMinutes).ToString() + " minutes ago";
                            }
                        }
                        if (Math.Floor(diffSpan.TotalHours) == 1)
                        {
                            return "about an hour ago";
                        }
                        else
                        {
                            return Math.Floor(diffSpan.TotalHours).ToString() + " hours ago";
                        }

                    }
                    else if (diffSpan.TotalDays == 1)
                    {
                        return "yesterday";
                    }                    
                    else
                    {
                        return String.Format("{0:D}", createdtime);
                        //return Math.Floor(diffSpan.TotalDays).ToString() + " days ago";
                    }
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class ListBoxContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CommenterViewModel)
            {
                return Application.Current.Resources["CommentsUserControl"] as DataTemplate;
            }
            else if (value is PostViewModel)
            {
                //    return Button;
                return Application.Current.Resources["PostMessageCotrol"] as DataTemplate;
            }
            else if (value is string)
            {
                //    return Button;
                return Application.Current.Resources["AddCommentUserControl"] as DataTemplate;
            }
            else
            {
                return Application.Current.Resources["AddCommentUserControl"] as DataTemplate;

                //                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}
