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
using System.Collections.ObjectModel;

namespace Pages.Model.Helpers
{
    [DataContract]
    public class PinnedPagesData
    {

        [DataMember]
        public ObservableCollection<DetailsViewModel> PinnedPages { get; set; }

        public PinnedPagesData()
        {
            this.PinnedPages = new ObservableCollection<DetailsViewModel>();
        }

    }
}
