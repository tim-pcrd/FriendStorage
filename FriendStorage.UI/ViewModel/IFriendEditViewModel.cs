using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;

namespace FriendStorage.UI.ViewModel
{
    public interface IFriendEditViewModel
    {
        void Load(int? friendId = null);
        Friend Friend { get; }
    }
}
