using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexMap_Continent : HexMap
{

    public void NoneUnit()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                units[x, y] = -1;
                buildings[x, y] = -1;
            }
        }
    }

    private int ElevationToOneTile(Hex h)
    {
        if (h == null)
        {
            return -1;
        }
        float elevation = h.Elevation;

        if (elevation > 1f)
        {
            return 4;
        }
        else if (elevation > 0.8f)
        {
            return 3;
        }
        else if (elevation > 0.2f)
        {
            return 2;
        }
        else if (elevation > 0f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void ElevationToTiles()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Hex h = getHex(x, y);
                tiles[x, y] = ElevationToOneTile(h);
            }
        }
    }

    override public void GenerateMapData()
    {
        tiles = new int[mapSizeX, mapSizeY];
        units = new int[mapSizeX, mapSizeY];
        buildings = new int[mapSizeX, mapSizeY];
        NoneUnit();


        int numContinent = 1;
        int continentalSpacing = 20;

        for (int c = 1; c <= numContinent; c++)
        {
            int numSplats = Random.Range(10, 15);
            for (int i = 0; i < numSplats; i++)
            {
                int radius = Random.Range(4, 12);
                int y = Random.Range(radius, mapSizeY - radius);
                int x = Random.Range(0, 10) - y / 2 + (c * continentalSpacing);

                ElevateArea(x, y, radius);
            }
        }

        float noiseRandom = Random.Range(0.5f, 1f);
        float noiseIncrement = 2f;

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Hex h = getHex(x, y);
                float n1 = Mathf.PerlinNoise((x * 2.5f) + noiseRandom, (y * 2.5f) + noiseRandom);
                float n2 = Mathf.PerlinNoise((x * 4) + (2 * noiseRandom), (y * 4) + (2 * noiseRandom));
                float n3 = ((n1 + n2) / 2f - 0.5f);
                h.Elevation += n3 * noiseIncrement;
            }
        }

        ElevationToTiles();
        PutStartingPoint(2,10,6);
        PutCity(2, 7, 10, 10);
        ElevationToTiles();
        //putTestUnit();
    }

    private void putTestUnit()
    {
        if (getHex(start.x, start.y + 1).Elevation > 0)
        {
            units[start.x, start.y + 1] = 0;
        }
        if (getHex(start.x, start.y - 1).Elevation > 0)
        {
            units[start.x, start.y - 1] = 0;
        }
        if (getHex(start.x-1, start.y + 1).Elevation > 0)
        {
            units[start.x-1, start.y + 1] = 0;
        }

    }

    void PutCity(int startTileType, int radius, int totalGoodTiles, int quantity = 1000)
    {
        int x = radius;
        int y = radius;

        int cities = 0;
        while (cities < quantity)
        {
            Hex h = getHex(x, y);
            if (ElevationToOneTile(h) == startTileType)
            {
                Hex[] areaHexes = GetHexesRadius(h, radius);
                int counter = 0;
                foreach (Hex hex in areaHexes)
                {
                    if (ElevationToOneTile(hex) == startTileType)
                    {
                        counter += 1;
                    }
                    if (buildings[hex.Q, hex.R] == 0 || buildings[hex.Q, hex.R] == 1)
                    {
                        counter = 0;
                        break;
                    }

                }
                if (counter >= totalGoodTiles)
                {
                    buildings[x, y] = 1;
                    cities++;
                    RaiseStartingCity(x, y, true);
                    //  return;
                }
            }
            x++;
            if (x > mapSizeX - radius)
            {
                x = radius;
                y++;
                if (y > mapSizeY - radius)
                {
                    return;
                }

            }

        }
    }

    void PutStartingPoint(int startTileType, int radius, int totalGoodTiles, int quantity = 1)
    {
        int x = radius;
        int y = radius;

        int cities = 0;
        while (cities < quantity)
        {
            Hex h = getHex(x, y);
            if (ElevationToOneTile(h) == startTileType)
            {
                Hex[] areaHexes = GetHexesRadius(h, radius);
                int counter = 0;
                foreach (Hex hex in areaHexes)
                {
                    if (ElevationToOneTile(hex) == startTileType)
                    {
                        counter += 1;
                    }

                }
                if (counter >= totalGoodTiles)
                {
                    buildings[x, y] = 0;
                    start = new Node();
                    start.x = x;
                    start.y = y;
                    RaiseStartingCity(x, y);
                    return;
                }
            }
            x++;
            if (x > mapSizeX - radius)
            {
                x = radius;
                y++;
                if (y > mapSizeY - radius)
                {
                    return;
                }

            }

        }
    }

    private void RaiseStartingCity(int x, int y, bool option = false)
    {
        if (x < mapSizeX - 1)
        {
            Hex h = getHex(x + 1, y);
            RaiseTile(h, option);
        }
        if (y < mapSizeY - 1)
        {
            Hex h = getHex(x, y + 1);
            RaiseTile(h, option);
        }
        if (x > 0)
        {
            Hex h = getHex(x - 1, y);
            RaiseTile(h, option);
        }
        if (y > 0)
        {
            Hex h = getHex(x, y - 1);
            RaiseTile(h, option);
        }
        if (x > 0 && y < mapSizeY - 1)
        {
            Hex h = getHex(x - 1, y + 1);
            RaiseTile(h, option);
        }
        if (y > 0 && x < mapSizeY - 1)
        {
            Hex h = getHex(x + 1, y - 1);
            RaiseTile(h, option);
        }
    }

    private void RaiseTile(Hex h, bool option= false)
    {
        if (option)
        {
            if (h.Elevation < 0f)
            {
                h.Elevation += 0.15f;
            }
            return;
        }

        while (true)
        {
            if (h.Elevation < 0f)
            {
                h.Elevation += 0.25f;
            }
            else break;
        }
    }

    void ElevateArea(int x, int y, int radius)
    {
        Hex center = getHex(x, y);
        if (center == null)
        {
            return;
        }
        Hex[] areaHexes = GetHexesRadius(center, radius);
        foreach( Hex h in areaHexes)
        {
            if (h != null)
            {
                if (h.Elevation <= -0.5f)
                {
                    h.Elevation = 0;
                }
                h.Elevation = 1f * Mathf.Lerp(1f, 0.1f, Hex.Distance(center, h) / radius);
            }

        }
    }

}



