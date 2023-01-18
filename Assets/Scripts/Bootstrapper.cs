using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    void Start()
    {
        var repository = new PromptRepository();
        StartCoroutine(repository.GetPromptResponse("Say this is a test"));
    }
}
