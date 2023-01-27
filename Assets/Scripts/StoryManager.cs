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
    public TextMeshProUGUI button3Text;
    public Button button1;
    public Button button2;
    public Button button3;
    private PromptRepository repository = new PromptRepository();
    public bool isGeneratingFirstOption = false;
    public bool isGeneratingSecondOption = false;
    public GameObject loader;
    public int? attemptedChoice;
    public Image helperImage;
    private Sprite imageOption1;
    private Sprite imageOption2;


    void Start()
    {
        storyContent = new Dictionary<int, string>();
    }

    void Update()
    {
        // First validation to check if we have to block user interaction
        if (bootstrapper.isDone is false && loader.activeSelf is false)
        {
            loader.SetActive(true);
        }

        // Second validation to see if we can initialize the story flow
        if (isInitialized is false && bootstrapper.isDone is true)
        {
            // Initialization
            StoryInitialization();
            isInitialized = true;
            loader.SetActive(false);
        }

        // Keep screen updated
        if (isInitialized) NodeDataSelection();
    }

    public void NodeDataSelection()
    {
        // This flow will keep the screen content updated along with the buttons
        var node = nodesStaticData[selectedNode];

        if (storyContent.TryGetValue(selectedNode, out var currentContent))
        {
            // Set the content value
            string cleanContent = Regex.Replace(currentContent, @"\t|\n|\r", "");
            content.text = cleanContent;

            // Check if it has button data
            if (node.FirstButtonText is not null) 
            {
                button1Text.text = node.FirstButtonText;
                button3Text.text = node.FirstButtonText;
            }
            else
            {
                button1Text.text = "Continue";
                button3Text.text = "Continue";
            }
            
            if (node.SecondButtonText is not null) 
            {
                button2Text.text = node.SecondButtonText;
            }
        }

        // Check to see if it can generate future options, this is to optimize the user experience
        if (isGeneratingFirstOption is false && node.FirstPathId is not null)
        {
            isGeneratingFirstOption = true;
            var tempNode = nodesStaticData[(int) node.FirstPathId];
            if (storyContent.TryGetValue(tempNode.Id, out var tempContent) is false)
            {
                StartCoroutine(repository.GetPromptResponse(tempNode.Id, tempNode.Prompt, 1, SetGeneratedContent));
                StartCoroutine(repository.DownloadImage(tempNode.ImageUrl, 1, SetImageValue));
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
                StartCoroutine(repository.DownloadImage(tempNode.ImageUrl, 2, SetImageValue));
            }
            else
            {
                isGeneratingSecondOption = false;
            }
        }
    }

    // Callback function called by the prompt coroutine. Thisone sets thestore content values
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

        // Checks if there was an attempted choice and we update the selected node value
        if (attemptedChoice == 1)
        {
            ContinueToOption1();
            attemptedChoice = null;
        }
        else if (attemptedChoice == 2)
        {
            ContinueToOption2();
            attemptedChoice = null;
        }

        // Only quit the loader if we finished generating future responses
        if (isGeneratingFirstOption == false && isGeneratingSecondOption == false) loader.SetActive(false);
    }

    public void StoryInitialization()
    {
        storyContent = bootstrapper.storyContent;
        nodesStaticData = bootstrapper.staticData.Nodes.ToDictionary(k => k.Id, v => v);
        helperImage.sprite = bootstrapper.firstImage;
    }

    public void ContinueToOption1()
    {
        if (isGeneratingFirstOption)
        {
            loader.SetActive(true);
            attemptedChoice = 1;
        }
        else
        {
            StoryAndButtonsSet(imageOption1, 1);
        }
    }
    public void ContinueToOption2()
    {
        if (isGeneratingSecondOption)
        {
            loader.SetActive(true);
            attemptedChoice = 2;
        }
        else
        {
            StoryAndButtonsSet(imageOption2, 2);
        }
    }

    public void StoryAndButtonsSet(Sprite imageToSet, int path)
    {
        if (button1.gameObject.activeSelf is false)  button1.gameObject.SetActive(true);
        if (button2.gameObject.activeSelf is false) button2.gameObject.SetActive(true);
        if (button3.gameObject.activeSelf is true) button3.gameObject.SetActive(false);

        var node = nodesStaticData[selectedNode];
        selectedNode = path == 1 ? (int) node.FirstPathId : (int) node.SecondPathId;
        node = nodesStaticData[selectedNode];
        helperImage.sprite = imageToSet;

        if (node.SecondButtonText is null)
        {
            // If the second button has no data then we show the center one
            if (button1.gameObject.activeSelf is true)  button1.gameObject.SetActive(false);
            if (button2.gameObject.activeSelf is true) button2.gameObject.SetActive(false);
            if (button3.gameObject.activeSelf is false) 
            {
                button3.interactable = false;
                button3.gameObject.SetActive(true);
                button3.interactable = true;
            }
        }
    }

    public void SetImageValue((Texture2D texture, int optionHelper) result)
    {
        if (result.optionHelper == 1)
        {
            imageOption1 = Sprite.Create (result.texture, new Rect (0, 0, 350, 350), new Vector2 ());
        }
        else
        {
            imageOption2 = Sprite.Create (result.texture, new Rect (0, 0, 350, 350), new Vector2 ());
        }
    }
}
