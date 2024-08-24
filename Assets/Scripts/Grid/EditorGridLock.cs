using System.Drawing;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class EditorGridLock : MonoBehaviour
{
    public float tileSize = 1;
    public Vector3 tileOffset = Vector3.zero;

#if UNITY_EDITOR
    

    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Vector3 currentPosition = transform.position;

            float snappedX = Mathf.Round(currentPosition.x / tileSize) * tileSize + tileOffset.x;
            float snappedZ = Mathf.Round(currentPosition.z / tileSize) * tileSize + tileOffset.z;
            float snappedY = tileOffset.y; // Preserve the original y-coordinate

            Vector3 snappedPosition = new Vector3(snappedX, snappedY, snappedZ);
            transform.position = snappedPosition;
        }
    }
#endif
    //public Vector3 GetNearestPointOnGrid(Vector3 position)
    //{
    //    position -= transform.position;

    //    int xCount = Mathf.RoundToInt(position.x / tileSize);
    //    int yCount = Mathf.RoundToInt(position.y / tileSize);
    //    int zCount = Mathf.RoundToInt(position.z / tileSize);

    //    Vector3 result = new Vector3(
    //        (float)xCount * tileSize,
    //        (float)yCount * size,
    //        (float)zCount * size);

    //    result += transform.position;

    //    return result;
    //}
}