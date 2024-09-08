using UnityEngine;
using Random = System.Random;

public class GridBehaviour : MonoBehaviour
{
    [SerializeField] private GridSettings _data;

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
        

        if (_showGridCells)
        {
            Gizmos.color = Color.yellow;
            for (float x = _minX; x < _maxX; x += TileSize)
            {
                for (float z = _minY; z < _maxY; z += TileSize)
                {
                    var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
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
                pos0.z = -_minY;
                pos1.x = i;
                pos1.z = _maxY;
                Gizmos.DrawLine(
                    pos0,
                    pos1
                );
            }

            for (float i = -_minY + TileSize / 2; i < _maxY; i += TileSize)
            {
                pos0.x = -_minX;
                pos0.z = i;
                pos1.x = _maxX;
                pos1.z = i;
                Gizmos.DrawLine(
                    pos0,
                    pos1
                );
            }
        }
    }
}


