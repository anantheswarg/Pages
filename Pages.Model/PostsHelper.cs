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
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Pages.Model
{
    [DataContract]
    public class PostsHelper
    {
        [DataMember]
        public ObservableCollection<PostViewModel> Posts { get; set; }

        public PostsHelper()
        {
            this.Posts = new ObservableCollection<PostViewModel>();
        }
    }
}
