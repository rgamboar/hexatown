using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableUnit : MonoBehaviour {

    public HexMap map;
    public GameObject parent;
    float t0;
    public Component halo;
    public int x;
    public int y;
    public List<Node> path;

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
        return new Vector3(2 * (current.x + current.y / 2f), Mathf.Sqrt(3) * current.y, 10);
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
                map.SelectedUnit.GetComponentInChildren<ClickableUnit>().changeHalo(false);
            }
            changeHalo(true);
            map.SelectedUnit = parent;
            Debug.Log("Clicked a unit");
        }   
    }

    public void changeHalo(bool option)
    {
        halo.GetType().GetProperty("enabled").SetValue(halo, option, null);
    }

}
