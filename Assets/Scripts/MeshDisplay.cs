using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshDisplay : MapDisplay
{
    public List<Color> colors;

    [Range(1, 10)]
    public int textureToModelRatio = 2;

    public float heightFactor = 20;
    public float quadSize = 1;

    public enum MeshCenter
    {
        Middle,
        LowerLeft,
        UpperRight
    };

    public MeshCenter meshCenter;

    public MeshFilter meshFilter;
    public Renderer textureRenderer;

    public override void DisplayMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Mesh mesh = CreateMesh(heightMap, heightMap.GetLength(0), heightMap.GetLength(1));

        meshFilter.mesh = mesh;

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = GetColorForIntensity(heightMap[x, y]);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        transform.localScale = Vector3.one;
    }

    private Mesh CreateMesh(float[,] heightMap, int width, int height)
    {
        Mesh mesh = new Mesh();
        mesh.name = "LandmassMesh";
        mesh.Clear();

        int vertCount = (height + 1) * (width + 1);
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] faces = new int[6 * height * width];

        int vertRows = height + 1;
        int vertCols = width + 1;

        Vector2 origin = new Vector2(0, 0);
        if (meshCenter == MeshCenter.Middle)
        {
            origin.x = (width * quadSize) / 2;
            origin.y = (height * quadSize) / 2;
        }
        else if (meshCenter == MeshCenter.UpperRight)
        {
            origin.x = width * quadSize;
            origin.y = height * quadSize;
        }

        for (int y = 0; y < vertRows; y++)
        {
            for (int x = 0; x < vertCols; x++)
            {
                float vertY;

                if (x != 0 && y != 0 && x != vertCols - 1 && y != vertRows - 1)
                {
                    // everything that is not on the edge of the mesh
                    float bottomRight = heightMap[x, y];
                    float bottomLeft = heightMap[x - 1, y];
                    float topRigth = heightMap[x, y - 1];
                    float topLeft = heightMap[x - 1, y - 1];

                    vertY = (bottomRight + bottomLeft + topLeft + topRigth) / 4f;
                }
                else
                {
                    if (x == 0)
                    {
                        if (y == 0 || y == vertRows - 1)
                        {
                            // top left and bottom left corners

                            vertY = heightMap[0, y - 1 < 0 ? 0 : y - 1];
                        } else
                        {
                            // left edge
                            float bottomRight = heightMap[0, y];
                            float topRight = heightMap[0, y - 1];

                            vertY = (topRight + bottomRight) / 2f;
                        }

                    } else if (x == vertCols - 1)
                    {
                        if (y == 0 || y == vertRows - 1)
                        {
                            // top right and bottom right corners

                            vertY = heightMap[width - 1, y - 1 < 0 ? 0 : y - 1];
                        }
                        else
                        {
                            // right edge
                            float bottomLeft = heightMap[width - 1, y];
                            float topLeft = heightMap[width - 1, y - 1];

                            vertY = (bottomLeft + topLeft) / 2f;
                        }
                    } else
                    {
                        if (y == 0)
                        {
                            // top edge 
                            float bottomRight = heightMap[x, 0];
                            float bottomLeft = heightMap[x - 1, 0];

                            vertY = (bottomRight + bottomLeft) / 2f; 
                        }
                        else
                        {
                            // bottom edge
                            float topRight = heightMap[x, height - 1];
                            float topLeft = heightMap[x - 1, height - 1];

                            vertY = (topRight + topLeft) / 2f;
                        }
                    }
                }

                vertices[y * vertCols + x] = new Vector3(-origin.x + x * quadSize, vertY * heightFactor, -origin.y + y * quadSize);
                uv[y * vertCols + x] = new Vector2((float)x / vertCols, (float)y / vertRows);
            }
        }

        int j, i;
        int lastColumn = vertCols - 1;
        for (j = 0, i = 0; i < faces.Length; j++, i += 6)
        {

            if (j == lastColumn)
            {
                lastColumn += vertCols;
                j++;
            }

            faces[i] = j;
            faces[i + 1] = j + vertCols;
            faces[i + 2] = j + 1;


            faces[i + 3] = j + 1;
            faces[i + 4] = j + vertCols;
            faces[i + 5] = j + vertCols + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = faces;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private Color GetColorForIntensity(float intensity)
    {
        float resolution = 1f / colors.Count;
        int index = Mathf.FloorToInt(intensity / resolution);

        index = Mathf.Clamp(index, 0, colors.Count - 1);

        return colors[index];
    }
}
