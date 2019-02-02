using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.DataAccess;
using FriendStorage.Model;

namespace FriendStorage.UI.DataProvider.Lookups
{
    public class FriendLookupProvider : ILookupProvider<Friend>
    {
        private readonly Func<IDataService> _dataServiceCreator;

        public FriendLookupProvider(Func<IDataService> dataServiceCreator)
        {
            _dataServiceCreator = dataServiceCreator;
        }

        public IEnumerable<LookupItem> GetLookups()
        {
            using (var service = _dataServiceCreator())
            {
                return service.GetAllFriends()
                    .Select(f => new LookupItem()
                    {
                        Id = f.Id,
                        DisplayValue = $"{f.FirstName} {f.LastName}"
                    })
                    .OrderBy(l => l.DisplayValue).ToList();
            }
        }
    }
}
