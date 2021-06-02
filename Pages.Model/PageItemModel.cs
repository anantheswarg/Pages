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
using System.ComponentModel;
using Newtonsoft.Json;
using Pages.Helpers;
using System.Runtime.Serialization;
using System.Globalization;
using System.Threading;
using System.Text;

namespace Pages.Model
{
    
    public class PageItemModel : INotifyPropertyChanged, IEquatable<PageItemModel>
    {

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
                
                return  Utilities.ConvertCaseString(_name);
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

        private DateTime _accessedtime;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public DateTime AccessedTime
        {
            get
            {
                return _accessedtime;
            }
            set
            {
                if (value != _accessedtime)
                {
                    _accessedtime = value;
                    NotifyPropertyChanged("AccessedTime");
                }
            }
        }

        private string _createdtime;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

        [DataMember]
        public string Created_time
        {
            get
            {
                return _createdtime;
            }
            set
            {
                if (value != _createdtime)
                {
                    _createdtime = value;

                    DateTime datetime;

                    DateTime.TryParse(_createdtime, out datetime);

                    datetime = datetime.ToUniversalTime();

                    _createdtime = String.Format("{0:dd MMM yyyy h:mm tt}", datetime);

                    NotifyPropertyChanged("Created_time");
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
                    this.ImageURL = FBHelper.GetImageUrl(_id);
                    this.ImageURLLarge = FBHelper.GetImageUrl(_id, "large");
                    
                    NotifyPropertyChanged("Id");
                }
            }
        }



        private string _imageURL;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string ImageURL
        {
            get
            {
                return _imageURL;
            }
            set
            {
                if (value != _imageURL)
                {
                    _imageURL = value;

                    NotifyPropertyChanged("ImageURL");
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

        //public PageItemModel(string name, string imageurl)
        //{
        //    this.Name = name;
        //    this.ImageURL = imageurl;
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static PageItemModel FromJson(string json)
        {
            PageItemModel model = JsonConvert.DeserializeObject<PageItemModel>(json);
            
            model.ImageURL = FBHelper.GetImageUrl(model.Id);
            model.ImageURLLarge = FBHelper.GetImageUrl(model.Id, "large");

            return model;
        }

        public bool Equals(PageItemModel o)
        {
            return o.Id.Equals(Id);
        }

        
    }
}
