using BackgroundTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace FiveThirtyEight
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RegisterBackgroundTask();

            SetUp();

        }

        async void SetUp()
        {
            //SET LIVETILE ON STARTUP
            BackgroundUpdater.RunFromMainPage(settingPhoto.IsOn);

            await Task.Delay(2000);

            loadRing.Visibility = Visibility.Collapsed;
            loadText.Visibility = Visibility.Collapsed;
            settingsStack.Visibility = Visibility.Visible;
        }



        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }

            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = taskEntryPoint;
            taskBuilder.SetTrigger(new TimeTrigger(15, false));


            var registration = taskBuilder.Register();

        }

        private const string taskName = "BackgroundUpdater";
        private const string taskEntryPoint = "BackgroundTasks.BackgroundUpdater";

        private void settingToggled(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            appSettings.Values["enableNotifications"] = settingNotifications.IsOn;
            appSettings.Values["enablePhoto"] = settingPhoto.IsOn;

            BackgroundUpdater.RunFromMainPage(settingPhoto.IsOn);
        }
    }

}
