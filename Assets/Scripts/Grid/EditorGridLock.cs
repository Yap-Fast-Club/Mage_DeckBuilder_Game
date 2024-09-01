using System.Drawing;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class EditorHexGridLock : MonoBehaviour
{
    public float tileSize = 1f;
    public Vector3 tileOffset = Vector3.zero;
    public bool isOddRow = false; // Determines if the current row is odd or even for hexagonal grid alignment

#if UNITY_EDITOR
    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Vector3 currentPosition = transform.position;

            // Calculate the snapped position based on a hexagonal grid
            float snappedX = Mathf.Round((currentPosition.x - (isOddRow ? tileSize * 0.75f : 0)) / tileSize) * tileSize + tileOffset.x;
            float snappedZ = Mathf.Round(currentPosition.z / (tileSize * 0.75f)) * tileSize * 0.75f + tileOffset.z; 
            float snappedY = tileOffset.y;

            Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
            transform.position = snappedPosition;
        }
    }
#endif
}