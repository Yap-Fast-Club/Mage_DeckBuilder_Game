using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableC : MonoBehaviour
{
    [SerializeField] private UnityEvent OnEnableAction;

    private void OnEnable()
    {
        OnEnableAction?.Invoke();
    }
}
