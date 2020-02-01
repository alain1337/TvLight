dotnet publish -c Release
scp bin\Release\netcoreapp3.1\publish\* pi@raspberrypi:/home/pi/TvLight
