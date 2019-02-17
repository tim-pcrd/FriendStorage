using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;
using FriendStorage.UI.ViewModel;

namespace FriendStorage.UI.Wrapper
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {
            
        }

      
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int FriendGroupId
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int FriendGroupIdOriginalValue => GetOriginalValue<int>(nameof(FriendGroupId));

        public bool FriendGroupIdIsChanged => GetIsChanged(nameof(FriendGroupId));

        
        public string FirstName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName));

        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));


        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName));

        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));


        public DateTime? Birthday
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public DateTime? BirthdayOriginalValue => GetOriginalValue<DateTime?>(nameof(Birthday));

        public bool BirthdayIsChanged => GetIsChanged(nameof(Birthday));


        public bool IsDeveloper
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IsDeveloperOriginalValue => GetOriginalValue<bool>(nameof(IsDeveloper));

        public bool IsDeveloperIsChanged => GetIsChanged(nameof(IsDeveloper));


        public AddressWrapper Address { get; private set; }

        public ChangeTrackingCollection<FriendEmailWrapper> Emails { get;private set; }

        protected override void InitializeComplexProperties(Friend model)
        {
            if (model.Address == null)
            {
                throw new ArgumentException("Address cannot be null");
            }
            this.Address= new AddressWrapper(model.Address);
            RegisterComplex(Address);

        }

        protected override void InitializeCollectionProperties(Friend model)
        {

            if (model.Emails == null)
            {
                throw new ArgumentException("Emails cannot be null");
            }

            //this.Emails = new ObservableCollection<FriendEmailWrapper>();
            //foreach (var email in model.Emails)
            //{
            //    Emails.Add(new FriendEmailWrapper(email));
            //}

            this.Emails = new ChangeTrackingCollection<FriendEmailWrapper>(
                model.Emails.Select(e => new FriendEmailWrapper(e)));
            RegisterCollection(Emails, model.Emails);

            //Emails.CollectionChanged += (sender, args) =>
            //{
            //    model.Emails = new List<FriendEmail>(Emails.Select(ew => ew.Model));
            //};
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("Firstname is required", new []{nameof(FirstName)});
            }

            if (IsDeveloper && Emails.Count == 0)
            {
                yield return new ValidationResult("A developer must have an email-address",
                    new []{nameof(IsDeveloper),nameof(Emails)});
            }
        }


        //private void RegisterCollection(ObservableCollection<FriendEmailWrapper> wrapperCollection, List<FriendEmail> modelCollection)
        //{
        //    wrapperCollection.CollectionChanged += (sender, args) =>
        //    {
        //        if (args.OldItems != null)
        //        {
        //            foreach (var item in args.OldItems.Cast<FriendEmailWrapper>())
        //            {
        //                modelCollection.Remove(item.Model);
        //            }
        //        }

        //        if (args.NewItems != null)
        //        {
        //            foreach (var item in args.NewItems.Cast<FriendEmailWrapper>())
        //            {
        //                modelCollection.Add(item.Model);
        //            }
        //        }
        //    };

        //}
    }
}
