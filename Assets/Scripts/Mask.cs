using UnityEngine;
using System.Collections;

public class Mask
{

    public static void ApplyIslandMask(float[,] map, float islandPercentage, float coastPercentage)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        float radiusX = width / 2f;
        float radiusY = height / 2f;

        float majorCurveA = radiusX * islandPercentage;
        float majorCurveB = radiusY * islandPercentage;

        float coastSize = ((majorCurveA + majorCurveB) * coastPercentage) / 2;

        float minorCurveA = majorCurveA - coastSize;
        float minorCurveB = majorCurveB - coastSize;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float centerToX = x - radiusX;
                float centerToY = y - radiusY;

                float distancePoint = Mathf.Sqrt(centerToX * centerToX + centerToY * centerToY);
                float factor = 1;
                float k;

                if (centerToX != 0)
                {
                    k = centerToY / centerToX;

                    float x1 = 1 / Mathf.Pow(Mathf.Pow(majorCurveA, -4f) + Mathf.Pow(k / majorCurveB, 4), 1 / 4f);
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

                map[x, y] *= factor;
            }
        }
    }
}
