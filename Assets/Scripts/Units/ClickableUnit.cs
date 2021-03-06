﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableUnit : MonoBehaviour {

    public HexMap map;
    public GameObject parent;
    public Unit unit;
    float t0;
    public Component halo;
    public int x;
    public int y;
    public List<Node> path;
    public int type;
    public TextMesh HP;
    public TextMesh Damage;
    public TextMesh Movement;




    private void Start()
    {
        t0 = 0f;
        halo = GetComponent("Halo");
        path = null;
    }

    private void Update()
    {
        if (path != null)
        {
            int currentNode = 0;
            while (currentNode <= path.Count - 2)
            {   
                Debug.DrawLine(Line(path[currentNode]), Line(path[currentNode+1]), Color.red);
                currentNode++;
            }
        }
    }

    public Vector3 Line(Node current)
    {
        return new Vector3(2 * (current.x + current.y / 2f)* 4, Mathf.Sqrt(3) * current.y * 4, 10);
    }
    void OnMouseDown()
    {
        t0 = Time.time;
    }
    void OnMouseUp()
    {
        if ((Time.time - t0) < 0.2f)
        { 
            if (map.SelectedUnit != null)
            {
                map.SelectedUnit.cu.changeHalo(false);
            }
            changeHalo(true);
            map.SelectedUnit = unit;
            Debug.Log("Clicked a unit");
        }   
    }

    public void changeHalo(bool option)
    {
        halo.GetType().GetProperty("enabled").SetValue(halo, option, null);
    }

}
