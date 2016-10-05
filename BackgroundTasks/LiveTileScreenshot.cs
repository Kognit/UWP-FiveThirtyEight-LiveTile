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
            

            //EXTRA
            for (int s = 0; s < 9; s++)
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
                    case 5:
                        tile.VisualElements.BackgroundColor = Colors.Teal;
                        break;
                    case 6:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 209, 52, 56);
                        break;
                    case 7:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 0, 120, 215);
                        break;
                    case 8:
                        tile.VisualElements.BackgroundColor = Color.FromArgb(255, 135, 100, 184);
                        break;
                    case 9:
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

                int mode = 0;
                for (int i = 0; i < 2; i++)
                {
                    TileContent content = GenerateTileContent(mode,s);

                    TileNotification t = new TileNotification(content.GetXml());
                    t.Tag = i.ToString();
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(t);
                    mode++;
                }
            }
        }



        private TileContent GenerateTileContent(int mode,int s)
        {
            TileVisual visual = new TileVisual();

            //MEDIUM TILE WILL SWITCH BETWEEN CANDIDATES USING A NOTIFICATION QUEUE
            if (s % 2 == 0)
            {
                visual.TileMedium = GenerateTileBindingMediumImage(mode);
            }
            else
            {
                visual.TileMedium = GenerateTileBindingMedium(mode);
            }
            if (mode == 1)
            {
                //WIDE AND LARGE TILES DO NOT NEED A NOTIFICATION QUEUE TO DISPLAY BOTH CANDIDATES
                visual.TileWide = GenerateTileBindingWide();
                visual.TileLarge = GenerateTileBindingLarge();
                //SADLY, SMALL TILES DO NOT SUPPORT A NOTIFICATION QUEUE
                visual.TileSmall = GenerateTileBindingSmall();
            }

            return new TileContent()
            {
                Visual = visual
            };
        }

        private TileBinding GenerateTileBindingSmall()
        {
            string text1 = (int)data.confidenceDemocrat + "% (D)";
            string text2 = (int)data.confidenceRepublican + "% (R)";

            TileBinding value = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {

                    TextStacking = TileTextStacking.Center,

                    Children =
            {

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
                    HintStyle = AdaptiveTextStyle.Caption
                }
            }

                }
            };


            return value;
        }

        private TileBinding GenerateTileBindingMedium(int mode)
        {
            string text1 = "";
            string text2 = "";
            string image = "";
            string back = "";
            switch (mode)
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
                    HintStyle = AdaptiveTextStyle.Base
                },

                new AdaptiveText()
                {
                    Text = text2,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                },
            }

                }
            };


            return value;
        }

        private TileBinding GenerateTileBindingMediumImage(int mode)
        {
            string text1 = "";
            string text2 = "";
            string back = "";
            switch (mode)
            {
                case 0:
                    text1 = (int)data.confidenceDemocrat + "%" + diffDemocrat;
                    text2 = democrat;
                    back = "Assets/medium-clinton.jpg";
                    break;
                case 1:
                    text1 = (int)data.confidenceRepublican + "%" + diffRepublican;
                    text2 = republican;
                    back = "Assets/medium-trump.jpg";
                    break;
            }


            TileBinding value = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {

                    TextStacking = TileTextStacking.Center,
                    BackgroundImage = new TileBackgroundImage() { Source = back },

                    Children =
            {

                new AdaptiveText()
                {
                    Text = "",
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.Caption
                },
                new AdaptiveText()
                {
                    Text = text1,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.HeaderNumeral
                },
                new AdaptiveText()
                {
                    Text = text2,
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.Caption
                }
            }

                }
            };


            return value;
        }



        private TileBinding GenerateTileBindingWide()
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

        private TileBinding GenerateTileBindingLarge()
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
                                    HintRemoveMargin = false,
                                },
                                new AdaptiveText()
                                {
                                    Text = data.confidenceDemocrat + "%" + diffDemocrat,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.TitleNumeral,
                                },

                                new AdaptiveText()
                                {
                                    Text = democrat,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },

                                new AdaptiveText()
                                {
                                    Text = "",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = data.electoralVotesDemocrat.ToString("F1"),
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Body,
                                },

                                new AdaptiveText()
                                {
                                    Text = "Electoral Votes",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = data.popularVoteDemocrat + "%",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Body,
                                },

                                new AdaptiveText()
                                {
                                    Text = "Popular Vote",
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
                                    HintRemoveMargin = false
                                },
                                new AdaptiveText()
                                {
                                    Text = data.confidenceRepublican + "%" + diffRepublican,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.TitleNumeral
                                },

                                new AdaptiveText()
                                {
                                    Text = republican,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },

                                new AdaptiveText()
                                {
                                    Text = "",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = data.electoralVotesRepublican.ToString("F1"),
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Body,
                                },

                                new AdaptiveText()
                                {
                                    Text = "Electoral Votes",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = data.popularVoteRepublican + "%",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Body,
                                },

                                new AdaptiveText()
                                {
                                    Text = "Popular Vote",
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



    }
}
