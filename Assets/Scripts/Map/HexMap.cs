using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexMap : MonoBehaviour {


    private List<Node> path;
    public GameObject Camara;
    public Unit SelectedUnit;
    public GameObject hexPrefab;
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
    public Node start;
    public Map controlMap;
    public int turn = 1;


    public Text turnCounter;

    public BuildingInterface buildingInterface;
    

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
        return controlMap.hexes[x, y];
    }

    internal void ShowBuildingMenu(Building building)
    {
        buildingInterface.OpenLeftInterface(building.x,building.y);
    }

    void Start()
    {
        controlMap = new Map();
        controlMap.sizeX = mapSizeX;
        controlMap.sizeY = mapSizeY;
        controlMap.map = this;
        GenerateMap();
        GenerateMapData();
        controlMap.GenerateNeightboards(tiles);
        GenerateMapVisual();
    }

    public void GenerateMap()
    {
        controlMap.hexes = new Hex[mapSizeX, mapSizeY];
        controlMap.Units = new Unit[mapSizeX, mapSizeY];
        controlMap.Buildings = new Building[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Hex h = new Hex(x, y);
                controlMap.hexes[x, y] = h;
                GameObject go = (GameObject)Instantiate(hexPrefab, h.Position(), Quaternion.identity, this.transform);
                h.GO = go;
                h.ch = go.GetComponentInChildren<ClickableHex>();
                h.ch.hex = h;
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
                Hex h = controlMap.hexes[x, y];

                ClickableHex ch = h.GO.GetComponentInChildren<ClickableHex>();
                h.ch = ch;
                ch.x = x;
                ch.y = y;
                ch.map = this;
                ch.changeHex(tiles[x, y]);

                if (units[x, y] != -1)
                {
                    controlMap.CreateUnit(units[x, y], x, y, 0);
                }
                if (buildings[x, y] != -1)
                {
                    controlMap.CreateBuilding(buildings[x,y], x, y, buildings[x, y]);
                }

            }
        }
    }
    

    public void ResetTurn()
    {
        turn++;
        turnCounter.text = "Turn: " + turn;
        buildingInterface.Produce(turn);
        changePath(null);
        if (SelectedUnit != null) SelectedUnit.cu.changeHalo(false);
        SelectedUnit = null;
        Hex h = null;
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (controlMap.Buildings[x, y] != null)
                {
                    if (buildings[x, y] == 0)
                    {
                        h = controlMap.hexes[x, y];
                    }
                    else if (controlMap.Buildings[x, y].owner == 1)
                    {
                        controlMap.Buildings[x, y].RandomSpawn(turn);
                    }
                }
                if (controlMap.Units[x, y] != null)
                {
                    controlMap.Units[x, y].ResetTurn();
                }
            }
        }
        checkGameOver(h);
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (controlMap.Units[x, y] != null)
                {
                    Unit u = controlMap.Units[x, y];
                    if (u.owner == 1 && u.turnMovement> 0)
                    {
                        MoveUnitCloser(h.x, h.y, u);
                        if (u.turnAttacks > 0)
                        {
                            AttackClose();
                        }
                    }
                }
                
            }
        }


        togglePath(false);
        SelectedUnit = null;
    }

    private void checkGameOver(Hex h)
    {
        List<Node> options = controlMap.nodes[h.x, h.y].Neightbours;
        foreach (Node i in options)
        {
            if (controlMap.Units[i.x, i.y] !=  null && controlMap.Units[i.x, i.y].owner == 2)
            {
                SceneManager.LoadScene(2);
            }
        }
    }

    private void AttackClose()
    {
        List<Node> options = controlMap.nodes[SelectedUnit.x, SelectedUnit.y].Neightbours;
        foreach (Node i in options)
        {
            if (SelectedUnit.turnAttacks > 0)
            {
                if (controlMap.Units[i.x, i.y] != null)
                {
                    if (controlMap.Units[i.x, i.y].owner != controlMap.Units[SelectedUnit.x, SelectedUnit.y].owner)
                    {
                        controlMap.AttackUnit(SelectedUnit, controlMap.Units[i.x, i.y]);
                    }
                }
                
            }
            else break;
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

    private List<Node> getNodesRadius(int x, int y, int tries)
    {
        Node start = controlMap.nodes[x, y];
        List<Node> startList = start.Neightbours;
        List<Node> result = new List<Node>();
        foreach (Node i in startList)
        {
            List<Node> temp = i.NeightboursLand;
            foreach (Node k in temp)
            {
                if (!result.Contains(k))
                {
                    result.Add(k);
                }
            }
        }
        return result;

    }


    public void MoveUnit(int endx, int endy)
    {
        if (path != null)
        {
            Node last = controlMap.nodes[endx, endy];
            if (path.Contains(last))
            {
                Hex actual = controlMap.hexes[endx, endy];
                int maxMovement = SelectedUnit.turnMovement;
                int counter = 0;
                foreach (Node i in path)
                {
                    actual = controlMap.hexes[i.x, i.y];
                    if (i.costOfMovement != -1)
                    {
                        counter += i.costOfMovement;
                    }
                    if (actual.x == last.x && actual.y == last.y) break;
                    if (counter > maxMovement) break;
                }
                togglePath(false);
                SelectedUnit.turnMovement -= counter;
                controlMap.MoveUnit(SelectedUnit, actual.x,actual.y);
                path = null;
                return;
            }
        }
        togglePath(false);
        if (SelectedUnit != null)
        {
            path = PathFindHex(endx, endy);
        }
        if (SelectedUnit != null && SelectedUnit.owner == 0)
        {
            togglePath(true);
        }
    }

    internal void MoveUnitCloser(int x, int y, Unit unit)
    {
        List<Node> posibilities =null;
        List<Node> final= null;
        int cost;
        int min = 1000;
        if (SelectedUnit != null) SelectedUnit.cu.changeHalo(false);
        SelectedUnit = unit;
        int tries = 1;
        while(tries < 3)
        {
            if (tries == 1) posibilities = controlMap.nodes[x, y].NeightboursLand;
            else posibilities = getNodesRadius(x, y, tries);
            foreach (Node n in posibilities)
            {
                List<Node> option = PathFindHex(n.x, n.y);
                if (option == null) cost = 1000;
                else cost = pathCost(option);
                if (min > cost)
                {
                    min = cost;
                    final = option;
                }
            }
            if (final != null) break;
            tries++;
        }
        if (final == null) return;
        path = final;
        Node last = path[path.Count - 1];
        MoveUnit(last.x, last.y);
        return;

    }

    private int pathCost(List<Node> option)
    {
        int counter = 0;
        foreach (Node i in option)
        {
            if (i.costOfMovement != -1)
            {
                counter += i.costOfMovement;
            }
        }
        return counter;
    }
    
    public void changePath(List<Node> newPath)
    {
        togglePath(false);
        path = newPath;
        togglePath(true);
    }

    public void togglePath(Boolean option)
    {
        if (path == null) return;
        int maxMovement = 100;
        if (option)
        {
            maxMovement = SelectedUnit.turnMovement;
        }
        int counter = 0;
        foreach (Node i in path)
        {
            ClickableHex ct = controlMap.hexes[i.x,i.y].ch;
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

        Node start = controlMap.nodes[SelectedUnit.x, SelectedUnit.y];
        Node finish = controlMap.nodes[endx,endy];

        if (finish.costOfMovement == -1)
        {
            return null;
        }

        foreach ( Node n in controlMap.nodes)
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

    public bool SpawnUnit(int x, int y, int type)
    {
        List<Node> neightbours = controlMap.nodes[x, y].NeightboursLand;
        foreach (Node i in neightbours)
        {
            if (controlMap.Units[i.x,i.y] == null)
            {
                units[i.x, i.y] = type;
                int owner;
                if (units[i.x, i.y] < 2)
                {
                    owner = 0;
                }
                else
                {
                    owner = 1;
                }

                controlMap.CreateUnit(type, i.x, i.y, owner);

                return true;
            }
        }
        return false;
    }

}

public class Unit {

    public int x = 0;
    public int y = 0;
    public int owner = 0;
    public Hex hex;
    public ClickableUnit cu;
    public GameObject GO;
    public int hp = 2;
    public int damage = 1;
    public int maxMovement = 1;
    public int turnMovement = 1;
    public int maxAttacks = 1;
    public int turnAttacks = 1;

    public void SetClickableHex(Hex newHex)
    {
        hex = newHex;
        hex.ch.owner = owner;
        hex.ch.currentUnit = this;
        x = hex.x;
        y = hex.y;
        cu.map.controlMap.nodes[x, y].costOfMovement = -1;
        ChangeTurnMovement(0);

    }

    public void ResetTurn()
    {
        turnMovement = maxMovement;
        turnAttacks = maxAttacks;
        ChangeTurnMovement(0);
    }

    public void ChangeHP(int change)
    {
        hp += change;
        cu.HP.text = hp.ToString();
    }

    public void ChangeTurnMovement(int change)
    {
        turnMovement += change;
        if (turnMovement < -1) turnMovement = -1;
        if (owner > 0)
        {
            cu.Movement.text = (maxMovement + 1).ToString();
        }
        else
        {
            cu.Movement.text = (turnMovement + 1).ToString();
        }
    }

    public void SetDamage(int change)
    {
        damage = change;
        cu.Damage.text = damage.ToString();
    }

    public void Dead()
    {

        GameObject.Destroy(GO);


    }
}

public class Building
{
    public int x = 0;
    public int y = 0;
    public int owner = 0;
    public Hex hex;
    public ClickableBuilding cb;
    public GameObject GO;
    public int hp = 3;
    public int spawnRate = 2;


    public void SetClickableHex(Hex newHex)
    {
        hex = newHex;
        hex.ch.owner = owner;
        hex.ch.currentBuilding = this;
        x = hex.x;
        y = hex.y;
        cb.map.controlMap.nodes[x, y].costOfMovement = -1;
        ChangeHP(0);

    }

    public void ResetTurn()
    {
    }
    
    public void ChangeHP(int change)
    {
        hp += change;
        if (hp < -1) hp = -1;
        if (owner > 0)
        {
            cb.HP.text = "∞";
        }
        else
        {
            cb.HP.text = hp.ToString();
        }
    }

    public void Dead()
    {

        GameObject.Destroy(GO);


    }

    internal void RandomSpawn(int turn)
    {
        int chance = Random.Range(0, 100);
        if (chance < spawnRate)
        {
            hex.ch.map.SpawnUnit(x, y, 2);
            spawnRate = 1;
        }
        else
        {
            if (turn > 20) spawnRate += 2;
            else spawnRate += 1;
        }
    }
}

public class Map
{
    public int sizeX;
    public int sizeY;

    public HexMap map;
    public Hex[,] hexes;
    public Node[,] nodes;
    public Unit[,] Units;
    public Building[,] Buildings;


    public void CreateUnit(int type, int x, int y, int owner)
    {
        Unit u = new Unit();
        u.owner = owner;
        hexes[x, y].ch.createUnit(u);
        u.cu.unit = u;
        hexes[x, y].ch.changeUnit(type);
        nodes[x, y].costOfMovement = -1;
        Units[x, y] = u;
        u.SetDamage(1);
        u.ChangeTurnMovement(0);
        u.ChangeHP(0);

    }

    public void CreateBuilding(int type, int x, int y, int owner)
    {
        Building b = new Building();
        if (type == 1)
        {
            owner = 1;
        }
        else owner = 0;
        b.owner = owner;
        hexes[x, y].ch.createBuilding(b);
        b.cb.building = b;
        nodes[x, y].costOfMovement = -1;
        Buildings[x, y] = b;
        b.ChangeHP(0);

    }

    public void AttackBuilding(Unit attacker, Building defender)
    {
        attacker.turnAttacks--;
        if (attacker.damage >= defender.hp)
        {
            Units[defender.x, defender.y] = null;
            defender.Dead();
            nodes[defender.x, defender.y].costOfMovement = nodes[defender.x, defender.y].normalCostOfMovement;
            defender.hex.ch.RemoveBuilding();

        }
        else
        {
            defender.ChangeHP(-attacker.damage);
        }


    }

    public void MoveUnit(Unit unit, int x, int y)
    {
        Hex oldHex = hexes[unit.x, unit.y];
        Units[unit.x, unit.y] = null;
        Hex newHex = hexes[x, y];
        Units[x, y] = unit;
        unit.GO.transform.parent = newHex.GO.transform;
        unit.GO.transform.position = newHex.Position();
        unit.SetClickableHex(newHex);
        oldHex.RemoveUnit();
    }

    public void AttackUnit (Unit attacker, Unit defender)
    {
        attacker.turnAttacks--;
        if (attacker.damage >= defender.hp)
        {
            Units[defender.x, defender.y] = null;
            defender.Dead();
            nodes[defender.x, defender.y].costOfMovement = nodes[defender.x, defender.y].normalCostOfMovement;
            defender.hex.ch.RemoveUnit();
        }
        else
        {
            defender.ChangeHP(-attacker.damage);
        }


    }

    public Unit NextUnit(Unit center)
    {
        return new Unit();
    }

    public List<Unit> AllUnit(Unit center)
    {
        return new List<Unit>();
    }

    public bool CheckStartingCity(int x, int y)
    {
        return true;
    }

    public void SpawnUnit(int x, int y, int type)
    {

    }

    public List<Hex> AllFreeSpaces(int x, int y, int radius)
    {
        return new List<Hex>();
    }

    public void GenerateNeightboards(int[,] tiles)
    {
        nodes = new Node[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Node n = new Node();
                nodes[x, y] = n;

            }
        }
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                List<Node> neigh = new List<Node>();
                if (x < sizeX - 1)
                {
                    neigh.Add(nodes[x + 1, y]);
                }
                if (y < sizeY - 1)
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
                if (x > 0 && y < sizeY - 1)
                {
                    neigh.Add(nodes[x - 1, y + 1]);
                }
                if (y > 0 && x < sizeY - 1)
                {
                    neigh.Add(nodes[x + 1, y - 1]);
                }

                nodes[x, y].Neightbours = neigh;
                nodes[x, y].costOfMovement = map.hexMovementCost[tiles[x, y]];
                nodes[x, y].normalCostOfMovement = map.hexMovementCost[tiles[x, y]];
                nodes[x, y].x = x;
                nodes[x, y].y = y;

            }
        }
    }



}