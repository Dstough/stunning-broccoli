using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 200;
    public int zSize = 200;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        CreatShape();
        UpdateMesh();
        
    }

    void Update()
    { }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void CreatShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int index = 0, z = 0; z <= zSize; z++)
            for (var x = 0; x <= xSize; x++, index++)
                vertices[index] = new Vector3(x, Mathf.PerlinNoise(x * .3f, z * .3f) * 2f, z);

        triangles = new int[xSize * zSize * 6];

        for (int vertexIndex = 0, triangleIndex = 0, z = 0; z < zSize; z++, vertexIndex++)
            for (var x = 0; x < xSize; x++, vertexIndex++, triangleIndex += 6)
            {
                triangles[triangleIndex + 0] = vertexIndex + 0;
                triangles[triangleIndex + 1] = vertexIndex + xSize + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + xSize + 1;
                triangles[triangleIndex + 5] = vertexIndex + xSize + 2;
            }
    }


    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        foreach (var vertex in vertices)
            Gizmos.DrawSphere(vertex, 0.1f);
    }
}
