dotnet publish -c Release
scp bin\Release\netcoreapp3.1\publish\* pi@192.168.1.123:/home/pi/TvLight
