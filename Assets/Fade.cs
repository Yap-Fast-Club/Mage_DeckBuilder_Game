using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private float fadeSpeed = 1f;

    private void Awake()
    {
        fader.alpha = 0; ;
    }

    public void FadeInFadeOut()
    {
        StartCoroutine(FadeInFadeOutCR());
    }
    private IEnumerator FadeInFadeOutCR()
    {
        yield return StartCoroutine(FadeCR(true));
        yield return StartCoroutine(FadeCR(false));
    }
    public IEnumerator FadeCR(bool isIn)
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = isIn ? 0f : 1f;

        while (true)
        {
            timer += Time.deltaTime * (isIn ? fadeSpeed : -fadeSpeed);

            fader.alpha = timer;

            if (timer >= 1f) break;
            if (timer < 0) break;

            yield return waitFrame;
        }

        //fader.alpha 
    }
}
