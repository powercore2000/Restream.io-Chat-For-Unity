using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RestreamChat
{
    public class RestreamChatSubscriber : MonoBehaviour
    {

        // Start is called before the first frame update
        void Awake()
        {
            RestreamChatListener.OnMessageRecived += LogMessage;
            //RestreamChatListener.OnMessageRecived += SendMessageToInword;
        }

        void LogMessage(string jsonString)
        {

            JObject jsonMessage = JObject.Parse(jsonString);
            string textInfo;
            Debug.Log($"Json recived {jsonMessage}");
            if (jsonMessage["action"].ToString() == "event")
            {

                textInfo = jsonMessage["payload"]["eventPayload"]["text"].ToString();
                Debug.Log("Message: " + textInfo);

            }

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}