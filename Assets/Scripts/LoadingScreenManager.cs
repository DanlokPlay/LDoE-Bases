using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public Text loadingText;

    public string[] loadingMessages;
    public float textChangeInterval = 1.0f;

    private int currentMessageIndex = 0;

    void Start()
    {
        StartCoroutine(ChangeLoadingText());
    }

    IEnumerator ChangeLoadingText()
    {
        while (true)
        {
            loadingText.text = loadingMessages[currentMessageIndex];
            yield return new WaitForSeconds(textChangeInterval);
            currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length;
        }
    }

    public void UpdateProgressBar(float progress)
    {
        progressBar.value = progress;
    }
}