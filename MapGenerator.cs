using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //
    public enum DrawMode { NoiseMap, ColourMap};
    public DrawMode drawMode;

    public int mapHeight;
    public int mapWidth;
    public float noiseScale;

    public uint seed;
    public float2 offset;

    public TerrainType[] regions;

    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;


    public bool autoUpdate;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistance, lacunarity, offset);

        //do the coloring
        Color[] colourMap = new Color[mapWidth * mapHeight];
        for(int y=0; y < mapHeight; y++)
        {
            for(int x=0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                //loop through the regions
                for(int i=0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        //store the color the color array
                        colourMap[y * mapWidth + x] = regions[i].color;
                        break;//found the exact 
                    }
                }
            }
        }

        //find the gameobject with the mapdisplay
        MapDisplay display = FindFirstObjectByType<MapDisplay>();

        //what drawmode
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap,mapWidth, mapHeight));
        }
        //
    }

    private void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
}

//show up in inspector
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}