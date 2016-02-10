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

    [Range(2f, 100f)]
    public float noiseScale = 2f;

    public bool autoUpdate;


    public bool randomSeed;
    [Range(0, 4096)]
    public int seed;

    public MapDisplay displayer;

    public enum MaskType
    { 
        NoMask,
        Island
    };

    public MaskType maskType;

    [Range(0, 1)]
    public float islandPercentage = 0.8f;
    [Range(0, 1)]
    public float coastPercentage = 0.2f;

    // Use this for initialization
    void Start () {
        //GenerateHeightmap();
	}

    public void GenerateMap()
    {
        if (randomSeed)
        {
            seed = Random.Range(0, 4096);
        }

        float[,] noiseMap = Noise.GenerateNoise(width, height, noiseScale, lucanarity, persistance, octaveCount, seed);

        ApplyMask(noiseMap);

        displayer.DisplayMap(noiseMap);
    }

    private void ApplyMask(float[,] map)
    {
        if (maskType == MaskType.Island)
        {
            Mask.ApplyIslandMask(map, islandPercentage, coastPercentage);
        } 
    }
}
