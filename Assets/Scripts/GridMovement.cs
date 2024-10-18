 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridElement))]
public class GridMovement : MonoBehaviour
{
    private GridElement _gridElement;
    [SerializeField] int _tileAmount = 1;
    [SerializeField] Vector3 _direction;

    private void Awake()
    {
        _gridElement = GetComponent<GridElement>();
    }


    public void Move()
    {
        transform.position = transform.position + _direction * _tileAmount * _gridElement.TileSize;

        _gridElement.SnapToGrid();
    }



}
