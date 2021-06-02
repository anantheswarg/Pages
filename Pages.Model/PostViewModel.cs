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
using System.ComponentModel;

namespace Pages.Model
{
    [DataContract]
    public class PostViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
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

        private string _link;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                if (value != _link)
                {
                    _link = value;

                    NotifyPropertyChanged("Link");
                }
            }
        }

        private string _from;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                if (value != _from)
                {
                    _from = value;

                    NotifyPropertyChanged("From");
                }
            }
        }

        private string _caption;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                if (value != _caption)
                {
                    _caption = value;

                    NotifyPropertyChanged("Caption");
                }
            }
        }

        private string _type;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;

                    NotifyPropertyChanged("Type");
                }
            }
        }

        private string _message;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;

                    NotifyPropertyChanged("Message");
                }
            }
        }

        private string _story;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Story
        {
            get
            {
                return _story;
            }
            set
            {
                if (value != _story)
                {
                    _story = value;

                    NotifyPropertyChanged("Story");
                }
            }
        }

        private int _likes;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public int Likes
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

        private int _comments;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public int Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                if (value != _comments)
                {
                    _comments = value;

                    NotifyPropertyChanged("Comments");
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
                //DateTime datetime;

                //if (DateTime.TryParse(_createdtime, out datetime))
                //{

                //    datetime = datetime.ToLocalTime();

                //    _createdtime = String.Format("{0:dd MMM yyyy h:mm tt}", datetime);

                //}
                return _createdtime;

            }
            set
            {
                if (value != _createdtime)
                {
                    _createdtime = value;
                    NotifyPropertyChanged("Created_time");
                    //DateTime datetime;

                    //if (DateTime.TryParse(_createdtime, out datetime))
                    //{

                    //    //datetime = datetime.ToLocalTime();

                    //    _createdtime = String.Format("{0:dd MMM yyyy h:mm tt}", datetime);

                    //    NotifyPropertyChanged("Created_time");
                    //}
                }
            }
        }
    }
}
