using NueGames.NueDeck.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private Collider _triggerCollider;
    [SerializeField] private GridElement _gridElement;


    //private GridBehaviour _gridElement;

    public enum TargetType { Single, Multiple, Random, Terrain }
    public TargetType currentTargetType = TargetType.Single;

    public Vector3 singleTargetSize = new Vector3(1, 1, 1);
    public Vector3 multipleTargetSize = new Vector3(1, 4);
    public float randomTargetChance = 0.5f; // 50% chance to select an EnemyBase>

    private List<EnemyBase> detectedEnemies = new List<EnemyBase>();
    private HashSet<Vector3> detectedTerrainCells = new HashSet<Vector3>();

    void Start()
    {
        _triggerCollider.isTrigger = true;
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
        _gridElement.SnapToGrid();

        switch (currentTargetType)
        {
            case TargetType.Single:
                _triggerCollider.transform.localScale = new Vector3(singleTargetSize.x * _gridElement.TileSize, singleTargetSize.y * _gridElement.TileSize, 1);
                break;
            case TargetType.Multiple:
                _triggerCollider.transform.localScale = new Vector3(multipleTargetSize.x * _gridElement.TileSize, multipleTargetSize.y * _gridElement.TileSize, 1);
                break;
            case TargetType.Random:
                _triggerCollider.transform.localScale = new Vector3(_gridElement.TileSize, _gridElement.TileSize, 1);
                break;
            case TargetType.Terrain:
                _triggerCollider.transform.localScale = new Vector3(multipleTargetSize.x * _gridElement.TileSize, multipleTargetSize.y * _gridElement.TileSize);
                break;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase EnemyBase = collision.GetComponent<EnemyBase>();
            if (!detectedEnemies.Contains(EnemyBase))
            {
                detectedEnemies.Add(EnemyBase);
                Debug.Log($"Added enemy {EnemyBase.gameObject} to targets");
            }
        }
        else if (collision.CompareTag("Terrain"))
        {
            Vector3 cellPosition = GetCellPosition(collision.transform.position);
            detectedTerrainCells.Add(cellPosition);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase EnemyBase = collision.GetComponent<EnemyBase>();
            detectedEnemies.Remove(EnemyBase);
                Debug.Log($"Removed enemy {EnemyBase.gameObject} to targets");
        }
        else if (collision.CompareTag("Terrain"))
        {
            Vector3 cellPosition = GetCellPosition(collision.transform.position);
            detectedTerrainCells.Remove(cellPosition);
        }
    }

    public List<EnemyBase> GetDetectedEnemies()
    {
        return detectedEnemies;
    }

    public HashSet<Vector3> GetDetectedTerrainCells()
    {
        return detectedTerrainCells;
    }

    public EnemyBase GetSingleTarget()
    {
        if (detectedEnemies.Count == 1)
        {
            return detectedEnemies[0];
        }
        return null;
    }

    public List<EnemyBase> GetMultipleTargets()
    {
        return detectedEnemies;
    }

    public EnemyBase GetRandomTarget()
    {
        if (detectedEnemies.Count > 0 && Random.value < randomTargetChance)
        {
            return detectedEnemies[Random.Range(0, detectedEnemies.Count)];
        }
        return null;
    }

    private Vector3 GetCellPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        return new Vector3(Mathf.RoundToInt(localPosition.x / _gridElement.TileSize), Mathf.RoundToInt(localPosition.y / _gridElement.TileSize));
    }
}
