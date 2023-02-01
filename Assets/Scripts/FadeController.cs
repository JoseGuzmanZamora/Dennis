using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void FadeToMainScene()
    {
        anim.Play("FadeIn");
        //yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    public void FadeToMenu()
    {
        anim.Play("FadeIn");
        //yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
}
