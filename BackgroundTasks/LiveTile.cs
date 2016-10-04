using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using NotificationsExtensions.Tiles;
using Microsoft.VisualBasic;
using NotificationsExtensions;
using Windows.Storage;
using Windows.ApplicationModel.Background;

namespace BackgroundTasks
{
    public sealed class LiveTile
    {

        static string logo = "Assets/clinton-alligator.png";
        ForecastData data;
        string democrat = "Hillary Clinton";
        string republican = "Donald Trump";

        string diffDemocrat = "";
        string diffRepublican = "";

        public LiveTile()
        {


        }

        internal async Task PinTile(ForecastData fd)
        {
            data = fd;

            if (data.confidenceDifference > 0.01)
            {
                diffDemocrat = " (+" + data.confidenceDifference.ToString("F1") + "%)";
                diffRepublican = " (" + (-data.confidenceDifference).ToString("F1") + "%)";
            }
            else if (data.confidenceDifference < -0.01)
            {
                diffDemocrat = " (" + data.confidenceDifference.ToString("F1") + "%)";
                diffRepublican = " (+" + (-data.confidenceDifference).ToString("F1") + "%)";
            }

            /*if (debug != "")
            {
                democrat = debug;
                republican = debug;
            }*/

            

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            int mode = 1;
            for (int i = 0; i < 2; i++)
            {
                string text1 = "";
                string text2 = "";
                string image = "";
                AdaptiveTextStyle hintstyle = AdaptiveTextStyle.Subheader;
                bool doadd = true;

                switch (i)
                {
                    case 0:
                        text1 = data.confidenceDemocrat + "%" + diffDemocrat;
                        text2 = democrat;
                        image = "Assets/clinton-head-line-alpha.png";
                        break;
                    case 1:
                        text1 = data.confidenceRepublican + "%" + diffRepublican;
                        text2 = republican;
                        image = "Assets/trump-head-line-alpha.png";
                        break;

                }

                if (doadd)
                {
                    TileContent content = GenerateTileContent(mode, text1, text2, hintstyle, image);

                    TileNotification t = new TileNotification(content.GetXml());
                    t.Tag = i.ToString();
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(t);
                    mode = 1;
                }


            }
        }

        private TileContent GenerateTileContent(int mode, string text1, string text2, AdaptiveTextStyle hintstyle, string image)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = GenerateTileBindingMedium(mode, text1, text2, hintstyle, image),
                    TileWide = GenerateTileBindingWide(mode, text1, text2, hintstyle, image),
                    TileLarge = GenerateTileBindingLarge(mode, text1, text2, hintstyle, image)
                }
            };
        }

        private static TileBinding GenerateTileBindingMedium(int mode, string text1, string text2, AdaptiveTextStyle hintstyle, string image)
        {



            TileBinding value = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {

                    TextStacking = TileTextStacking.Center,

                    Children =
            {
                new AdaptiveImage()
                {
                    Source = image,
                    HintRemoveMargin = true
                    
                    
                },

                new AdaptiveText()
                {
                    Text = text1,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.Caption
                },

                new AdaptiveText()
                {
                    Text = text2,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                },
                //Text = ((int)((DateTime.Now - MainPage.settings.FirstStart).TotalDays)).ToString(),
            }

                }
            };

            if (mode % 2 == 0)
            {
                ((TileBindingContentAdaptive)value.Content).PeekImage = new TilePeekImage()
                {
                    Source = logo
                };
            }

            return value;
        }

        

        internal  TileBinding GenerateTileBindingWide(int mode, string text1, string text2, AdaptiveTextStyle hintstyle, string image)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
            {
                new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 48,


                            Children =
                            {

                                new AdaptiveImage()
                                {
                                    Source = "Assets/clinton-head-line-alpha.png",
                                    HintRemoveMargin = true,
                                },
                                new AdaptiveText()
                                {
                                    Text = data.confidenceDemocrat + "%" + diffDemocrat,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption,
                                },

                                new AdaptiveText()
                                {
                                    Text = democrat,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                
                            }
                        },

                        new AdaptiveSubgroup()
                        {
                            HintWeight = 48,


                            Children =
                            {
                                new AdaptiveImage()
                                {
                                    Source = "Assets/trump-head-line-alpha.png",
                                    HintRemoveMargin = true
                                },
                                new AdaptiveText()
                                {
                                    Text = data.confidenceRepublican + "%" + diffRepublican,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },

                                new AdaptiveText()
                                {
                                    Text = republican,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                        }
                    }
                }
            }
                }
            };
        }

        private static TileBinding GenerateTileBindingLarge(int mode, string text1, string text2, AdaptiveTextStyle hintstyle, string image)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    TextStacking = TileTextStacking.Center,

                    Children =
            {
                new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 1
                        },
                        

                        new AdaptiveSubgroup()
                        {
                            HintWeight = 2,
                            Children =
                            {
                                new AdaptiveImage()
                                {
                                    Source = logo,
                                    HintCrop = AdaptiveImageCrop.Circle
                                }
                            }
                        },

                        new AdaptiveSubgroup()
                        {
                            HintWeight = 1
                        }
                    }
                },

                new AdaptiveText()
                {
                    Text = text1,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = hintstyle
                },

                new AdaptiveText()
                {
                    Text = text2,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.SubtitleSubtle
                }
            }
                }
            };
        }



    }
}
