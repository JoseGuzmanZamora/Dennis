using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class PromptRepository
    {
        private const string ApiUrl = "https://dennis-backend.fly.dev";

        public IEnumerator GetPromptResponse(string prompt)
        {
            var uwr = new UnityWebRequest($"{ApiUrl}/completion", "POST");
            var request = new PromptRequest
            {
                pass = "test",
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
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }
    }
}