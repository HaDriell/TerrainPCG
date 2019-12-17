using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Simplex2DOperator : TerrainOperator
{
    [SerializeField]
    private Vector3 size = Vector3.zero;

    [SerializeField][Range(1.0f, 10000.0f)]
    private float simplexNoiseScale = 1.0f;
    [SerializeField]
    private float value = 1.0f;
    [SerializeField] 
    private float persistence = 0.5f;
    [SerializeField] 
    private float lacunarity = 2;
    [SerializeField] 
    private int octaves = 1;

    public override void Apply(float[, ,] matrix)
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

                    float height = size.y * Perlin.Noise(x / simplexNoiseScale, z / simplexNoiseScale, persistence, lacunarity, octaves);

                    if (j <= height)
                        matrix[x, y, z] = value;
                }
            }
        }
    }

}
