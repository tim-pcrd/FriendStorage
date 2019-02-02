using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;
using FriendStorage.UI.DataProvider.Lookups;
using Prism.Events;

namespace FriendStorage.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILookupProvider<Friend> _friendLookupProvider;

        public NavigationViewModel(IEventAggregator eventAggregator, ILookupProvider<Friend> friendLookupProvider)
        {
            _eventAggregator = eventAggregator;
            _friendLookupProvider = friendLookupProvider;
            NavigationItems = new ObservableCollection<NavigationItemViewModel>();
        }

        public ObservableCollection<NavigationItemViewModel> NavigationItems { get; }

        public void Load()
        {
            NavigationItems.Clear();
            foreach (var friendLookupItem in _friendLookupProvider.GetLookups())
            {
                NavigationItems.Add(
                    new NavigationItemViewModel(friendLookupItem.Id,friendLookupItem.DisplayValue,_eventAggregator));
            }
        }
    }
}
