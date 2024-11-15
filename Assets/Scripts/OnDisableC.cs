using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDisableC : MonoBehaviour
{
    [SerializeField] private UnityEvent OnDisableAction;

    private void OnDisable()
    {
        OnDisableAction?.Invoke();
    }
}
