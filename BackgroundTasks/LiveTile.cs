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
        string democratShort = "CLI";
        string republicanShort = "TRU";

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
            
            

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            int mode = 0;
            for (int i = 0; i < 2; i++)
            {
                TileContent content = GenerateTileContent(mode);

                TileNotification t = new TileNotification(content.GetXml());
                t.Tag = i.ToString();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(t);
                mode++;
            }
        }

        private TileContent GenerateTileContent(int mode)
        {
            TileVisual visual = new TileVisual();

            //MEDIUM TILE WILL SWITCH BETWEEN CANDIDATES USING A NOTIFICATION QUEUE
            visual.TileMedium = GenerateTileBindingMediumImage(mode);
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
                    text1 = (int)data.confidenceDemocrat + "%";
                    text2 = democrat;
                    back = "Assets/medium-clinton.jpg";
                    break;
                case 1:
                    text1 = (int)data.confidenceRepublican + "%";
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
            string dem1 = democrat;
            string dem2 = "";
            string rep1 = republican;
            string rep2 = "";

            if(diffDemocrat != "")
            {
                dem1 = diffDemocrat.Substring(1);
                dem2 = democrat;
                rep1 = diffRepublican.Substring(1);
                rep2 = republican;
            }

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
                                    Text = data.confidenceDemocrat + "%",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.TitleNumeral,
                                },
                                new AdaptiveText()
                                {
                                    Text = dem1,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = dem2,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
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
                                    Text = data.confidenceRepublican + "%",
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.TitleNumeral
                                },
                                new AdaptiveText()
                                {
                                    Text = rep1,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = rep2,
                                    HintAlign = AdaptiveTextAlign.Center,
                                    HintStyle = AdaptiveTextStyle.Caption
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
