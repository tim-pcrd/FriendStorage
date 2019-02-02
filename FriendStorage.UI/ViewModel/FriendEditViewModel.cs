using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;

namespace FriendStorage.UI.ViewModel
{
    class FriendEditViewModel : IFriendEditViewModel
    {
        public void Load(int? friendId = null)
        {
            throw new NotImplementedException();
        }

        public Friend Friend { get; }
    }
}
