using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FriendStorage.DataAccess;
using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.DataProvider.Lookups;
using FriendStorage.UI.View;
using FriendStorage.UI.ViewModel;
using Prism.Events;

namespace FriendStorage.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<FileDataService>().As<IDataService>();
            builder.RegisterType<FriendLookupProvider>().As<ILookupProvider<Friend>>();
            builder.RegisterType<FriendGroupLookupProvider>().As<ILookupProvider<FriendGroup>>();
            builder.RegisterType<FriendDataProvider>().As<IFriendDataProvider>();

            builder.RegisterType<FriendEditViewModel>().As<IFriendEditViewModel>();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();

            return builder.Build();
        }
    }
}
