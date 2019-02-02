using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using Prism.Events;

namespace FriendStorage.UI.ViewModel
{
    public class NavigationItemViewModel : Observable
    {
        private string _displayValue;
        private readonly IEventAggregator _eventAggregator;

        public NavigationItemViewModel(int friendId,string displayValue, IEventAggregator eventAggregator)
        {
            FriendId = friendId;
            DisplayValue = displayValue;
            _eventAggregator = eventAggregator;
            OpenFriendEditViewCommand = new DelegateCommand(OpenFriendEditViewExecute);
        }

        public int FriendId { get; }

        public string DisplayValue
        {
            get { return _displayValue; }
            set
            {
                _displayValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFriendEditViewCommand { get; }

        private void OpenFriendEditViewExecute(object obj)
        {
            _eventAggregator.GetEvent<OpenFriendEditViewEvent>().Publish(FriendId);
        }
    }
}
