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
using Pages.Helpers;
using System.Runtime.Serialization;

namespace Pages.Model
{
    [DataContract]
    public class PhotoViewModel : ViewModelBase
    {
        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;

                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _name;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;

                    NotifyPropertyChanged("Name");
                }
            }
        }

        private bool _isLargeImageLoaded;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public bool IsLargeImageLoaded
        {
            get
            {
                return _isLargeImageLoaded;
            }
            set
            {
                if (value != _isLargeImageLoaded)
                {
                    _isLargeImageLoaded = value;

                    NotifyPropertyChanged("IsLargeImageLoaded");
                }
            }
        }

        private string _picture;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Picture
        {
            get
            {

                return _picture;
            }
            set
            {
                if (value != _picture)
                {
                    _picture = value;
                    NotifyPropertyChanged("Picture");
                }
            }
        }

        private string _source;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Source
        {
            get
            {

                return _source;
            }
            set
            {
                if (value != _source)
                {
                    _source = value;
                    NotifyPropertyChanged("Source");
                }
            }
        }

        private int _height;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public int Height
        {
            get
            {

                return _height;
            }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        [DataMember]
        private int _width;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        public static PhotoViewModel FromJson(string json)
        {
            PhotoViewModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoViewModel>(json);

            return model;
        }
    }
}
