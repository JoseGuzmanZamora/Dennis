using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts;
using Newtonsoft.Json;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public StoryNodes staticData;
    public PromptRepository repository;
    public Dictionary<int, string> storyContent;
    public Sprite firstImage;
    public bool isDone = false;
    public TextAsset staticJson;
    public TextAsset envFile;
    void Start()
    {
        storyContent = new Dictionary<int, string>();
        repository = new PromptRepository(envFile);
        staticData = LoadStaticData();
        GenerateIntroduction();
    }

    // Read static data from json
    public StoryNodes LoadStaticData()
    {
        return JsonConvert.DeserializeObject<StoryNodes>(staticJson.text);
    }

    // Loop through static data and start generating the story
    public void GenerateIntroduction()
    {
        var firstNode = staticData.Nodes[0];
        //var secondNode = staticData.Nodes[1];
        StartCoroutine(repository.GetPromptResponse(firstNode.Id, firstNode.Prompt, 0, ResultCallback));
        StartCoroutine(repository.DownloadImage(firstNode.ImageUrl, 0, SetImageValue));
        //StartCoroutine(repository.GetPromptResponse(secondNode.Id, secondNode.Prompt, 0, ResultCallback));
    }

    // Adding the result of the introduction to the dictionary
    public void ResultCallback((string content, int nodeId, int optionHelper) result)
    {
        storyContent[result.nodeId] = result.content;
        Debug.Log(JsonConvert.SerializeObject(storyContent));
    }

    public void SetImageValue((Texture2D texture, int optionHelper) result)
    {
        var texture = result.texture;
        firstImage = Sprite.Create (result.texture, new Rect (0, 0, texture.width, texture.height), new Vector2 ());
    }
    
    private void Update() {
        if (storyContent.Count >= 1 && isDone is false)
        {
            isDone = true;
        }
    }
    
}
