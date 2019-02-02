using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendStorage.UI.DataProvider.Lookups
{
    public interface ILookupProvider<T>
    {
        IEnumerable<LookupItem> GetLookups();
    }
}
