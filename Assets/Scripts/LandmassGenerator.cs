using UnityEngine;
using System.Collections;

public class LandmassGenerator : MonoBehaviour {

    [Range(1, 100)]
    public float lucanarity = 2;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Range(1, 1024)]
    public int width = 100;
    [Range(1, 1024)]
    public int height = 100;
    [Range(1, 5)]
    public int octaveCount = 3;

    [Range(0, 0.9f)]
    public float islandPercentage = 0.8f;

    [Range(0, 0.5f)]
    public float coastPercentage = 0.2f;

    private float xOrg;
    private float yOrg;

    private Renderer plane_renderer;

	// Use this for initialization
	void Start () {
        plane_renderer = GetComponent<Renderer>();
        xOrg = Random.Range(0, 1024);
        yOrg = Random.Range(0, 1024);
	}
	
	// Update is called once per frame
	void Update () {
        
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
                    float xCoord = xOrg + (float)x / width * Mathf.Pow(lucanarity, octave);
                    float yCoord = yOrg + (float)y / height * Mathf.Pow(lucanarity, octave);
                    float noise = (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * Mathf.Pow(persistance, octave);
                    pixelIntensity[y * width + x] += noise;
                }
            }
        }

        float radiusX = width / 2f;
        float radiusy = height / 2f;
        float tresholdX = islandPercentage * width / 2f;
        float tresholdY = islandPercentage * height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float centerToX = x - radiusX;
                float centerToY = y - radiusy;

                float distanceToCenter = Mathf.Sqrt(centerToX * centerToX + centerToY * centerToY);

                float pixel;
                if (distanceToCenter > tresholdX || distanceToCenter > tresholdY)
                {
                    pixel = 0;
                } else
                {
                    pixel = pixelIntensity[y * width + x] + 0.5f;
                }

                pixels[y * width + x] = new Color(pixel, pixel, pixel);
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        return noiseTex;
    }

    public void generateHeightmap()
    {
        Debug.Log("Rendering again ...");

        xOrg = Random.Range(0, 500);
        yOrg = Random.Range(0, 500);

        Texture2D noiseTexture = createNoiseTexture();
        plane_renderer.material.mainTexture = noiseTexture;
    }
}
