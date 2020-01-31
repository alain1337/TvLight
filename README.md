# TvLight
TvLight allows to monitor the IPs of TVs and turn on or off Hue lights when the TV is on. TvLight is intended to run all the time, ideally on a low-power device such as a Rasberry Pi.

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
      "type": "Tv"
    },
    {
      "name": "Wohnzimmer",
      "mac": "00-05-CD-25-00-81",
      "type": "Tv"
    }
  ] 
}
```
While TvLight is limited to a single subnet, it can easily and automatically handle IP changes.
