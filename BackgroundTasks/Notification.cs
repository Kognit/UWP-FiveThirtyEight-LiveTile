using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    class Notification
    {
        ForecastData data;

        public Notification(ForecastData fd)
        {
            data = fd;
            PopToast();
        }

        private void PopToast()
        {
            ToastContent content = GenerateToastContent();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        private ToastContent GenerateToastContent()
        {
            string stringDemocrat = data.confidenceDemocrat + "% Hillary Clinton";
            string stringRepublican = data.confidenceRepublican + "% Donald Trump";

            if(data.confidenceDifference > 0.01)
            {
                stringDemocrat = stringDemocrat + " (+" + data.confidenceDifference.ToString("F1") + "%)";
                stringRepublican = stringRepublican + " (" + (-data.confidenceDifference).ToString("F1") + "%)";
            }
            else if (data.confidenceDifference < -0.01)
            {
                stringDemocrat = stringDemocrat + " (" + data.confidenceDifference.ToString("F1") + "%)";
                stringRepublican = stringRepublican + " (+" + (-data.confidenceDifference).ToString("F1") + "%)";
            }

            return new ToastContent()
            {
                Launch = "action=viewEvent&eventId=1983",
                Scenario = ToastScenario.Reminder,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "FiveThirtyEight Forecast Updated!"
                    },

                    new AdaptiveText()
                    {
                        Text = stringDemocrat
                    },

                    new AdaptiveText()
                    {
                        Text = stringRepublican
                    }
                }
                    }
                }

                
            };
        }


    }
}
