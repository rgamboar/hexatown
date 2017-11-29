using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class HexMap : MonoBehaviour {


    public List<Node> path;
    public GameObject Camara;
    public GameObject SelectedUnit;
    public ClickableUnit SUcu;
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
    public Node start;
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
        GenerateNeightboards();
        GenerateMapVisual();
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

                ClickableHex ct = hexToGameObjectMap[h].GetComponentInChildren<ClickableHex>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;

                ct.changeHex(tiles[x, y]);

                if (units[x, y] != -1)
                {
                    ct.createUnit();
                    ct.changeUnit(units[x, y]);
                    nodes[x, y].costOfMovement = -1;
                }
                if (buildings[x, y] != -1)
                {
                    ct.createBuilding(buildings[x, y], h);
                    nodes[x, y].costOfMovement = -1;
                }

            }
        }
    }

    public void ResetTurn()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (units[x,y] != -1)
                {
                    ClickableUnit cu = hexToGameObjectMap[hexes[x,y]].GetComponentInChildren<ClickableUnit>();
                    cu.turnMovement = cu.maxMovement;
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
        GameObject hex= hexToGameObjectMap[hexes[x, y]];
        SelectedUnit.transform.parent = hex.transform;
        SelectedUnit.transform.position = hexes[x, y].Position();
        ClickableUnit cu = hex.GetComponentInChildren<ClickableUnit>();

        ChangeUnitHex(cu.x, cu.y);
        ChangeUnitHex(x, y, SelectedUnit);
    }

    public void ChangeUnitHex(int x, int y, GameObject unit=null)
    {
        GameObject hex = hexToGameObjectMap[hexes[x, y]];
        ClickableHex ch = hex.GetComponentInChildren<ClickableHex>();
        ch.gameObjectUnit = unit;
        if (unit == null)
        {
            units[x, y] = -1;
            nodes[x, y].costOfMovement = nodes[x,y].normalCostOfMovement;
        }
        else
        {
            ClickableUnit cu = hex.GetComponentInChildren<ClickableUnit>();
            units[x, y] = 1;
            cu.x = x;
            cu.y = y;
            nodes[x, y].costOfMovement = -1;
        }
    }

    public void MoveUnit(int endx, int endy)
    {
        if (path != null)
        {
            Node last = nodes[endx, endy];
            if (path.Contains(last))
            {
                Hex actual = hexes[endx, endy];
                int maxMovement = SUcu.turnMovement;
                int counter = 0;
                foreach (Node i in path)
                {
                    actual = hexes[i.x, i.y];
                    if (i.costOfMovement != -1)
                    {
                        counter += i.costOfMovement;
                    }
                    if (actual.X() == last.x && actual.Y() == last.y) break;
                    if (counter > maxMovement) break;
                }
                togglePath(false);
                ChangeMPSelectedUnit(-counter);
                MoveSelectedUnit(actual.X(), actual.Y());
                path = null;
                return;
            }
        }
        togglePath(false);
        if (SelectedUnit != null)
        {
            path = PathFindHex(endx, endy);
        }
        togglePath(true);
    }

    private void ChangeMPSelectedUnit(int counter)
    {
        SUcu.turnMovement += counter;
    }

    public void togglePath(Boolean option)
    {
        if (path == null) return;
        int maxMovement = 100;
        if (option)
        {
            maxMovement = SUcu.turnMovement;
        }
        int counter = 0;
        foreach (Node i in path)
        {
            ClickableHex ct = hexToGameObjectMap[hexes[i.x, i.y]].GetComponentInChildren<ClickableHex>();
            if (i.costOfMovement > 0) ct.showPath(option);
            if (i.costOfMovement != -1)
            {
                counter += i.costOfMovement;
            }
            if (counter > maxMovement) return;
        }
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
                dist[n] = Mathf.Infinity;
            }
            else if (n == start)
            {
                previous[n] = null;
                unvisited.Add(n);
                dist[n] = 0;
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

    public void SpawnUnit(int x, int y, int type)
    {
        List<Node> neightbours = nodes[x, y].NeightboursLand;
        foreach (Node i in neightbours)
        {
            if (units[i.x, i.y] == -1)
            {
                units[i.x, i.y] = type;
                ClickableHex ct = hexToGameObjectMap[hexes[i.x, i.y]].GetComponentInChildren<ClickableHex>();
                ct.createUnit();
                ct.changeUnit(units[i.x, i.y]);
                nodes[i.x, i.y].costOfMovement = -1;
                break;
            }
        }
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
                nodes[x, y].normalCostOfMovement = hexMovementCost[tiles[x, y]];
                nodes[x, y].x = x;
                nodes[x, y].y = y;

            }
        }
    }
}
