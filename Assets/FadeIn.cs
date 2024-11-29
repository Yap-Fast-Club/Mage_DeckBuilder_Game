using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private float fadeSpeed = 1f;

    private void Awake()
    {
        StartCoroutine(Fade(true));
    }
    public IEnumerator Fade(bool isIn)
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = isIn ? 0f : 1f;

        while (true)
        {
            timer += Time.deltaTime * (isIn ? fadeSpeed : -fadeSpeed);

            fader.alpha = timer;

            if (timer >= 1f) break;

            yield return waitFrame;
        }
    }
}
