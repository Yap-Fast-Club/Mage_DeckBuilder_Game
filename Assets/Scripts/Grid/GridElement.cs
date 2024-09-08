using System.Drawing;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GridElement : MonoBehaviour
{

    [SerializeField] private GridSettings _data;
    [SerializeField] private Vector3 _tileOffset = Vector3.zero;
    
    private float _tileSize => _data.TileSize;

    void Update()
    {

        if (!Application.isPlaying)
        {
            SnapToGrid();
        }
    }


    private void SnapToGrid()
    {
        if (_data == null) return;

        if (!EditorApplication.isPlaying)
        {
            Vector3 currentPosition = transform.position;

            float snappedX = Mathf.Round(currentPosition.x / _tileSize) * _tileSize + _tileOffset.x;
            float snappedZ = Mathf.Round(currentPosition.z / _tileSize) * _tileSize + _tileOffset.z;
            float snappedY = _tileOffset.y;

            Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
            transform.position = snappedPosition;
        }
    }
}
