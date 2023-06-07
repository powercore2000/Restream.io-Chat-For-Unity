using System;
using System.Collections;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RestreamChat
{
    public class RestreamChatListener : MonoBehaviour
    {
        private static string AccessTokenURL = "https://api.restream.io/oauth/token";
        private static string refreshToken;
        public static Action<string> OnMessageRecived;
        public static RestreamChatListener inst { get; private set; }


        int tokenRefreshInterval = 3000;
        private void Awake()
        {
            if (inst == null)
                inst = this;
            RestreamDataParser.InitalizeSaveData();
            StartCoroutine(StartWebsocket());
        }

        IEnumerator StartWebsocket()
        {

            while (true)
            {
                yield return inst.RegenerateTokens();
                yield return ActivateWebsocket();
                yield return new WaitForSeconds(tokenRefreshInterval);
            }
        }

        public IEnumerator RegenerateTokens()
        {

            UnityEngine.Debug.Log("Running temp rejen!");
            if (RestreamDataParser.saveData != null)
            {
                refreshToken = RestreamDataParser.saveData.refreshToken;
            }
            else
                refreshToken = "1";

            WWWForm formData = new WWWForm();

            formData.AddField("grant_type", "refresh_token");
            formData.AddField("refresh_token", refreshToken);


            using (UnityWebRequest webRequest = UnityWebRequest.Post(AccessTokenURL, formData))
            {
                string auth = $"{RestreamDataParser.saveData.clientId}:{RestreamDataParser.saveData.clientSecret}";
                string authHeader = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))}";
                webRequest.SetRequestHeader("Authorization", authHeader);
                webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                yield return webRequest.SendWebRequest();
                UnityEngine.Debug.Log(webRequest.result.ToString());

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string response = webRequest.downloadHandler.text;
                    // Handle the access token response
                    UnityEngine.Debug.Log(response);
                    JObject responseJson = JObject.Parse(response);
                    RestreamDataParser.SaveNewTokens(responseJson["accessToken"].ToString(), responseJson["refreshToken"].ToString());

                }
                else
                {
                    string error = webRequest.error;
                    JObject errorJson = JObject.Parse(webRequest.downloadHandler.text);
                    // Handle the error
                    UnityEngine.Debug.LogError(error);
                    UnityEngine.Debug.LogError(errorJson.ToString());
                    //UnityEngine.Debug.Log("Current save data " + RestreamDataParser.saveData.ToString());
                }




            }



        }

        /// <summary>
        /// Made to test if normal get responses are working
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetChatUrl()
        {

            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://api.restream.io/v2/user/webchat/url"))
            {
                string auth = $"{RestreamDataParser.saveData.clientId}:{RestreamDataParser.saveData.clientSecret}";
                string authHeader = $"Bearer ae430155b6f7741d1f0d11597e0adc639b028695";
                webRequest.SetRequestHeader("Authorization", authHeader);
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();
                UnityEngine.Debug.Log("Request finished!");
                if (!webRequest.isNetworkError)
                {
                    string response = webRequest.downloadHandler.text;
                    // Handle the access token response
                    UnityEngine.Debug.Log(response);
                }
                else
                {
                    string error = webRequest.error;
                    // Handle the error
                    UnityEngine.Debug.LogError(error);
                }



            }
        }

        public static async Task ActivateWebsocket()
        {
            string accessToken = RestreamDataParser.saveData.accessToken;
            string url = $"wss://chat.api.restream.io/ws?accessToken={accessToken}";
            UnityEngine.Debug.Log("Starting socket...");
            using (ClientWebSocket connection = new ClientWebSocket())
            {
                await connection.ConnectAsync(new Uri(url), CancellationToken.None);
                UnityEngine.Debug.Log("Connection attempted...");

                while (connection.State == WebSocketState.Open)
                {
                    UnityEngine.Debug.Log("Open Socket!");
                    WebSocketReceiveResult result;
                    byte[] buffer = new byte[4096];
                    StringBuilder message = new StringBuilder();

                    do
                    {
                        result = await connection.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    OnMessageRecived.Invoke(message.ToString());

                }

                if (connection.State != WebSocketState.Closed)
                {
                    await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    UnityEngine.Debug.Log("Closed Socket!");
                }
            }
        }


    }

    public interface IUpdates
    {
        // Define the properties and methods of the IUpdates interface here
        // ...
    }
}