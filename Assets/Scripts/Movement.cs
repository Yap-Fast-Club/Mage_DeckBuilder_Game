using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] int _movementAmount = 1;
    [SerializeField] int _tileSize = 3;
    [SerializeField] Vector3 _direction;

    [Button]
    public void Move()
    {
        transform.position = transform.position + _direction * _movementAmount * _tileSize;
    }



}
