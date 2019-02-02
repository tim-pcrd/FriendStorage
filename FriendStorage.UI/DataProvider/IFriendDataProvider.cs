using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;

namespace FriendStorage.UI.DataProvider
{
    public interface IFriendDataProvider
    {
        Friend GetFriendById(int id);
        void SaveFriend(Friend friend);
        void DeleteFriend(int id);
    }
}
