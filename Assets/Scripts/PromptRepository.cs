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

        public IEnumerator GetPromptResponse(int nodeId, string prompt, int optionHelper, Action<(string, int, int)> callback)
        {
            var uwr = new UnityWebRequest($"{ApiUrl}/completion", "POST");
            var request = new PromptRequest
            {
                pass = GetLocalPassport(),
                prompt = prompt
            };
            var jsonString = JsonConvert.SerializeObject(request);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

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
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/.env");
            return File.ReadAllText(path);
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