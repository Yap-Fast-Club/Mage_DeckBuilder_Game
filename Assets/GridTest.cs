using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class GridTest : MonoBehaviour
{
    [SerializeField]
    public GameObject cellPrefab;

    [SerializeField]
    private Grid grid;

    [SerializeField] private int rowCount;
    [SerializeField] private int colCount;

    [HideInInspector] public Transform cellsContainer;


    private void Start()
    {
        ClearGrid();
        InstantiateCells();
    }

    [Button]
    public void InstantiateCells()
    {
        if (grid == null)
        {
            Debug.LogError("El componente Grid no se encontró en este GameObject.");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("El prefab de la celda no ha sido asignado.");
            return;
        }

        ClearGrid();

        cellsContainer = new GameObject("CellsContainer").transform;
        cellsContainer.SetParent(transform);
        cellsContainer.localPosition = Vector3.zero;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Vector3Int cellGridPosition = new Vector3Int(col, row, 0);
                Vector3 cellWorldPosition = grid.CellToWorld(cellGridPosition) + grid.cellSize / 2f;
                GameObject cell = Instantiate(cellPrefab, cellWorldPosition, Quaternion.identity, cellsContainer);
                cell.transform.localScale = grid.cellSize;
                cell.name = $"Cell_{col}_{row}";
            }
        }
    }
    [Button]
    public void ClearGrid()
    {
        if (cellsContainer != null)
        {
            DestroyImmediate(cellsContainer.gameObject);
            cellsContainer = null;
        }
    }

}