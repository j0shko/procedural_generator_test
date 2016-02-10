using UnityEngine;
using System.Collections;

public abstract class MapDisplay : MonoBehaviour {

    public abstract void DisplayMap(float[,] heightMap);
}
