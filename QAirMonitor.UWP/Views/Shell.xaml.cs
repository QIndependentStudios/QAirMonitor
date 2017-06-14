using QKit.Controls;
using System;
using System.Linq;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QAirMonitor.UWP.Views
{
    public sealed partial class Shell : Page
    {

        public static Shell Instance { get; set; }
        public static NavigationMenuView NavigationMenu => Instance.NavigationMenuRoot;

        public INavigationService NavigationService { get; private set; }

        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            RemoveNavigationService();
            NavigationService = navigationService;
            NavigationService.AfterRestoreSavedNavigation += NavigationService_AfterRestoreSavedNavigation;
            NavigationService.FrameFacade.Navigated += FrameFacade_Navigated;
            NavigationMenuRoot.NavigationFrame = NavigationService.Frame;
        }

        public void RemoveNavigationService()
        {
            if (NavigationService == null)
                return;

            NavigationService.AfterRestoreSavedNavigation -= NavigationService_AfterRestoreSavedNavigation;
            NavigationService.FrameFacade.Navigated -= FrameFacade_Navigated;
            NavigationMenuRoot.NavigationFrame = null;
            NavigationService = null;
        }

        internal void HighlightCurrentMenuItem(Type pageType, object pageParam)
        {
            // match type only
            var menuItems = NavigationMenuRoot.PrimaryMenuItems
                .Union(NavigationMenuRoot.SecondaryMenuItems)
                .Where(x => Equals(x.PageType, pageType));

            // serialize parameter for matching
            if (pageParam == null)
            {
                pageParam = NavigationService.CurrentPageParam;
            }
            else if (pageParam.ToString().StartsWith("{"))
            {
                try
                {
                    pageParam = ((NavigationService)NavigationService).SerializationService.Deserialize(pageParam.ToString());
                }
                catch { }
            }

            // add parameter match
            menuItems = menuItems.Where(x => Equals(x.PageParameters, null) || Equals(x.PageParameters, pageParam));
            var menuItem = menuItems.Select(x => x).FirstOrDefault();
            if (menuItem != null)
                menuItem.IsChecked = true;
        }

        private void NavigationService_AfterRestoreSavedNavigation(object sender, Type e)
        {
            // _navigationService.CurrentPageType and CurrentPageParam is broken and only returns null. Workaround below.
            var savedNavigationServiceState = NavigationService.FrameFacade.PageStateSettingsService(NavigationService.GetType().ToString());
            var currentPageType = Type.GetType(savedNavigationServiceState.Read<string>("CurrentPageType"));
            var currentPageParam = savedNavigationServiceState.Read<object>("CurrentPageParam");
            Template10.Common.BootStrapper.Current.UpdateShellBackButton();

            HighlightCurrentMenuItem(currentPageType, currentPageParam);
        }

        private void FrameFacade_Navigated(object sender, NavigatedEventArgs e)
        {
            HighlightCurrentMenuItem(e.PageType, e.Parameter);
        }

        private void NavigationMenuRoot_SelectedMenuItemChanged(object sender, RoutedEventArgs e)
        {
            if (NavigationMenuRoot.SelectedMenuItem == null ||
                NavigationMenuRoot.SelectedMenuItem.PageType == null)
                return;

            NavigationService.Navigate(NavigationMenuRoot.SelectedMenuItem.PageType,
              NavigationMenuRoot.SelectedMenuItem.PageParameters);

            if (NavigationMenuRoot.SelectedMenuItem.ClearHistory)
                NavigationService.ClearHistory();
        }
    }
}