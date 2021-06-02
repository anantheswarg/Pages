using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Agent.Model;

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
