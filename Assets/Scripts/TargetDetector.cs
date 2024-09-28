using NueGames.NueDeck.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private GridBehaviour grid;


    public enum TargetType { Single, Multiple, Random, Terrain }
    public TargetType currentTargetType = TargetType.Single;

    public Vector3 singleTargetSize = new Vector3(1, 1, 1);
    public Vector3 multipleTargetSize = new Vector3(1, 4);
    public float randomTargetChance = 0.5f; // 50% chance to select an EnemyBase>

    private List<EnemyBase> detectedEnemies = new List<EnemyBase>();
    private HashSet<Vector3> detectedTerrainCells = new HashSet<Vector3>();

    void Start()
    {
        triggerCollider.isTrigger = true;
        grid = FindObjectOfType<GridBehaviour>();
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        switch (currentTargetType)
        {
            case TargetType.Single:
                triggerCollider.transform.localScale = new Vector3(singleTargetSize.x * grid.TileSize, singleTargetSize.y * grid.TileSize, 1);
                break;
            case TargetType.Multiple:
                triggerCollider.transform.localScale = new Vector3(multipleTargetSize.x * grid.TileSize, multipleTargetSize.y * grid.TileSize, 1);
                break;
            case TargetType.Random:
                triggerCollider.transform.localScale = new Vector3(grid.TileSize, grid.TileSize, 1);
                break;
            case TargetType.Terrain:
                triggerCollider.transform.localScale = new Vector3(multipleTargetSize.x * grid.TileSize, multipleTargetSize.y * grid.TileSize);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBase"))
        {
            EnemyBase EnemyBase = collision.GetComponent<EnemyBase>();
            if (!detectedEnemies.Contains(EnemyBase))
            {
                detectedEnemies.Add(EnemyBase);
            }
        }
        else if (collision.CompareTag("Terrain"))
        {
            Vector3 cellPosition = GetCellPosition(collision.transform.position);
            detectedTerrainCells.Add(cellPosition);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBase"))
        {
            EnemyBase EnemyBase = collision.GetComponent<EnemyBase>();
            detectedEnemies.Remove(EnemyBase);
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
        return new Vector3(Mathf.RoundToInt(localPosition.x / grid.TileSize), Mathf.RoundToInt(localPosition.y / grid.TileSize));
    }
}
