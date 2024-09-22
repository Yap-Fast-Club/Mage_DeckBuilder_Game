using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

public class GridBehaviour : MonoBehaviour
{
    [SerializeField, InlineEditor] private GridSettings _data;

    [SerializeField]
    private bool _showGridLines = true;
    [SerializeField]
    private bool _showGridCells = true;

    public float TileSize => _data.TileSize;


    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / TileSize);
        int yCount = Mathf.RoundToInt(position.y / TileSize);
        int zCount = Mathf.RoundToInt(position.z / TileSize);

        Vector3 result = new Vector3(
            (float)xCount * TileSize,
            (float)yCount * TileSize,
            (float)zCount * TileSize);

        result += transform.position;

        return result;
    }


    [SerializeField]
    private float _minX = 0;
    [SerializeField]
    private float _minY = 0;
    [SerializeField]
    private float _maxX = 30;
    [SerializeField]
    private float _maxY = 30;

    private void OnDrawGizmos()
    {

        if (TileSize <= 0.01f) return;

        if (_showGridCells)
        {
            Gizmos.color = Color.yellow;
            for (float x = _minX; x < _maxX; x += TileSize)
            {
                for (float y = _minY; y < _maxY; y += TileSize)
                {
                    var point = GetNearestPointOnGrid(new Vector3(x, y, 0));
                    Gizmos.DrawSphere(point, 0.1f);
                }

            }
        }

        if (_showGridLines)
        {
            Gizmos.color = new Color(255, 255, 255, 0.2f);
            Vector3 pos0 = new Vector3();
            Vector3 pos1 = new Vector3();
            for (float i = -_minX + TileSize / 2; i < _maxX; i += TileSize)
            {
                pos0.x = i;
                pos0.y = -_minY;
                pos1.x = i;
                pos1.y = _maxY;
                Gizmos.DrawLine(
                    pos0,
                    pos1
                );
            }

            for (float i = -_minY + TileSize / 2; i < _maxY; i += TileSize)
            {
                pos0.x = -_minX;
                pos0.y = i;
                pos1.x = _maxX;
                pos1.y = i;
                Gizmos.DrawLine(
                    pos0,
                    pos1
                );
            }
        }
    }
}


