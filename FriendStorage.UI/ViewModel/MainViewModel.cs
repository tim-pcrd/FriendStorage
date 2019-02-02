using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace FriendStorage.UI.ViewModel
{
    public class MainViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public MainViewModel(IEventAggregator eventAggregator, INavigationViewModel navigationViewModel)
        {
            _eventAggregator = eventAggregator;
            NavigationViewModel = navigationViewModel;
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }

        public INavigationViewModel NavigationViewModel { get; }
    }
}
