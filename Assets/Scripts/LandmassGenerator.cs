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

    [Range(0, 1)]
    public float islandPercentage = 0.8f;
    [Range(0, 1)]
    public float coastPercentage = 0.2f;

    public Color snow;
    public Color mountainHigh;
    public Color mountainLow;
    public Color grassHigh;
    public Color grassLow;
    public Color sand;
    public Color seaShallow;
    public Color seaDeep;

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

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float centerToX = x - radiusX;
                float centerToY = y - radiusy;

                float maskFactor = getMaskFactor(centerToX, centerToY);

                float pixel = Mathf.Clamp((pixelIntensity[y * width + x] + 0.5f), 0, 1) * maskFactor;
                Color color = getColorForIntensity(pixel);
                pixels[y * width + x] = color;
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

    private float getMaskFactor(float x, float y)
    {
        float factor = 1;
        float majorCurveA = width / 2f * islandPercentage;
        float majorCurveB = height / 2f * islandPercentage;

        float minorCurveA = majorCurveA * (1 - coastPercentage);
        float minorCurveB = majorCurveB * (1 - coastPercentage);

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

            factor = 1f - Mathf.Clamp((distancePoint - distanceMinor) / (distanceMajor - distanceMinor) , 0, 1);
        }
        else
        {
            factor = 1f - Mathf.Clamp((distancePoint - minorCurveB) / (majorCurveB - minorCurveB), 0, 1);
        }

        return factor;
    }

    private Color getColorForIntensity(float intensity)
    {
        if (intensity >= 0 && intensity < 0.2f)
        {
            return seaDeep;
        }
        else if (intensity >= 0.2f && intensity < 0.4f)
        {
            return seaShallow;
        }
        else if (intensity >= 0.4f && intensity < 0.5f)
        {
            return sand;
        }
        else if (intensity >= 0.5f && intensity < 0.6f)
        {
            return grassLow;
        }
        else if (intensity >= 0.6f && intensity < 0.7f)
        {
            return grassHigh;
        }
        else if (intensity >= 0.7f && intensity < 0.8f)
        {
            return mountainLow;
        }
        else if (intensity >= 0.8f && intensity < 0.9f)
        {
            return mountainHigh;
        }
        else
        {
            return snow;
        }
    }
}
