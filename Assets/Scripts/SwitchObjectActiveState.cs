using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SwitchObjectActiveState : MonoBehaviour
{
    [FormerlySerializedAs("activeGameObject")]
    public GameObject targetObject;

    [SerializeField] private float initialDelay = 0.01f;
    [SerializeField] private bool activateOnEnable = false;

    float timer = 0;
    private void OnEnable()
    {
        if (!activateOnEnable) return;
        timer = 0;
        StartCoroutine(ActivationCR());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ActivationCR()
    {
        while (timer <= initialDelay)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        ActivateObject();
    }


    public void ActivateObject()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
}
