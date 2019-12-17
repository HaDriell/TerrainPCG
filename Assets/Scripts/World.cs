using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class World : MonoBehaviour
{
    private MeshFilter meshFilter;

    [SerializeField]
    public int width;
    [SerializeField]
    public int height;
    [SerializeField]
    public int depth;

    [SerializeField]
    public int snowHeight;
    [SerializeField]
    public int grassValue;
    [SerializeField]
    public int rockValue;

    [SerializeField]
    public Color grassColor = Color.green;
    [SerializeField]
    public Color rockColor = Color.gray;
    [SerializeField]
    public Color snowColor = Color.white;
    

    public int SnowHeight { get => snowHeight; }
    public int GrassValue { get => grassValue; }
    public int RockValue { get => rockValue; }

    public float[,,] Matrix { get; set; }

    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public int Depth { get => depth; set => depth = value; }

    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void Compute()
    {
        ComputeMatrix();
        ComputeMesh();
    }

    private void ComputeMatrix()
    {
        Matrix = new float[Width, Height, Depth];
        foreach (TerrainOperator op in GetComponentsInChildren<TerrainOperator>())
        {
            op.Apply(Matrix);
        }
    }

    private void ComputeMesh()
    {
        List<Color> colors = new List<Color>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int k = 0; k < Depth; k++)
                {
                    if (IsSolid(Matrix, i, j, k))
                    {
                        //Dirty AF. Switch for a proper dictionary later on. *pukes of disgust*
                        Color materialColor = Color.black;
                        int materialValue = (int)Matrix[i, j, k];
                        switch (materialValue)
                        {
                            case 1: materialColor = rockColor; break;
                            case 2: materialColor = grassColor; break;
                        }
                        if (j >= snowHeight) materialColor = snowColor;


                        Surface(Matrix, i, j, k, 0.5f, materialColor, vertices, colors, indices);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.colors = colors.ToArray();
        mesh.triangles = indices.ToArray();
        meshFilter.mesh = mesh;
    }

    private static bool IsSolid(float[,,] matrix, int x, int y, int z)
    {
        if (x < 0 || x >= matrix.GetLength(0)) return false;
        if (y < 0 || y >= matrix.GetLength(1)) return false;
        if (z < 0 || z >= matrix.GetLength(2)) return false;
        return matrix[x, y, z] > 0;
    }

    //TODO : refactor that
    private static void Cube(Vector3 center, float halfSize, List<Vector3> vertices, List<int> indices)
    {
        //We're adding a bunch of vertices. If they're not the "first" ones, we want to offset the indices
        int offset = vertices.Count;
        //Front panel
        vertices.Add(new Vector3(center.x - halfSize, center.y - halfSize, center.z - halfSize));
        vertices.Add(new Vector3(center.x + halfSize, center.y - halfSize, center.z - halfSize));
        vertices.Add(new Vector3(center.x + halfSize, center.y + halfSize, center.z - halfSize));
        vertices.Add(new Vector3(center.x - halfSize, center.y + halfSize, center.z - halfSize));
        //Back panel
        vertices.Add(new Vector3(center.x - halfSize, center.y + halfSize, center.z + halfSize));
        vertices.Add(new Vector3(center.x + halfSize, center.y + halfSize, center.z + halfSize));
        vertices.Add(new Vector3(center.x + halfSize, center.y - halfSize, center.z + halfSize));
        vertices.Add(new Vector3(center.x - halfSize, center.y - halfSize, center.z + halfSize));

        //Faces indices
        indices.Add(offset + 0); indices.Add(offset + 2); indices.Add(offset + 1); //face front
        indices.Add(offset + 0); indices.Add(offset + 3); indices.Add(offset + 2);
        indices.Add(offset + 2); indices.Add(offset + 3); indices.Add(offset + 4); //face top
        indices.Add(offset + 2); indices.Add(offset + 4); indices.Add(offset + 5);
        indices.Add(offset + 1); indices.Add(offset + 2); indices.Add(offset + 5); //face right
        indices.Add(offset + 1); indices.Add(offset + 5); indices.Add(offset + 6);
        indices.Add(offset + 0); indices.Add(offset + 7); indices.Add(offset + 4); //face left
        indices.Add(offset + 0); indices.Add(offset + 4); indices.Add(offset + 3);
        indices.Add(offset + 5); indices.Add(offset + 4); indices.Add(offset + 7); //face back
        indices.Add(offset + 5); indices.Add(offset + 7); indices.Add(offset + 6);
        indices.Add(offset + 0); indices.Add(offset + 6); indices.Add(offset + 7); //face bottom
        indices.Add(offset + 0); indices.Add(offset + 1); indices.Add(offset + 6);
    }

    private static void Surface(float[,,] matrix, int x, int y, int z, float halfSize, Color color, List<Vector3> vertices, List<Color> colors, List<int> indices)
    {
        //We're adding a bunch of vertices. If they're not the "first" ones, we want to offset the indices
        int offset = vertices.Count;

        //Front panel (4 vertices)
        vertices.Add(new Vector3(x - halfSize, y - halfSize, z - halfSize));
        vertices.Add(new Vector3(x + halfSize, y - halfSize, z - halfSize));
        vertices.Add(new Vector3(x + halfSize, y + halfSize, z - halfSize));
        vertices.Add(new Vector3(x - halfSize, y + halfSize, z - halfSize));
        //Back panel (4 vertices)
        vertices.Add(new Vector3(x - halfSize, y + halfSize, z + halfSize));
        vertices.Add(new Vector3(x + halfSize, y + halfSize, z + halfSize));
        vertices.Add(new Vector3(x + halfSize, y - halfSize, z + halfSize));
        vertices.Add(new Vector3(x - halfSize, y - halfSize, z + halfSize));

        //Colors (4 front vertices)
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        //Colors (4 back vertices)
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        //Faces instanciation
        if (!IsSolid(matrix, x, y, z - 1))
        {
            indices.Add(offset + 0); indices.Add(offset + 2); indices.Add(offset + 1); //face front
            indices.Add(offset + 0); indices.Add(offset + 3); indices.Add(offset + 2);
        }

        if (!IsSolid(matrix, x, y + 1, z))
        {
            indices.Add(offset + 2); indices.Add(offset + 3); indices.Add(offset + 4); //face top
            indices.Add(offset + 2); indices.Add(offset + 4); indices.Add(offset + 5);
        }

        if (!IsSolid(matrix, x + 1, y, z))
        {
            indices.Add(offset + 1); indices.Add(offset + 2); indices.Add(offset + 5); //face right
            indices.Add(offset + 1); indices.Add(offset + 5); indices.Add(offset + 6);
        }

        if (!IsSolid(matrix, x - 1, y, z))
        {
            indices.Add(offset + 0); indices.Add(offset + 7); indices.Add(offset + 4); //face left
            indices.Add(offset + 0); indices.Add(offset + 4); indices.Add(offset + 3);
        }

        if (!IsSolid(matrix, x, y, z + 1))
        {
            indices.Add(offset + 5); indices.Add(offset + 4); indices.Add(offset + 7); //face back
            indices.Add(offset + 5); indices.Add(offset + 7); indices.Add(offset + 6);
        }

        if (!IsSolid(matrix, x, y - 1, z))
        {
            indices.Add(offset + 0); indices.Add(offset + 6); indices.Add(offset + 7); //face bottom
            indices.Add(offset + 0); indices.Add(offset + 1); indices.Add(offset + 6);
        }
    }
}