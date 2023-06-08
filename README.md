# Restream.io-Chat-For-Unity

A basic WebSocket system that provides access to your chat from Restream.io.

## Requirments
[netwonsoft for unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM)

## Features
- Auto Token Refresh: You can set the delay for token refresh in the `RestreamChatListener.cs` script.
- WebSocket Chat Listener: It reads all JSON event data from Restream and allows you to parse chat messages or other information.

## Credentials
Your credentials are stored in the `streamingAssets/RestreamConfig.json` file. Please fill in all the fields there for full functionality during play.

Example configuration:
```json
{
  "accessToken": "YOUR_ACCESS_TOKEN",
  "refreshToken": "YOUR_REFRESH_TOKEN",
  "clientId": "YOUR_CLIENT_ID",
  "clientSecret": "YOUR_CLIENT_SECRET"
}
```
## Resources
For information on how to get your credentials from the restream API head to their [developer portal](https://developers.restream.io/) and read their documentation. 

Note: Something I struggled with was the Capture the code stage. A quick fix i did was set the redirect URI to `http://localhost/`  
