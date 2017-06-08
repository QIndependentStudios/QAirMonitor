using Microsoft.EntityFrameworkCore;
using QAirMonitor.Persist.Context;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using static Template10.Common.BootStrapper;

namespace QAirMonitor.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : BootStrapper
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        //public override async Task OnInitializeAsync(IActivatedEventArgs args)
        //{
        //    if (Window.Current.Content as ModalDialog == null)
        //    {
        //        // create a new frame 
        //        var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

        //        // create modal root
        //        //Window.Current.Content = new ModalDialog
        //        //{
        //        //    DisableBackButtonWhenModal = true,
        //        //    Content = new Frame,
        //        //    //ModalContent = new Views.Busy()
        //        //};
        //    }
        //    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(320, 480));
        //    await Task.CompletedTask;
        //}

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            using (var db = new AppDataContext())
            {
                db.Database.Migrate();
            }

            NavigationService.Navigate(typeof(MainPage));
            await Task.CompletedTask;
        }
    }
}
