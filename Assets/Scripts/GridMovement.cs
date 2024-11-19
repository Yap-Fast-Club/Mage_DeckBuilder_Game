using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UIElements;

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

    public void SetAmount(int amount)
    {
        _tileAmount = amount;
    }

    [Button]
    public void Move()
    {
        transform.position = transform.position + _direction * _tileAmount * _gridElement.TileSize;

        _gridElement.SnapToGrid();
    }

    public void GetPushed(int tileAmount)
    {
        StartCoroutine(GetPushedCR(tileAmount));
    }

    private bool collided = false;
    private IEnumerator GetPushedCR(int tileAmount)
    {
        int remainingTiles = tileAmount;
        collided = false;

        while (remainingTiles > 0) 
        {
            Debug.Log(remainingTiles);
            transform.position = transform.position - _direction * _gridElement.TileSize;

            yield return new WaitForFixedUpdate();

            remainingTiles--;

            if (collided){
                yield return new WaitForSeconds(0.05f);
                transform.position = transform.position + _direction * _gridElement.TileSize;
                remainingTiles = 0;
            }

            yield return new WaitForSeconds(0.05f);
        }


        _gridElement.SnapToGrid();

    }


    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }


}
