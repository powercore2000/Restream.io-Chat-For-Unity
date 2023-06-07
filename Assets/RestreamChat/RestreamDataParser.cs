using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RestreamChat
{
    public static class RestreamDataParser
    {
        public static SaveData saveData { get; private set; }
        static string saveFileName = "RestreamConfig.json";
        static string saveFilePath { get { return Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, saveFileName)); } }

        public static void InitalizeSaveData()
        {

            saveData = LoadData();
        }
        public static void SaveAccessToken(string aToken)
        {

            saveData.accessToken = aToken;
            SaveDataToFile();
        }
        public static void SaveRefreshToken(string rToken)
        {
            saveData.refreshToken = rToken;
            SaveDataToFile();
        }

        public static void SaveNewTokens(string aToken, string rToken)
        {
            saveData.accessToken = aToken;
            saveData.refreshToken = rToken;
            SaveDataToFile();
        }

        static void SaveDataToFile()
        {

            if (!File.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }
            JObject saveDataJson = JObject.FromObject(saveData);

            using (FileStream stream = new FileStream(saveFilePath, FileMode.Create))
            {

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    Debug.Log("Saved tokens!");
                    writer.Write(saveDataJson);
                }
            }
        }

        public static SaveData LoadData()
        {
            Debug.Log("Loading data");
            SaveData returnData = null;
            JObject dataJson;
            if (File.Exists(saveFilePath))
            {
                try
                {

                    using (FileStream stream = new FileStream(saveFilePath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {

                            dataJson = JObject.Parse(reader.ReadToEnd());
                            Debug.Log("Loaded JSON Data: " + dataJson.ToString());
                        }
                    }

                    returnData = JsonUtility.FromJson<SaveData>(dataJson.ToString());

                }
                catch (System.Exception e)
                {
                    Debug.LogError($"File did not exist at {saveFilePath}");
                    return returnData;
                }
            }


            return returnData;
        }


    }

    [System.Serializable]
    public class SaveData
    {

        public string accessToken;
        public string refreshToken;
        public string clientId;
        public string clientSecret;
    }
}