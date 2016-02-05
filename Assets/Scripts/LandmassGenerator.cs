using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandmassGenerator : MonoBehaviour {

    [Range(1, 100)]
    public float lucanarity = 2;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Range(1, 1024)]
    public int width = 200;
    [Range(1, 1024)]
    public int height = 200;
    [Range(1, 5)]
    public int octaveCount = 3;

    [Range(0, 1)]
    public float islandPercentage = 0.8f;
    [Range(0, 1)]
    public float coastPercentage = 0.2f;

    public List<Color> colors;

    private float xOrg;
    private float yOrg;

    public int samplingSize = 200;
    public int scaleSize = 200;

    public float heightFactor = 20;
    public float quadSize = 1;

    private Renderer plane_renderer;

    public enum MeshCenter
    {
        Middle,
        LowerLeft,
        UpperRight
    };

    public MeshCenter meshCenter;

	// Use this for initialization
	void Start () {
        plane_renderer = GetComponent<Renderer>();
        xOrg = Random.Range(0, 1024);
        yOrg = Random.Range(0, 1024);

        generateHeightmap();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void generateHeightmap()
    {
        Debug.Log("Rendering again ...");

        xOrg = Random.Range(0, 1024);
        yOrg = Random.Range(0, 1024);

        Texture2D noiseTexture = createNoiseTexture();
        GetComponent<Renderer>().sharedMaterial.mainTexture = noiseTexture;
    }

    public Texture2D createNoiseTexture()
    {
        Texture2D noiseTex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        float[] pixelIntensity = new float[width * height];
        for (int octave = 0; octave < octaveCount; octave++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float xCoord = xOrg + (float)x / samplingSize * Mathf.Pow(lucanarity, octave);
                    float yCoord = yOrg + (float)y / samplingSize * Mathf.Pow(lucanarity, octave);
                    float noise = (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * Mathf.Pow(persistance, octave);
                    pixelIntensity[y * width + x] += noise;
                }
            }
        }

        float radiusX = width / 2f;
        float radiusy = height / 2f;

        calculateMaskVariables();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float centerToX = x - radiusX;
                float centerToY = y - radiusy;

                float maskFactor = getMaskFactor(centerToX, centerToY);

                float pixel = Mathf.Clamp((pixelIntensity[y * width + x] + 0.5f), 0, 1) * maskFactor;
                pixelIntensity[y * width + x] = (pixelIntensity[y * width + x] + 0.5f) * maskFactor;
                Color color = getColorForIntensity(pixel);
                pixels[y * width + x] = color;
            }
        }

        Mesh mesh = createMesh(pixelIntensity);

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        GetComponent<MeshFilter>().mesh = mesh;
        //transform.localScale = new Vector3((float) width / scaleSize, 1, (float) height / scaleSize);

        return noiseTex;
    }

    private float majorCurveA;
    private float majorCurveB;

    private float minorCurveA;
    private float minorCurveB;

    private void calculateMaskVariables()
    {
        majorCurveA = width / 2f * islandPercentage;
        majorCurveB = height / 2f * islandPercentage;

        float coastSize = ((majorCurveA + majorCurveB) * coastPercentage) / 2;

        minorCurveA = majorCurveA - coastSize;
        minorCurveB = majorCurveB - coastSize;
    }

    private float getMaskFactor(float x, float y)
    {
        float factor = 1;

        float distancePoint = Mathf.Sqrt(x * x + y * y);

        float k;
        if (x != 0)
        {
            k = y / x;

            float x1 = 1 / Mathf.Pow(Mathf.Pow(majorCurveA, -4f) + Mathf.Pow(k / majorCurveB, 4), 1/4f);
            float y1 = k * x1;
            float distanceMajor = Mathf.Sqrt(x1 * x1 + y1 * y1);

            float x2 = 1 / Mathf.Pow(Mathf.Pow(minorCurveA, -4f) + Mathf.Pow(k / minorCurveB, 4), 1 / 4f);
            float y2 = k * x2;
            float distanceMinor = Mathf.Sqrt(x2 * x2 + y2 * y2);

            factor = 1f - Mathf.Clamp((distancePoint - distanceMinor) / (distanceMajor - distanceMinor), 0, 1);
        }
        else
        {
            factor = 1f - Mathf.Clamp((distancePoint - minorCurveB) / (majorCurveB - minorCurveB), 0, 1);
        }

        return factor;
    }

    private Color getColorForIntensity(float intensity)
    {
        float resolution = 1f / colors.Count;
        int index = Mathf.FloorToInt(intensity / resolution);
        if (index == colors.Count)
        {
            index--;
        }
        return colors[index];
    }

    private Color getGrayscaleForIntensity(float intensity)
    {
        return new Color(intensity, intensity, intensity);
    }

    private Mesh createMesh(float[] pixelIntensity)
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
        } else if (meshCenter == MeshCenter.UpperRight)
        {
            origin.x = width * quadSize;
            origin.y = height * quadSize;
        }

        for (int y = 0; y < vertRows ; y++)
        {
            for (int x = 0; x < vertCols; x++)
            {
                if (x == 0 || y == 0 || x == vertCols - 1 || y == vertRows - 1) 
                {
                    vertices[y * vertCols + x] = new Vector3(-origin.x + x * quadSize, 0, -origin.y + y * quadSize);
                    uv[y * vertCols + x] = new Vector2((float) x / vertCols, (float) y / vertRows);
                } else
                {
                    // pixels
                    float downRight = pixelIntensity[y * width + x];
                    float downLeft = pixelIntensity[y * width + (x - 1)];
                    float upperRigth = pixelIntensity[(y - 1) * width + x];
                    float upperLeft = pixelIntensity[(y - 1) * width + (x - 1)];

                    float vertY = (downRight + downLeft + upperLeft + upperRigth) / 4f * heightFactor;

                    vertices[y * vertCols + x] = new Vector3(-origin.x + x * quadSize, vertY, -origin.y + y * quadSize);
                    uv[y * vertCols + x] = new Vector2((float) x / vertCols, (float) y / vertRows);
                }
            }
        }

        int j, i;
        int lastColumn = vertCols - 1;
        for (j = 0, i = 0; i < faces.Length ; j++, i += 6) {

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
}
