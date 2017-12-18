using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableHex : MonoBehaviour {

    public int x, y;
    public GameObject parent;
    public HexMap map;
    public List<Node> path;
    public GameObject gameObjectHex;
    public GameObject gameObjectBuilding;
    public Sprite[] spriteHex;
    public Sprite[] spriteUnit;
    public Color[] spriteColor;
    public GameObject unitPrefab;
    public Component halo;
    public GameObject buildingPrefab;
    public Sprite[] buildingSprites;
    public Unit currentUnit;
    public Building currentBuilding;
    public Hex hex;

    public int owner;

    float t0;
    private void Start()
    {
        t0 = 0f;
        parent = transform.parent.gameObject;
        path = null;
        halo = gameObjectHex.GetComponent("Halo");
    }
  
    void OnMouseDown()
    {
        t0 = Time.time;
    }
    void OnMouseUp()
    {
        if ((Time.time - t0) < 0.2f)
        {
            if (owner == 0)
            {
                if(currentBuilding != null)
                {
                    map.ShowBuildingMenu(currentBuilding); 
                }
                if (map.SelectedUnit != currentUnit && currentUnit != null)
                {
                    if (map.SelectedUnit != null)
                    {
                        map.SelectedUnit.cu.changeHalo(false);
                    }
                    map.SelectedUnit = currentUnit;
                    changeHalo(true);
                }

            }
            else if (owner == -1)
            {
                map.MoveUnit(x, y);
            }
            if (currentUnit != null && map.SelectedUnit != null && currentUnit.owner != map.SelectedUnit.owner)
            {
                if (map.SelectedUnit.turnAttacks > 0)
                {
                    if (isNeightboard())
                    {
                        map.controlMap.AttackUnit(map.SelectedUnit, currentUnit);
                    }
                }
            }
           //if (gameObjectBuilding != null)
            //{
             //   int unit;
              //  if (owner == 1) unit = 2;
               // else unit = UnityEngine.Random.Range(0, 2);
                //map.SpawnUnit(x, y, unit);
           // }

        }
    }

    private bool isNeightboard()
    {
        List<Node> options = map.controlMap.nodes[map.SelectedUnit.x, map.SelectedUnit.y].Neightbours;
        foreach (Node i in options)
        {
            if (i.x == x && i.y == y) return true;
        }
        return false;

    }

    private void Update()
    {
    }

    public Vector3 Line(Node current)
    {
        return new Vector3(2 * (current.x + current.y / 2f), Mathf.Sqrt(3) * current.y, 10);
    }

    public void changeHalo(bool option)
    {
        currentUnit.cu.changeHalo(option);
    }

    public void showPath(bool option)
    {
        halo.GetType().GetProperty("enabled").SetValue(halo, option, null);
    }

    public void changeHex(int i)
    {
        SpriteRenderer sr = gameObjectHex.GetComponent<SpriteRenderer>();
        sr.sprite = spriteHex[0];
        sr.color = spriteColor[i];
    }
    public void createUnit(Unit newUnit)
    {
        GameObject gameObjectUnit = (GameObject)Instantiate(unitPrefab, transform.position, Quaternion.identity, parent.transform);
        currentUnit = newUnit;
        currentUnit.GO = gameObjectUnit;
        ClickableUnit cu = gameObjectUnit.GetComponentInChildren<ClickableUnit>();
        currentUnit.cu = cu;
        cu.map = map;
        currentUnit.hex = hex;
        currentUnit.SetClickableHex(hex);
        cu.map = map;
        cu.x = x;
        cu.y = y;
        cu.halo = gameObjectUnit.GetComponent("Halo");
    }
    public void changeUnit(int i)
    {
        SpriteRenderer sr = currentUnit.GO.GetComponent<SpriteRenderer>();
        sr.sprite = spriteUnit[i];
        ClickableUnit cu = currentUnit.cu;
        cu.type = i;
        if (i < 2)
        {
            owner = 0;
            currentUnit.owner = 0;
        }
        else
        {
            owner = 1;
            currentUnit.owner = 1;
        }

        currentUnit.maxMovement = 1;
        currentUnit.turnMovement = 1;
    }

    public void createBuilding(Building newBuilding)
    {

        gameObjectBuilding = (GameObject)Instantiate(buildingPrefab, transform.position, Quaternion.identity, parent.transform);
        SpriteRenderer srBuilding = gameObjectBuilding.GetComponentInChildren<SpriteRenderer>();
        srBuilding.sprite = buildingSprites[newBuilding.owner];
        owner = newBuilding.owner;
        if (owner == 0)
        {
            Camera.main.transform.position = new Vector3(gameObjectBuilding.transform.position.x, gameObjectBuilding.transform.position.y, Camera.main.transform.position.z);
        }
        srBuilding.sortingOrder = 1;

        currentBuilding = newBuilding;
        currentBuilding.GO = gameObjectBuilding;
        ClickableBuilding cb = gameObjectBuilding.GetComponentInChildren<ClickableBuilding>();
        currentBuilding.cb = cb;
        cb.map = map;
        currentBuilding.hex = hex;
        currentBuilding.SetClickableHex(hex);
        cb.x = x;
        cb.y = y;
    }

    internal void RemoveUnit()
    {
        currentUnit = null;
        owner = -1;
    }

    internal void RemoveBuilding()
    {
        throw new NotImplementedException();
    }
}

