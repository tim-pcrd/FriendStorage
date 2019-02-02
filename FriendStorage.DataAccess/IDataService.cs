using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;

namespace FriendStorage.DataAccess
{
    public interface IDataService : IDisposable
    {
        Friend GetFriendById(int friendId);
        void SaveFriend(Friend friend);
        void DeleteFriend(int friendId);
        IEnumerable<Friend> GetAllFriends();
        IEnumerable<FriendGroup> GetAllFriendGroups();
    }
}
