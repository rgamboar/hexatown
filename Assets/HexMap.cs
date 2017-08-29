using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class HexMap : MonoBehaviour {

    public GameObject SelectedUnit;
    public GameObject hexPrefab;
    public GameObject buildingMaster;
    public GameObject unitMaster;
    public Sprite[] hexSprites;
    public int[] hexMovementCost;
    public Sprite[] buildingSprites;
    public Sprite[] unitsSprites;
    public int[,] tiles;
    public GameObject buildingPrefab;
    public int[,] buildings;
    public GameObject unitPrefab;
    public int[,] units;
    public bool ShowCordinates = false;
    public Hex[,] hexes;
    public Node[,] nodes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;

    public int mapSizeX = 25;
    public int mapSizeY = 20;


    private float offsetX, offsetY;

    public Hex getHex(int x, int y)
    {
        if (x < 0 || x >= mapSizeX)
        {
            return null;
        }
        if (y < 0 || y >= mapSizeY)
        {
            return null;
        }
        return hexes[x, y];
    }

    void Start()
    {
        GenerateMap();
        GenerateMapData();
        GenerateMapVisual();
        GenerateNeightboards();
    }

    public void GenerateMap()
    {
        hexes = new Hex[mapSizeX, mapSizeY];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Hex h = new Hex(x, y);
                hexes[x, y] = h;
                GameObject go = (GameObject)Instantiate(hexPrefab, h.Position(), Quaternion.identity, this.transform);
                ClickableHex ct = go.GetComponentInChildren<ClickableHex>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
                hexToGameObjectMap[h] = go;
                if (ShowCordinates)
                {
                    go.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", h.Q, h.R);
                };
            }

        }
    }

    virtual public void GenerateMapData()
    {
        tiles = new int[mapSizeX, mapSizeY];
        units = new int[mapSizeX, mapSizeY];
        buildings = new int[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = Random.Range(0, 4);
                units[x, y] = -1;
                buildings[x, y] = -1;
            }
        }
        tiles[1, 1] = 1;
        tiles[2, 1] = 1;
        tiles[5, 1] = 1;
        tiles[7, 2] = 1;
        units[1, 1] = 0;
        units[2, 1] = 0;
        units[5, 1] = 0;
        units[7, 2] = 0;
    }

    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Hex h = hexes[x, y];
                GameObject go = hexToGameObjectMap[h];
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.sprite = hexSprites[tiles[x, y]];

                if (units[x, y] != -1)
                {
                    GameObject goUnit = (GameObject)Instantiate(unitPrefab, h.Position(), Quaternion.identity, unitMaster.transform);
                    ClickableUnit cu = goUnit.GetComponentInChildren<ClickableUnit>();
                    cu.map = this;
                    cu.x = x;
                    cu.y = y;
                    SpriteRenderer srUnit = goUnit.GetComponentInChildren<SpriteRenderer>();
                    srUnit.sprite = unitsSprites[units[x, y]];
                }
                if (buildings[x, y] != -1)
                {
                    GameObject goBuilding = (GameObject)Instantiate(buildingPrefab, h.Position(), Quaternion.identity, buildingMaster.transform);
                    SpriteRenderer srBuilding = goBuilding.GetComponentInChildren<SpriteRenderer>();
                    srBuilding.sprite = buildingSprites[buildings[x, y]];
                    srBuilding.sortingOrder = 1;
                }

            }
        }
    }


    public Hex[] GetHexesRadius(Hex center, int radius)
    {
        List<Hex> result = new List<Hex>();

        for (int x= -radius; x < radius-1; x++)
        {
            for (int y = Mathf.Max(-radius+1, -x-radius); y < Mathf.Min(radius, -x + radius-1); y++)
            {
                result.Add(getHex(center.Q + x, center.R + y));
            }
        }
        return result.ToArray();
    }

    public void MoveSelectedUnit(int x, int y)
    {
        Hex h = new Hex(x, y);
        SelectedUnit.transform.position = h.Position();
    }

    public void MoveUnit(int endx,int endy)
    {
        SelectedUnit.GetComponentInChildren<ClickableUnit>().path = PathFindHex(endx, endy);
    }

    public List<Node> PathFindHex(int endx, int endy)
    {
        List<Node> unvisited = new List<Node>();
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        Node start = nodes[SelectedUnit.GetComponentInChildren<ClickableUnit>().x, SelectedUnit.GetComponentInChildren<ClickableUnit>().y];
        Node finish = nodes[endx,endy];

        if (finish.costOfMovement == -1)
        {
            return null;
        }

        foreach ( Node n in nodes)
        {
            if (n.costOfMovement != -1)
            {
                previous[n] = null;
                unvisited.Add(n);
                if (n == start)
                {
                    dist[n] = 0;
                }
                else
                {
                    dist[n] = Mathf.Infinity;
                }
            }
        }

        while (unvisited.Count > 0)
        {
            Node u = null;
            foreach (Node min in unvisited)
            {
                if (u==null || dist[min] < dist[u])
                {
                    u = min;
                }
            }

            if (u == finish){
                break;
            }
            unvisited.Remove(u);
            foreach (Node n in u.NeightboursLand)
            {
                float altDist = dist[u] + n.costOfMovement;
                if (altDist < dist[n])
                {
                    dist[n] = altDist;
                    previous[n] = u;
                }
            }
        }

        if (previous[finish] == null)
        {
            return null;
        }

        List<Node> path = new List<Node>();

        Node current = finish;

        while (current != null)
        {
            path.Add(current);
            current = previous[current];
        }
        path.Reverse();
        return path;

    }


    private void GenerateNeightboards()
    {
        nodes = new Node[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Node n = new Node();
                nodes[x, y] = n;

            }
        }
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                List<Node> neigh = new List<Node>();
                if (x < mapSizeX - 1)
                {
                    neigh.Add(nodes[x + 1, y]);
                }
                if (y < mapSizeY - 1)
                {
                    neigh.Add(nodes[x, y + 1]);
                }
                if (x > 0)
                {
                    neigh.Add(nodes[x - 1, y]);
                }
                if (y > 0)
                {
                    neigh.Add(nodes[x, y - 1]);
                }
                if (x > 0 && y < mapSizeY - 1)
                {
                    neigh.Add(nodes[x - 1, y + 1]);
                }
                if (y > 0 && x < mapSizeX - 1)
                {
                    neigh.Add(nodes[x + 1, y - 1]);
                }

                nodes[x, y].Neightbours = neigh;
                nodes[x, y].costOfMovement = hexMovementCost[tiles[x, y]];
                nodes[x, y].x = x;
                nodes[x, y].y = y;

            }
        }
    }
}
