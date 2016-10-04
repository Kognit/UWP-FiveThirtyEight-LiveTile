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
using Windows.UI;

namespace BackgroundTasks
{
    public sealed class LiveTileScreenshot
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //THIS CLASS IS NOT BEING USED
        //ITS ONLY PURPOSE WAS TO SETUP A PRETTY SCREENSHOT BY PINNING A BUNCH OF SECONDARY TILES
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        static string logo = "Assets/clinton-alligator.png";
        ForecastData data;
        string democrat = "Hillary Clinton";
        string republican = "Donald Trump";

        string diffDemocrat = "";
        string diffRepublican = "";

        public LiveTileScreenshot()
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

            //EXTRA
            for (int s = 0; s < 4; s++)
            {
                SecondaryTile tile = new SecondaryTile(DateTime.Now.Ticks.ToString())
                {
                    DisplayName = "FiveThirtyEight",
                    Arguments = "args"
                };
                tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
                tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                tile.VisualElements.ShowNameOnSquare150x150Logo = false;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = false;

                switch (s)
                {
                    case 0:
                        tile.VisualElements.BackgroundColor = Colors.Teal;
                        break;
                    case 1:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 209, 52, 56);
                        break;
                    case 2:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 0, 120, 215);
                        break;
                    case 3:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 135, 100, 184);
                        break;
                    case 4:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 16, 137, 62);
                        break;
                }

                if (!await tile.RequestCreateAsync())
                {
                    return;
                }
                //EXTRA
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Clear();
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).EnableNotificationQueue(true);
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
                        //TileUpdateManager.CreateTileUpdaterForApplication().Update(t);
                        TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(t);
                        mode = 1;
                    }


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



        internal TileBinding GenerateTileBindingWide(int mode, string text1, string text2, AdaptiveTextStyle hintstyle, string image)
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
