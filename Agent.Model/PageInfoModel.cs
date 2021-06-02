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
using System.Runtime.Serialization;
using System.ComponentModel;
using Pages.Model.Helpers;
using Newtonsoft.Json;

namespace Pages.Model
{
    [DataContract]
    public class PageInfoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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

                return Utilities.ConvertCaseString(_name);
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

        private string _imageURLLarge;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string ImageURLLarge
        {
            get
            {
                return _imageURLLarge;
            }
            set
            {
                if (value != _imageURLLarge)
                {
                    _imageURLLarge = value;

                    NotifyPropertyChanged("ImageURLLarge");
                }
            }
        }

        private string _category;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    NotifyPropertyChanged("Category");
                }
            }
        }

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
                    this.ImageURLLarge = FBHelper.GetImageUrl(_id, "large");

                    NotifyPropertyChanged("Id");
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

        private string _website;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Website
        {
            get
            {
                return _website;
            }
            set
            {
                if (value != _website)
                {
                    _website = value;

                    NotifyPropertyChanged("Website");
                }
            }
        }

        private string _about;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string About
        {
            get
            {
                return _about;
            }
            set
            {
                if (value != _about)
                {
                    _about = value;

                    NotifyPropertyChanged("About");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;

                    NotifyPropertyChanged("Description");
                }
            }
        }

        private string _likes;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Likes
        {
            get
            {
                return _likes;
            }
            set
            {
                if (value != _likes)
                {
                    _likes = value;

                    NotifyPropertyChanged("Likes");
                }
            }
        }

        private string _talkingcount;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Talking_about_count
        {
            get
            {
                return _talkingcount;
            }
            set
            {
                if (value != _talkingcount)
                {
                    _talkingcount = value;

                    NotifyPropertyChanged("Talking_about_count");
                }
            }
        }

        public static PageInfoModel FromJson(string json)
        {
            PageInfoModel model = JsonConvert.DeserializeObject<PageInfoModel>(json);
            model.ImageURLLarge = FBHelper.GetImageUrl(model.Id, "large");

            return model;
        }


    }
}
