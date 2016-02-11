using UnityEngine;
using System.Collections;

public class Noise {

    private static float MINUMUM_SCALE = 0.0001f;

	public static float[,] GenerateNoise(int width, int height, float scale, float lacunarity, float persistence, int octaveCount, int seed, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaveCount]; 
        for (int i = 0; i < octaveCount; i++)
        {
            float offsetX = rng.Next(-10000, 10000) + offset.x;
            float offsetY = rng.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = MINUMUM_SCALE;
        }

        float maxNoiseValue = float.MinValue;
        float minNoiseValue = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = 0;
                float frequency = 1;
                float amplitude = 1;

                for (int octave = 0; octave < octaveCount; octave++)
                {
                    float xCoord = (x - halfWidth) / scale * frequency + octaveOffsets[octave].x;
                    float yCoord = (y - halfHeight) / scale * frequency + octaveOffsets[octave].y;

                    // Perlin noise in interval [-1, 1]
                    float perlinNoise = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseValue += perlinNoise * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity; 
                }

                if (noiseValue > maxNoiseValue)
                {
                    maxNoiseValue = noiseValue;
                } else if (noiseValue < minNoiseValue)
                {
                    minNoiseValue = noiseValue;
                }

                noiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noiseMap[x, y]);
            }
        }
        
        return noiseMap;
    }
}
