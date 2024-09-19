using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _enemies;

    private bool _levelActive = false;

    private enum TurnPhase
    {
        Player,
        Enemy
    }

    private TurnPhase _turnPhase = TurnPhase.Player;

    void Start()
    {
        _turnPhase = TurnPhase.Player;

        StartCoroutine(LoopCR());
        _levelActive = true;  
    }

    void Update()
    {
           
    }


    private IEnumerator LoopCR()
    {
        while (_levelActive) 
        {
            if (_turnPhase == TurnPhase.Player)
            {
                yield return StartCoroutine(PlayerTurnCR());
            }
            if (_turnPhase == TurnPhase.Enemy)
            {
                yield return StartCoroutine(EnemyTurnCR());
            }

            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
            _turnPhase++;
        }

    }


    private IEnumerator PlayerTurnCR()
    {
        yield break;
    }

    private IEnumerator EnemyTurnCR()
    { 
        yield break;
    }

}
