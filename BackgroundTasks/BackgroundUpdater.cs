using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
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

            ForecastData data = new ForecastData();
            await data.GetData(1);

            LiveTile tile = new LiveTile();
            await tile.PinTile(data);

            Notification toast = new Notification(data);

            
            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static async void RunFromMainPage()
        {
            ForecastData data = new ForecastData();
            await data.GetData(1);

            LiveTile tile = new LiveTile();
            await tile.PinTile(data);
            
        }
    }
}