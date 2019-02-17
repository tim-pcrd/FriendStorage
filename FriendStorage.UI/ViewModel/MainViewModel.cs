using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using FriendStorage.UI.View.Services;
using Prism.Events;

namespace FriendStorage.UI.ViewModel
{
    public class MainViewModel : Observable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<IFriendEditViewModel> _friendEditViewModelCreator;
        private IFriendEditViewModel _selectedFriendEditViewModel;
        private readonly IMessageDialogService _messageDialogService;

        public MainViewModel(IEventAggregator eventAggregator, 
            INavigationViewModel navigationViewModel,
            Func<IFriendEditViewModel> friendEditViewModelCreator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _friendEditViewModelCreator = friendEditViewModelCreator;
            _messageDialogService = messageDialogService;
            NavigationViewModel = navigationViewModel;

            FriendEditViewModels = new ObservableCollection<IFriendEditViewModel>();
            CloseFriendTabCommand = new DelegateCommand(OnCloseFriendTabExecute);
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute);

            _eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendTab);
            _eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (FriendEditViewModels.Any(f => f.Friend.IsChanged))
            {
                var result = _messageDialogService.ShowYesNoDialog("Close application?",
                    "You'll lose your changes if you close this application. Close it?", MessageDialogResult.No);
                e.Cancel = result == MessageDialogResult.No;
            }
        }

        public INavigationViewModel NavigationViewModel { get; }

        public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; }

        public IFriendEditViewModel SelectedFriendEditViewModel
        {
            get { return _selectedFriendEditViewModel; }
            set
            {
                _selectedFriendEditViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddFriendCommand { get; }
        public ICommand CloseFriendTabCommand { get; }

        private void OnAddFriendExecute(object obj)
        {
            IFriendEditViewModel friendEditVm = _friendEditViewModelCreator();
            FriendEditViewModels.Add(friendEditVm);
            friendEditVm.Load();
            SelectedFriendEditViewModel = friendEditVm;
        }

        private void OnCloseFriendTabExecute(object obj)
        {
            var friendEditVmToClose = obj as IFriendEditViewModel;
            if (friendEditVmToClose != null)
            {
                // TODO: Check if the Friend has changes and ask user to cancel
                if (friendEditVmToClose.Friend.IsChanged)
                {
                    var result = _messageDialogService.ShowYesNoDialog("Close Tab?",
                        $"There are unsaved changes, do you really want to close this tab?",
                        MessageDialogResult.No);

                    if (result == MessageDialogResult.No)
                    {
                        SelectedFriendEditViewModel = friendEditVmToClose;
                        return;
                    }
                }
                FriendEditViewModels.Remove(friendEditVmToClose);
            }
        }

        private void OnFriendDeleted(int friendId)
        {
            IFriendEditViewModel friendEditVmToClose =
                FriendEditViewModels.SingleOrDefault(f => f.Friend.Id == friendId);
            if (friendEditVmToClose != null)
            {
                FriendEditViewModels.Remove(friendEditVmToClose);
            }
        }

        private void OnOpenFriendTab(int friendId)
        {
            IFriendEditViewModel friendEditVm = FriendEditViewModels.SingleOrDefault(vm => vm.Friend.Id == friendId);
            if (friendEditVm == null)
            {
                friendEditVm = _friendEditViewModelCreator();
                FriendEditViewModels.Add(friendEditVm);
                friendEditVm.Load(friendId);
            }

            SelectedFriendEditViewModel = friendEditVm;
        }

       
    }
}
