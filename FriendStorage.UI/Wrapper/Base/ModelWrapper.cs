using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;
using FriendStorage.UI.ViewModel;

namespace FriendStorage.UI.Wrapper
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject, IValidatableObject
    {
        private readonly Dictionary<string, object> _originalValues;
        private readonly List<IValidatableTrackingObject> _trackingObjects;

        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IValidatableTrackingObject>();
            //Deze 2 methodes moeten uitgevoerd worden voor Validate()
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
            Validate();
        }

       

        public T Model { get; private set; } 

        public bool IsChanged => _originalValues.Count > 0 || _trackingObjects.Any(t => t.IsChanged);

        public bool IsValid
        {
            get { return !HasErrors && _trackingObjects.All(t => t.IsValid); }
        }

        public void AcceptChanges()
        {
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.AcceptChanges();
            }
            OnPropertyChanged("");
        }

        public void RejectChanges()
        {
            foreach (var originalValueEntry in _originalValues)
            {
                Model.GetType().GetProperty(originalValueEntry.Key)
                    .SetValue(Model,originalValueEntry.Value);
            }
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.RejectChanges();
            }
            Validate();
            OnPropertyChanged("");
        }

        protected virtual void InitializeCollectionProperties(T model)
        {
        }

        protected virtual void InitializeComplexProperties(T model)
        {
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);
            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName);
                propertyInfo.SetValue(Model, newValue);
                Validate();
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName+"IsChanged");
            }
        }

        private void Validate()
        {
            ClearErrors();

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, true);
            if (results.Any())
            {
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                foreach (var propertyName in propertyNames)
                {
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage).Distinct().ToList();
                    OnErrorsChanged(propertyName);
                    
                }             
            }
            OnPropertyChanged(nameof(IsValid));
        }

       

        #region Oude Validate methode

        //private void ValidateProperty(string propertyName, object newValue)
        //{
        //    var results = new List<ValidationResult>();
        //    var context = new ValidationContext(this) { MemberName = propertyName };
        //    Validator.TryValidateProperty(newValue, context, results);
        //    if (results.Any())
        //    {
        //        Errors[propertyName] = results.Select(r => r.ErrorMessage).Distinct().ToList();
        //        OnErrorsChanged(propertyName);
        //        OnPropertyChanged(nameof(IsValid));
        //    }
        //    else
        //    {
        //        if (Errors.ContainsKey(propertyName))
        //        {
        //            Errors.Remove(propertyName);
        //            OnErrorsChanged(propertyName);
        //            OnPropertyChanged(nameof(IsValid));
        //        }
        //    }
        //}

        #endregion


        private void UpdateOriginalValue(object currentValue, object newValue,string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName,currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(_originalValues[propertyName],newValue))
                {
                    _originalValues.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        protected TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue) propertyInfo.GetValue(Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            return  (_originalValues.ContainsKey(propertyName)
                ? (TValue)_originalValues[propertyName]
                : GetValue<TValue>(propertyName));
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection,
            List<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (sender, args) =>
            {
                modelCollection.Clear();
                modelCollection.AddRange(wrapperCollection.Select(w => w.Model));
                Validate();

                //Problem with Clear method
                //if (args.OldItems != null)
                //{
                //    foreach (var item in args.OldItems.Cast<TWrapper>())
                //    {
                //        modelCollection.Remove(item.Model);
                //    }
                //}

                //if (args.NewItems != null)
                //{
                //    foreach (var item in args.NewItems.Cast<TWrapper>())
                //    {
                //        modelCollection.Add(item.Model);
                //    }
                //}
            };
           RegisterTrackingObject(wrapperCollection);
        }


        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject(IValidatableTrackingObject trackingObject) 
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(IsChanged))
                    {
                        OnPropertyChanged(nameof(IsChanged));
                    }

                    if (args.PropertyName == nameof(IsValid))
                    {
                        OnPropertyChanged(nameof(IsValid));
                    }
                };
            }
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
