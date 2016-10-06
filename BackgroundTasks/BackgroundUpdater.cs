using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace BackgroundTasks
{
    public sealed class BackgroundUpdater : IBackgroundTask
    {

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();


            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;

            ForecastData data = new ForecastData();
            await data.GetData(1);

            if (appSettings.Values.ContainsKey("enablePhoto") && (bool)appSettings.Values["enablePhoto"])
            {
                LiveTile tile = new LiveTile(true);
                await tile.PinTile(data);               
            }
            else
            {
                LiveTile tile = new LiveTile(false);
                await tile.PinTile(data);
            }
            
            
            if (appSettings.Values.ContainsKey("enableNotifications"))
            {
                if ((bool)appSettings.Values["enableNotifications"])
                {
                    Notification toast = new Notification(data);
                }
            }

            
            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static async void RunFromMainPage(bool doPhoto)
        {
            ForecastData data = new ForecastData();
            await data.GetData(1);

            LiveTileScreenshot tile = new LiveTileScreenshot();
            await tile.PinTile(data);
            
        }
    }
}