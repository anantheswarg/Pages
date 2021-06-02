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
using Pages.Helpers;
using System.Runtime.Serialization;

namespace Pages.Model
{
    [DataContract]
    public class AlbumViewModel : ViewModelBase
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
                    
                    NotifyPropertyChanged("Id");
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

                    datetime = datetime.ToLocalTime();

                    _createdtime = String.Format("{0:dd MMM yyyy h:mm tt}", datetime);

                    NotifyPropertyChanged("Created_time");
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


        public static AlbumViewModel FromJson(string json)
        {
            AlbumViewModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<AlbumViewModel>(json);

            model.ImageURL = FBHelper.GetImageUrl(model.Id);

            return model;
        }

        

    }
}
