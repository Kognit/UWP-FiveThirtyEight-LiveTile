# UWP-FiveThirtyEight-LiveTile
UWP App implementing Live-Tile functionality to display presidential race data from fivethirtyeight.com

![FiveThirtyEight Tile](previewmulti.png)

Starting this app once will register a Background-Process polling fivethirtyeight.com every 15 minutes to get the latest confidence scores of Nate Silvers model.

# Installation
If you don't want to build the project from source, i have included a prebuilt package you can use to sideload the app. Heres how to do it, taken from [Microsoft](https://msdn.microsoft.com/en-us/windows/uwp/packaging/packaging-uwp-apps#sideload-your-app-package):

- [make sure you have sideloading enabled on your system](https://msdn.microsoft.com/windows/uwp/get-started/enable-your-device-for-development#enable-your-windows-10-devices)
- download the repository (green button top right)
- unzip
- open the "FiveThirtyEight_1.0.0.0" folder
- right-click "Add-AppDevPackage.ps1"
- select "Run with Powershell"

Further on-screen instructions will then guide you through a few Enter-presses and you're done.

# Live-Tile

All Live-Tile sizes (small, medium, wide, large) are supported and will show the overall confidence scores for the candidates.

When the numbers change, a trend will also be displayed.

You can choose between fivethirtyeights cartoon heads and actual photos of the candidates to be displayed on the medium sized tile.
The large tile will also display the predicted Electoral Votes and Popular Vote.

# Notifications

You can choose to receive a notification when the numbers change.
