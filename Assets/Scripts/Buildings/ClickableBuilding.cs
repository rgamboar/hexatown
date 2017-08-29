using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableBuilding : MonoBehaviour {

    public HexMap map;
    float t0;

    private void Start()
    {
        t0 = 0f;
    }
    void OnMouseDown()
    {
        t0 = Time.time;
    }
    void OnMouseUp()
    {
        if ((Time.time - t0) < 0.2f)
        {
            Debug.Log("Clicked a building");
        }
    }
}
