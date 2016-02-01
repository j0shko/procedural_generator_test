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

    public float xOrg;
    public float yOrg;

    private float lastRenderTime;
    public float renderAgainTime = 1f;

    private Terrain terrain;

	// Use this for initialization
	void Start () {
        lastRenderTime = Time.time;
        xOrg = Random.Range(0, 500);
        yOrg = Random.Range(0, 500);
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > lastRenderTime + renderAgainTime)
        {
            //Debug.Log("Rendering again ...");

            terrain = GetComponent<Terrain>();
            float[,] heightmap;
            Texture2D noiseTexture = createNoiseTexture(width, height, lucanarity, persistance, out heightmap);
            terrain.terrainData.SetHeights(0, 0, heightmap);
            terrain.Flush();

            lastRenderTime = Time.time;
        }
    }

    Texture2D createNoiseTexture(int width, int height, float lucanarity, float persistance, out float[,] heightmap)
    {
        Texture2D noiseTex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        heightmap = new float[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = xOrg + (float) x / width * lucanarity;
                float yCoord = yOrg + (float) y / height * lucanarity;
                float noise = Mathf.PerlinNoise(xCoord, yCoord) * persistance;
                pixels[y * width + x] = new Color(noise, noise, noise, noise);
                heightmap[y, x] = noise;
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        return noiseTex;
    }
}
