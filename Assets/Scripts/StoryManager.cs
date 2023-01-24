using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StoryManager : MonoBehaviour
{
    public Dictionary<int, string> storyContent;
    public Dictionary<int, StoryNode> nodesStaticData;
    public int selectedNode = 1;
    public bool isInitialized = false;
    public Bootstrapper bootstrapper;
    public TextMeshProUGUI content;
    public TextMeshProUGUI button1Text;
    public TextMeshProUGUI button2Text;
    public Button button1;
    public Button button2;
    private PromptRepository repository = new PromptRepository();
    public bool isGeneratingFirstOption = false;
    public bool isGeneratingSecondOption = false;


    void Start()
    {
        storyContent = new Dictionary<int, string>();
    }

    void Update()
    {
        if (isInitialized is false && bootstrapper.isDone is true)
        {
            // Initialization
            StoryInitialization();
            isInitialized = true;
        }

        // Keep screen updated
        if (isInitialized) NodeDataSelection();
    }

    public void NodeDataSelection()
    {
        var node = nodesStaticData[selectedNode];

        if (storyContent.TryGetValue(selectedNode, out var currentContent))
        {
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            string cleanContent = Regex.Replace(currentContent, @"\t|\n|\r", "");
            content.text = cleanContent;

            // Check if it has button data
            if (node.FirstButtonText is not null) 
            {
                button1Text.text = node.FirstButtonText;
            }
            
            if (node.SecondButtonText is not null) 
            {
                button2Text.text = node.SecondButtonText;
            }
            else
            {
                button2.gameObject.SetActive(false);
            }

            if (node.FirstButtonText is null)
            {
                button1Text.text = "Continue";
            }
        }

        // Check to see if it can generate future options
        if (isGeneratingFirstOption is false && node.FirstPathId is not null)
        {
            isGeneratingFirstOption = true;
            var tempNode = nodesStaticData[(int) node.FirstPathId];
            if (storyContent.TryGetValue(tempNode.Id, out var tempContent) is false)
            {
                StartCoroutine(repository.GetPromptResponse(tempNode.Id, tempNode.Prompt, 1, SetGeneratedContent));
            }
            else
            {
                isGeneratingFirstOption = false;
            }
        }

        if (isGeneratingSecondOption is false && node.SecondPathId is not null)
        {
            isGeneratingSecondOption = true;
            var tempNode = nodesStaticData[(int) node.SecondPathId];
            if (storyContent.TryGetValue(tempNode.Id, out var tempContent) is false)
            {
                StartCoroutine(repository.GetPromptResponse(tempNode.Id, tempNode.Prompt, 2, SetGeneratedContent));
            }
            else
            {
                isGeneratingSecondOption = false;
            }
        }
    }

    private void SetGeneratedContent((string content, int nodeId, int optionHelper) result)
    {
        storyContent[result.nodeId] = result.content;
        if (result.optionHelper == 1)
        {
            isGeneratingFirstOption = false;
        }
        else
        {
            isGeneratingSecondOption = false;
        }
    }

    public void StoryInitialization()
    {
        storyContent = bootstrapper.storyContent;
        nodesStaticData = bootstrapper.staticData.Nodes.ToDictionary(k => k.Id, v => v);
    }

    public void ContinueToOption1()
    {
        var node = nodesStaticData[selectedNode];
        selectedNode = (int) node.FirstPathId;
    }
    public void ContinueToOption2()
    {
        var node = nodesStaticData[selectedNode];
        selectedNode = (int) node.SecondPathId;
    }
}
