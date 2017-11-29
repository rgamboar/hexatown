using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableHex : MonoBehaviour {

    public int tileX, tileY;
    public GameObject parent;
    public HexMap map;
    public List<Node> path;
    public GameObject gameObjectHex;
    public GameObject gameObjectUnit;
    public GameObject gameObjectBuilding;
    public Sprite[] spriteHex;
    public Sprite[] spriteUnit;
    public Color[] spriteColor;
    public GameObject unitPrefab;
    public Component halo;
    public GameObject buildingPrefab;
    public Sprite[] buildingSprites;

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
            if (map.SelectedUnit != gameObjectUnit && gameObjectUnit != null)
            {
                if (map.SelectedUnit != null)
                {
                    map.SelectedUnit.GetComponent<ClickableUnit>().changeHalo(false);
                }
                map.SelectedUnit = gameObjectUnit;
                map.SUcu = gameObjectUnit.GetComponent<ClickableUnit>();
                changeHalo(true);
            }
            
            //map.MoveSelectedUnit(tileX, tileY);
            map.MoveUnit(tileX, tileY);
            if (gameObjectBuilding != null)
            {
                map.SpawnUnit(tileX, tileY, 1);
            }

        }
    }
    private void Update()
    {
     //   if (path != null)
     //   {
     //       int currentNode = 0;
     //       while (currentNode <= path.Count - 2)
     //       {
     //           Debug.DrawLine(Line(path[currentNode]), Line(path[currentNode + 1]), Color.red);
     //           currentNode++;
     //       }
     //   }
    }

    public Vector3 Line(Node current)
    {
        return new Vector3(2 * (current.x + current.y / 2f), Mathf.Sqrt(3) * current.y, 10);
    }

    public void changeHalo(bool option)
    {
        gameObjectUnit.GetComponent<ClickableUnit>().changeHalo(option);
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
    public void createUnit()
    {
        gameObjectUnit = (GameObject)Instantiate(unitPrefab, transform.position, Quaternion.identity, parent.transform);
        ClickableUnit cu = gameObjectUnit.GetComponent<ClickableUnit>();
        cu.map = map;
        cu.x = tileX;
        cu.y = tileY;
        cu.halo = gameObjectUnit.GetComponent("Halo");
    }
    public void changeUnit(int i)
    {
        SpriteRenderer sr = gameObjectUnit.GetComponent<SpriteRenderer>();
        sr.sprite = spriteUnit[i];
        ClickableUnit cu = gameObjectUnit.GetComponent<ClickableUnit>();
        cu.maxMovement = 5;
        cu.turnMovement = 5;
    }

    internal void createBuilding(int type, Hex h)
    {
        gameObjectBuilding = (GameObject)Instantiate(buildingPrefab, h.Position(), Quaternion.identity, parent.transform);
        SpriteRenderer srBuilding = gameObjectBuilding.GetComponentInChildren<SpriteRenderer>();
        srBuilding.sprite = buildingSprites[type];
        if (type == 1)
        {
            Camera.main.transform.position = new Vector3(gameObjectBuilding.transform.position.x, gameObjectBuilding.transform.position.y, Camera.main.transform.position.z);
        }
        srBuilding.sortingOrder = 1;

    }
}

