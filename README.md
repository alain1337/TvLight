![.NET Core](https://github.com/alain1337/TvLight/workflows/.NET%20Core/badge.svg)

# TvLight
TvLight allows to monitor the IPs of TVs and turn on or off Hue lights when the TV is on. TvLight is intended to run all the time, ideally on a low-power device such as a Raspberry Pi.

While TvLight is limited to a single subnet, it can easily and automatically handle IP changes.

## Requirements

* Philips Hue Bridge
* Network connected TV
* Any kind of Hue lamp
* [.NET Core 3.1 Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Setting it up

Devices are configurable and are identified by their MAC address:

```javascript
{
  "devices": [
    {
      "name": "Hue Bridge",
      "mac": "EC-B5-FA-18-6B-9D",
      "type": "HueBridge"
    },
    {
      "name": "Schlafzimmer",
      "mac": "20-3D-BD-F0-DC-60",
      "type": "Tv",
      "controls": []
    },
    {
      "name": "Wohnzimmer",
      "mac": "00-05-CD-25-00-81",
      "type": "Tv",
      "controls": []
    }
  ] 
}
```

You need to [setup a user name](https://developers.meethue.com/develop/get-started-2/) in your Hue Bridge and put the username into credentials.json in the same directory as the executable:

```javascript
{
  "username": "insert-username-here"
}
```

Now you can run it with `dotnet run` and check if the basics work. This also outputs your groups and lights and you can add this to controls in your TV device like this:

```javascript
{
  "devices": [
    {
      "name": "Hue Bridge",
      "mac": "EC-B5-FA-18-6B-9D",
      "type": "HueBridge"
    },
    {
      "name": "Schlafzimmer",
      "mac": "20-3D-BD-F0-DC-60",
      "type": "Tv",
      "controls": [ "TV Schlafzimmer Links", "TV Schlafzimmer Rechts" ]
    },
    {
      "name": "Wohnzimmer",
      "mac": "00-05-CD-25-00-81",
      "type": "Tv",
      "controls": [ "TV Wohnzimmer" ]
    }
  ] 
}
```
