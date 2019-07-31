using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainMesh : MonoBehaviour
{
    public Texture2D texture;
    private float heightFactor = 100;

    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private Mesh mesh;

    private void Start() {
        vertices = new Vector3[(texture.width + 1) * (texture.height + 1)];
        triangles = new int[texture.width * texture.height * 6];
        uvs = new Vector2[vertices.Length];
        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        AddVertices();
        AddTriangles();
        Refresh();
        MoveToCenter();
    }

    private void Refresh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void AddVertices() {
        for(int x = 0, i = 0; x <= texture.width; x++) {
            for(int z = 0; z <= texture.height; z++, i++) {
                vertices[i] = new Vector3(
                    x, 
                    GetTextureHeight(x, z) * heightFactor,
                    z
                );
                float uvX = (float)x / (float)texture.width;
                float uvZ = (float)z / (float)texture.height;
                
                uvs[i] = new Vector2(uvX, uvZ);
            }
        }
    }

    private float GetTextureHeight(int x, int z) {
        return texture.GetPixel(x, z).grayscale;
    }

    private void AddTriangles() {
        for(int triIndex = 0, verIndex = 0, x = 0; x < texture.width; x++) {
            for(int z = 0; z < texture.height; z++, triIndex += 6, verIndex++) {
                triangles[triIndex] = verIndex;
                triangles[triIndex + 1] = triangles[triIndex + 4] = verIndex + 1;
                triangles[triIndex + 2] = triangles[triIndex + 3] = verIndex + texture.width + 1;
                triangles[triIndex + 5] = verIndex + texture.width + 2;
            }
        }
    }

    private void MoveToCenter() {
        transform.position -= new Vector3(
            texture.width * 0.5f * transform.localScale.x, 
            0f, 
            texture.height * 0.5f * transform.localScale.z
        );
    }
}
