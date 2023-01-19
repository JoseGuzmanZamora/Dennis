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
    void Start()
    {
        storyContent = new Dictionary<int, string>();
        repository = new PromptRepository();
        staticData = LoadStaticData();
        GenerateIntroduction();
    }

    // Read static data from json
    public StoryNodes LoadStaticData()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/StaticData/StoryNodes.json");
        using (StreamReader r = new StreamReader(path))
        {
            string stringValue = r.ReadToEnd();
            return JsonConvert.DeserializeObject<StoryNodes>(stringValue);
        }
    }

    // Loop through static data and start generating the story
    public void GenerateIntroduction()
    {
        var firstNode = staticData.Nodes[0];
        var secondNode = staticData.Nodes[1];
        StartCoroutine(repository.GetPromptResponse(firstNode.Id, firstNode.Prompt, ResultCallback));
        StartCoroutine(repository.GetPromptResponse(secondNode.Id, secondNode.Prompt, ResultCallback));
    }

    // Adding the result of the introduction to the dictionary
    public void ResultCallback((string content, int nodeId) result)
    {
        storyContent[result.nodeId] = result.content;
        Debug.Log(JsonConvert.SerializeObject(storyContent));
    }
}
