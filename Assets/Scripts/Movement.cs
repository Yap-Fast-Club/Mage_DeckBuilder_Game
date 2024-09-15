using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] int _movementAmount = 1;
    [SerializeField] int _tileSize = 3;


    [Button]
    public void Move()
    {
        transform.position = transform.position + transform.forward * _movementAmount * _tileSize;
    }



}
