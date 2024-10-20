using NaughtyAttributes;
using System.Drawing;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GridElement : MonoBehaviour
{
    [SerializeField, Expandable] private GridSettings _data;
    [SerializeField] private Vector3 _tileOffset = Vector3.zero;
    
    public float TileSize => _data.TileSize;

    void Update()
    {
        SnapToGrid();
    }


    public void SnapToGrid()
    {
        if (_data == null) return;

        //if (!EditorApplication.isPlaying)
        {
            Vector3 currentPosition = transform.position;

            float snappedX = Mathf.Round(currentPosition.x / TileSize) * TileSize + _tileOffset.x;
            float snappedZ = _tileOffset.z;
            float snappedY = Mathf.Round(currentPosition.y / TileSize) * TileSize + _tileOffset.y;

            Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
            transform.position = snappedPosition;
        }
    }
}
