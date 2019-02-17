﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;
using FriendStorage.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FriendStorage.UITests.Wrapper
{
    [TestClass]
    public class ChangeTrackingCollectionTests
    {
        private List<FriendEmailWrapper> _emails;

        [TestInitialize]
        public void Initialize()
        {
            _emails = new List<FriendEmailWrapper>()
            {
                new FriendEmailWrapper(new FriendEmail(){Email = "thomas@thomasclaudiushuber.com"}),
                new FriendEmailWrapper(new FriendEmail(){Email = "julia@juhu-design.com"})
            };
        }

        [TestMethod]
        public void ShouldTrackAddedItems()
        {
            var emailToAdd = new FriendEmailWrapper(new FriendEmail());

            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);
            Assert.AreEqual(2,c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Add(emailToAdd);
            Assert.AreEqual(3,c.Count);
            Assert.AreEqual(1,c.AddedItems.Count);
            Assert.AreEqual(0,c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(emailToAdd,c.AddedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Remove(emailToAdd);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackRemovedItems()
        {
            var emailToRemove = _emails.First();
            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Remove(emailToRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(emailToRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Add(emailToRemove);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackModifiedItems()
        {
            var emailToModify = _emails.First();
            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            emailToModify.Email = "modified@thomasclaudiushuber.com";
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.IsTrue(c.IsChanged);

            emailToModify.Email = "thomas@thomasclaudiushuber.com";
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldNotTrackAddedItemsAsModified()
        {
            var emailToAdd = new FriendEmailWrapper(new FriendEmail());

            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);
            c.Add(emailToAdd);
            emailToAdd.Email = "modified@thomasclaudiushuber.com";
            Assert.IsTrue(emailToAdd.IsChanged);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var emailToModify = _emails.First();
            var emailToRemove = _emails.Skip(1).First();
            var emailToAdd = new FriendEmailWrapper(new FriendEmail { Email = "anotherOne@thomasclaudiushuber.com" });

            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);

            c.Add(emailToAdd);
            c.Remove(emailToRemove);
            emailToModify.Email = "modified@thomasclaudiushuber.com";
            Assert.AreEqual("thomas@thomasclaudiushuber.com", emailToModify.EmailOriginalValue);

            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);

            c.AcceptChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(emailToModify));
            Assert.IsTrue(c.Contains(emailToAdd));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);

            Assert.IsFalse(emailToModify.IsChanged);
            Assert.AreEqual("modified@thomasclaudiushuber.com", emailToModify.Email);
            Assert.AreEqual("modified@thomasclaudiushuber.com", emailToModify.EmailOriginalValue);

            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var emailToModify = _emails.First();
            var emailToRemove = _emails.Skip(1).First();
            var emailToAdd = new FriendEmailWrapper(new FriendEmail { Email = "anotherOne@thomasclaudiushuber.com" });

            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);

            c.Add(emailToAdd);
            c.Remove(emailToRemove);
            emailToModify.Email = "modified@thomasclaudiushuber.com";
            Assert.AreEqual("thomas@thomasclaudiushuber.com", emailToModify.EmailOriginalValue);

            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);

            c.RejectChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(emailToModify));
            Assert.IsTrue(c.Contains(emailToRemove));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);

            Assert.IsFalse(emailToModify.IsChanged);
            Assert.AreEqual("thomas@thomasclaudiushuber.com", emailToModify.Email);
            Assert.AreEqual("thomas@thomasclaudiushuber.com", emailToModify.EmailOriginalValue);

            Assert.IsFalse(c.IsChanged);
        }


        [TestMethod]
        public void ShouldRejectChangesWithModifiedAndRemovedItem()
        {
            var email = _emails.First();

            var c = new ChangeTrackingCollection<FriendEmailWrapper>(_emails);

            email.Email = "modified@thomasclaudiushuber.com";
            c.Remove(email);
            Assert.AreEqual("thomas@thomasclaudiushuber.com", email.EmailOriginalValue);

            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);

            c.RejectChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(email));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);

            Assert.IsFalse(email.IsChanged);
            Assert.AreEqual("thomas@thomasclaudiushuber.com", email.Email);
            Assert.AreEqual("thomas@thomasclaudiushuber.com", email.EmailOriginalValue);

            Assert.IsFalse(c.IsChanged);
        }


    }
}
