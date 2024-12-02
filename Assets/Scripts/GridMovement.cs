using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UIElements;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;

[RequireComponent(typeof(GridElement))]
public class GridMovement : MonoBehaviour
{
    private GridElement _gridElement;
    [SerializeField] int _tileAmount = 1;
    [SerializeField] Vector3 _direction;
    [SerializeField, ReadOnly] int _delayBeforeMove = 0;

    private int _pushCollisionDmg => GameManager.Instance.GameplayData.PushCollisionDamage;

    private EnemyBase _thisEnemy;

    private void Awake()
    {
        _gridElement = GetComponent<GridElement>();
        _thisEnemy = GetComponent<EnemyBase>();
    }

    public void SetAmount(int amount)
    {
        _tileAmount = amount;
    }

    public void SetDelay(int delayAmount)
    {
        _delayBeforeMove = delayAmount;
    }

    [Button]
    public void Move()
    {
        if (_delayBeforeMove > 0)
        {
            _delayBeforeMove--;
            this.GetComponent<CharacterBase>()?.CharacterStats.SetCurrentMoveDelay(_delayBeforeMove);
            return;
        }

        StartCoroutine(MoveCR(_tileAmount, _direction));
        _gridElement.SnapToGrid();
    }

    public void GetPushed(int tileAmount)
    {
        StartCoroutine(MoveCR(tileAmount, _direction * -1, onCollision: (c) => { if (c.GetComponent<CharacterBase>() is EnemyBase other) SufferPushDamage(other); }));
    }

    private void SufferPushDamage(EnemyBase otherEnemy)
    {

        if (_pushCollisionDmg == 0) return;

        otherEnemy.CharacterStats.Damage(_pushCollisionDmg, true);
        FxManager.Instance.SpawnFloatingText(otherEnemy.TextSpawnRoot, _pushCollisionDmg.ToString());

        var thisChar = GetComponent<CharacterBase>();
        if (!thisChar) return;

        thisChar.CharacterStats.Damage(_pushCollisionDmg, true);
        FxManager.Instance.SpawnFloatingText(thisChar.TextSpawnRoot, _pushCollisionDmg.ToString());
    }

    private Collider _lastCollision = null;
    private IEnumerator MoveCR(int tileAmount, Vector3 direction, Action<Collider> onCollision = null)
    {
        int remainingTiles = tileAmount;

        _lastCollision = null;

        while (remainingTiles > 0)
        {
            transform.position = transform.position + direction * _gridElement.TileSize;

            yield return new WaitForFixedUpdate();

            remainingTiles--;

            if (_lastCollision != null)
            {
                //Debug.Log($"{_thisEnemy.name} Collided with {_lastCollision}");

                onCollision?.Invoke(_lastCollision);
                yield return new WaitForSeconds(0.05f);
                transform.position = transform.position - direction * _gridElement.TileSize;
                remainingTiles = 0;
            }

            _thisEnemy.EnemyCanvas.HighlightAttack(transform.position.x <= _tileAmount);

            yield return new WaitForSeconds(0.05f);
        }


        _gridElement.SnapToGrid();
    }


    private void OnTriggerEnter(Collider other)
    {
        _lastCollision = other;
    }


}
