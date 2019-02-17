using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendStorage.Model;
using FriendStorage.UI.Command;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.DataProvider.Lookups;
using FriendStorage.UI.Events;
using FriendStorage.UI.View.Services;
using FriendStorage.UI.Wrapper;
using Prism.Events;

namespace FriendStorage.UI.ViewModel
{
    class FriendEditViewModel : Observable, IFriendEditViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IFriendDataProvider _friendDataProvider;
        private readonly ILookupProvider<FriendGroup> _friendGroupLookupProvider;
        private FriendWrapper _friend;
        private IEnumerable<LookupItem> _friendGroupLookup;
        private FriendEmailWrapper _selectedEmail;

        public FriendEditViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IFriendDataProvider friendDataProvider,
            ILookupProvider<FriendGroup> friendGourpLookupProvider)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _friendDataProvider = friendDataProvider;
            _friendGroupLookupProvider = friendGourpLookupProvider;

            SaveCommand = new DelegateCommand(OnSaveExecute,OnSaveCanExecute);
            ResetCommand = new DelegateCommand(OnResetExecute,OnResetCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute,OnDeleteCanExecute);

            AddEmailCommand = new DelegateCommand(OnAddEmailExecute);
            RemoveEmailCommand = new DelegateCommand(OnRemoveEmailExecute, OnRemoveEmailCanExecute);
        }

        public void Load(int? friendId = null)
        {
            FriendGroupLookup = _friendGroupLookupProvider.GetLookups();

            var friend = friendId.HasValue
                ? _friendDataProvider.GetFriendById(friendId.Value)
                : new Friend() {Address = new Address(), Emails = new List<FriendEmail>()};

            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Friend.IsChanged) || args.PropertyName == nameof(Friend.IsValid))
                {
                    InValidateCommands();
                }
            };
            InValidateCommands();
        }

      

        public FriendWrapper Friend
        {
            get { return _friend; }
            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }


        public IEnumerable<LookupItem> FriendGroupLookup
        {
            get { return _friendGroupLookup; }
            set
            {
                _friendGroupLookup = value;
                OnPropertyChanged();
            }
        }


        public FriendEmailWrapper SelectedEmail
        {
            get { return _selectedEmail; }
            set
            {
                _selectedEmail = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveEmailCommand).RaiseCanExecuteChanged();
            }
        }


        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddEmailCommand { get; }
        public ICommand RemoveEmailCommand { get; }

        private bool OnSaveCanExecute(object arg)
        {
            return Friend.IsChanged && Friend.IsValid;
        }

        private void OnSaveExecute(object obj)
        {
            _friendDataProvider.SaveFriend(Friend.Model);
            Friend.AcceptChanges();
            _eventAggregator.GetEvent<FriendSavedEvent>().Publish(Friend.Model);
            InValidateCommands();
        }

        private bool OnResetCanExecute(object arg)
        {
            return Friend.IsChanged;
        }

        private void OnResetExecute(object obj)
        {
            Friend.RejectChanges();
        }

        private bool OnDeleteCanExecute(object arg)
        {
            return Friend != null && Friend.Id > 0;
        }

        private void OnDeleteExecute(object obj)
        {
            var result = _messageDialogService.ShowYesNoDialog("Delete Friend",
                $"Do you really want to delete the friend {Friend.FirstName} {Friend.LastName}",
                MessageDialogResult.No);

            if (result == MessageDialogResult.Yes)
            {
                _friendDataProvider.DeleteFriend(Friend.Id);
                _eventAggregator.GetEvent<FriendDeletedEvent>().Publish(Friend.Id);
            }
        }

        private void OnAddEmailExecute(object obj)
        {
            Friend.Emails.Add(new FriendEmailWrapper(new FriendEmail()));
        }

        private bool OnRemoveEmailCanExecute(object arg)
        {
            return SelectedEmail != null;
        }

        private void OnRemoveEmailExecute(object obj)
        {
            Friend.Emails.Remove(SelectedEmail);
            ((DelegateCommand)RemoveEmailCommand).RaiseCanExecuteChanged();
        }

        private void InValidateCommands()
        {
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ResetCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

    }
}
