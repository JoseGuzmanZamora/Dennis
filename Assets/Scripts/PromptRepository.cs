using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class PromptRepository
    {
        private const string ApiUrl = "https://dennis-backend.fly.dev";
        public TextAsset envFile;

        public PromptRepository(TextAsset envFile)
        {
            this.envFile = envFile;
        }

        public IEnumerator GetPromptResponse(int nodeId, string prompt, int optionHelper, Action<(string, int, int)> callback)
        {
            var uwr = new UnityWebRequest($"{ApiUrl}/completion", "POST");
            var request = new PromptRequest
            {
                pass = GetLocalPassport(),
                prompt = $"{prompt} Make it 5 sentences long and under 100 words."
            };
            var jsonString = JsonConvert.SerializeObject(request);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                // Deserialize into result
                var resultObject = JsonConvert.DeserializeObject<PromptResult>(uwr.downloadHandler.text);
                callback((resultObject.result, nodeId, optionHelper));
            }
        }
        
        public string GetLocalPassport()
        {
            return envFile.text;
        }

        public IEnumerator DownloadImage(string imageUrl, int optionHelper, Action<(Texture2D, int)> callback)
        {   
            if (imageUrl != null && imageUrl != "")
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return request.SendWebRequest();
                if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);   
                }
                else
                {
                    var resultTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                    callback((resultTexture, optionHelper));
                }
            }
        } 
    }
}