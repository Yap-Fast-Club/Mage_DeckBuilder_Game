using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonInteractableDelay : MonoBehaviour
{
    [SerializeField] private float delay;

    private Button button;
    private float timer = 0;
    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= -0.5f) this.enabled = false;

        timer -= Time.deltaTime;

        if (timer <= 0)
            button.interactable = true;
    }
}
