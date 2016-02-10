using UnityEngine;
using System.Collections;

public class Noise {

    private static float MINUMUM_SCALE = 0.0001f;

	public static float[,] GenerateNoise(int width, int height, float scale, float lucanarity, float persistance,
        float octaveCount, int seed)
    {
        float[,] noiseMap = new float[width, height];

        if (scale <= 0)
        {
            scale = MINUMUM_SCALE;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = 0;
                for (int octave = 0; octave < octaveCount; octave++)
                {
                    float xCoord = seed + (x / scale) * Mathf.Pow(lucanarity, octave);
                    float yCoord = seed + (y / scale) * Mathf.Pow(lucanarity, octave);

                    noiseValue += (Mathf.Clamp01(Mathf.PerlinNoise(xCoord, yCoord)) - 0.5f) * Mathf.Pow(persistance, octave);
                }

                noiseMap[x, y] = noiseValue + 0.5f;
            }
        }

        return noiseMap;
    }
}
