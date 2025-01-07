using UnityEngine;
using Random = System.Random;


public class HexGridBehaviour : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;
    public Color gizmoColor = Color.white;

    private Vector3[] hexCorners = new Vector3[6];

    void Start()
    {
        GenerateHexCorners();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        for (int z = 0; z < height; z++)
        {
            bool isOddRow = z % 2 == 1;
            float rowOffset = isOddRow ? tileSize * 0.75f : 0;

            for (int x = 0; x < width; x++)
            {
                Vector3 center = new Vector3((x * tileSize) + rowOffset, 0, (z * tileSize * 0.75f));
                Gizmos.color = gizmoColor;
                for (int i = 0; i < hexCorners.Length; i++)
                {
                    int nextIndex = (i + 1) % hexCorners.Length;
                    Gizmos.DrawLine(center + hexCorners[i], center + hexCorners[nextIndex]);
                }
            }
        }
    }

    void GenerateHexCorners()
    {
        float angle;
        hexCorners[0] = new Vector3(0, 0, tileSize); //top
        for (int i = 1; i < hexCorners.Length; i++)
        {
            angle = 2 * Mathf.PI / hexCorners.Length * i;
            hexCorners[i] = new Vector3(Mathf.Cos(angle) * tileSize, 0, Mathf.Sin(angle) * tileSize);
        }
    }
}


