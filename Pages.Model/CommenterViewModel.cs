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
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Pages.Model
{
    public class CommenterViewModel: ViewModelBase
    {
        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

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

        private string _from;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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


        private string _message;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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

        private string _createdtime;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>

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

                    if (DateTime.TryParse(_createdtime, out datetime))
                    {

                        datetime = datetime.ToLocalTime();

                        _createdtime = String.Format("{0:dd MMM yyyy h:mm tt}", datetime);

                        NotifyPropertyChanged("Created_time");
                    }
                }
            }
        }


        public static CommenterViewModel FromJson(string json)
        {
            CommenterViewModel model = JsonConvert.DeserializeObject<CommenterViewModel>(json);


            return model;
        }

    }
}
