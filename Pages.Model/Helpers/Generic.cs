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
using Pages.Model;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Pages.Helpers
{
    public class GroupedOC<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The Group Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// This is used to colour the tiles - greying out those that have no entries
        /// </summary>
        public Brush GroupBackgroundBrush
        {
            get
            {
                return (SolidColorBrush)Application.Current.Resources[(HasItems) ?
                            "PhoneAccentBrush" : "PhoneChromeBrush"];
            }
        }

        /// <summary>
        /// Constructor ensure that a Group Title is included
        /// </summary>
        /// <param name="name">string to be used as the Group Title</param>
        public GroupedOC(string name)
        {
            this.Title = name;
        }

        /// <summary>
        /// Returns true if the group has a count more than zero
        /// </summary>
        public bool HasItems
        {
            get
            {
                return (Count != 0);
            }
            private set
            {
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged(String propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (null != handler)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }


    public static class CollectionHelpers
    {
        /// <summary>
        /// Groups a passed Contacts ObservableCollection
        /// </summary>
        /// <param name="InitialContactsList">Unordered collection of Contacts</param>
        /// <returns>Grouped Observable Collection of Contacts suitable for the LongListSelector</returns>
        public static ObservableCollection<GroupedOC<PageItemModel>> CreateGroupedOC(ObservableCollection<PageItemModel> InitialContactsList)
        {

            //Initialise the Grouped OC to populate and return
            ObservableCollection<GroupedOC<PageItemModel>> GroupedContacts = new ObservableCollection<GroupedOC<PageItemModel>>();

            //first sort our contacts collection into a temp List using LINQ
            var SortedList = (from con in InitialContactsList
                              orderby con.Name
                              select con).ToList();

            GroupedOC<PageItemModel> firstGOC = new GroupedOC<PageItemModel>("#");

            var SubsetOfCons1 = (from con in SortedList
                                where (!(con.Name[0] >= 'A' && con.Name[0] <= 'Z'))
                                select con).ToList<PageItemModel>();

            //Populate the GroupedOC
            foreach (PageItemModel csm in SubsetOfCons1)
            {
                firstGOC.Add(csm);
            }

            GroupedContacts.Add(firstGOC);

            //Now enumerate throw the alphabet creating empty groups objects
            //This ensure that the whole alphabet exists even if we never populate them
            string Alpha = "abcdefghijklmnopqrstuvwxyz";
            foreach (char c in Alpha)
            {
                //Create GroupedOC for given letter
                GroupedOC<PageItemModel> thisGOC = new GroupedOC<PageItemModel>(c.ToString());
                //thisGOC.Title = c.ToString();

                //Create a temp list with the appropriate Contacts that have this NameKey
                var SubsetOfCons = (from con in SortedList
                                    where con.Name.StartsWith(c.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                    select con).ToList<PageItemModel>();

                //Populate the GroupedOC
                foreach (PageItemModel csm in SubsetOfCons)
                {
                    thisGOC.Add(csm);
                }

                //Add this GroupedOC to the observable collection that is being returned
                // and the LongListSelector can be bound to.
                GroupedContacts.Add(thisGOC);
            }
            return GroupedContacts;
        }
    }

    class PageItemModelComparer : IEqualityComparer<PageItemModel>
    {
        #region IEqualityComparer<Contact> Members

        public bool Equals(PageItemModel x, PageItemModel y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(PageItemModel obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
