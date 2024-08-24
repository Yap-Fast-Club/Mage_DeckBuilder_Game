using UnityEngine;
using Random = System.Random;

public class GridBehaviour : MonoBehaviour
{

    [SerializeField]
    private float size = 1f;
    [SerializeField]
    private bool _showGridLines = true;
    [SerializeField]
    private bool _showGridCells = true;


    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        result += transform.position;

        return result;
    }

    public float Size => size;

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
            for (float x = _minX; x < _maxX; x += size)
            {
                for (float z = _minY; z < _maxY; z += size)
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
            for (float i = -_minX + size / 2; i < _maxX; i += size)
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

            for (float i = -_minY + size / 2; i < _maxY; i += size)
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


