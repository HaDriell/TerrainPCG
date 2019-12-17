using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOperator : TerrainOperator
{
    [SerializeField]
    public Vector3Int size;

    [SerializeField]
    public float value;

    public override void Apply(float[,,] matrix)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                for (int k = 0; k < size.z; k++)
                {
                    int x = (int)(transform.position.x + i - size.x / 2.0f);
                    int y = (int)(transform.position.y + j);
                    int z = (int)(transform.position.z + k - size.z / 2.0f);

                    if (x < 0 || x >= matrix.GetLength(0)) continue;
                    if (y < 0 || y >= matrix.GetLength(1)) continue;
                    if (z < 0 || z >= matrix.GetLength(2)) continue;
                    //Box replaces the terrain to be a given value
                    matrix[x, y, z] = value;
                }
            }
        }
    }
}
