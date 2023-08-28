using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, uint seed, float scale, int octaves, float persistance, float lacunarity, float2 offset )
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random prng = Random.CreateFromIndex((uint)seed);
        float2[] octaveOffsets = new float2[octaves];
        for(int i = 0;i< octaves; i++) { 
            float offsetX  = prng.NextFloat(-100_000 ,100_000) + offset.x;
            float offsetY = prng.NextFloat(-100_000, 100_000) + offset.y;
            octaveOffsets[i] = new float2 (offsetX, offsetY);
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1; // height
                float frequency = 1; //sideways
                float noiseHeight = 0; //height instead of direct perlin value

                for(int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y  - halfHeight)/ scale * frequency + octaveOffsets[i].y;

                    //perlin value should sometimes be negative to decrease height amplitude
                    //its *2 because perlinValue is on range 0 to 1, after multiplying by 2 its now 0 to 2, subtract 1 to become -1 to 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; //decreases each octave
                    frequency *= lacunarity; //frequency increase each octave
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;   
                }

                noiseMap[x, y] = noiseHeight; //final amplitude height after octaves 

            }
        }
        //normalize noisemap from range 0 to 1, get the min and max value then inverse lerp
        //we want to go through the whole 2d noise map again to normalize each value
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //return value from 0  to 1 depending on how close it is with min and max range
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight,maxNoiseHeight, noiseMap[x,y]);
                //Debug.Log(noiseMap[x,y]+" ");
            }
        }

          return noiseMap;
    }
}
