﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendStorage.UI.Wrapper
{
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IValidatableTrackingObject
        where T : class, IValidatableTrackingObject
    {
        private IList<T> _originalCollection;

        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _removedItems;
        private readonly ObservableCollection<T> _modifiedItems;

        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {
            _originalCollection = this.ToList();

            AttachItemPropertyChangedHandler(_originalCollection);

            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);
        }

      

        public ReadOnlyObservableCollection<T> AddedItems { get; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; }


        public void AcceptChanges()
        {
            _addedItems.Clear();
            _modifiedItems.Clear();
            _removedItems.Clear();
            foreach (var item in this)
            {
                item.AcceptChanges();
            }

            _originalCollection = this.ToList();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        public void RejectChanges()
        {
            foreach (var addedItem in _addedItems.ToList())
            {
                Remove(addedItem);
            }

            foreach (var removedItem in _removedItems.ToList())
            {
                removedItem.RejectChanges(); //in case it has been first modified and then removed
                Add(removedItem);
            }

            foreach (var modifiedItem in _modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        public bool IsChanged
        {
            get { return AddedItems.Count > 0 || RemovedItems.Count > 0 || ModifiedItems.Count > 0; }
        }

        public bool IsValid
        {
            get { return this.All(t => t.IsValid); }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var added = this.Where(current => _originalCollection.All(orig => orig != current));
            var removed = _originalCollection.Where(orig => this.All(current => current != orig));

            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged);

            AttachItemPropertyChangedHandler(added);
            DetachItemPropertyChangedHandler(removed);

            UpdateObservableCollection(_addedItems, added);
            UpdateObservableCollection(_removedItems, removed);
            UpdateObservableCollection(_modifiedItems, modified);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
        }

        private void UpdateObservableCollection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        private void DetachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }


        private void AttachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
            }
            else
            {
                var item = (T)sender;

                if (_addedItems.Contains(item))
                {
                    return;
                }

                if (item.IsChanged)
                {
                    if (!_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Add(item);
                    }
                }
                else
                {
                    if (_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Remove(item);
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
         
        }

        
    }
}
