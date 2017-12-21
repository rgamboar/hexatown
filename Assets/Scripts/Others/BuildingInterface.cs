using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildingInterface : MonoBehaviour {



    public GameObject leftInterface;
    public bool leftVisibility;
    public HexMap map;
    public int x, y;

    public float tools = 2;
    public float residents = 2;
    public int maxunitTurn = 2;
    public int unitTurn = 2;

    public Text toolsText;
    public Text residentsText;


    public void OpenLeftInterface(int newX, int newY)
    {
        x = newX;
        y = newY;
        leftVisibility = true;
        leftInterface.SetActive(leftVisibility);
    }
    public void CloseLeftInterface()
    {
        if (leftVisibility)
        {
            leftVisibility = !leftVisibility;
            leftInterface.SetActive(leftVisibility);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            CloseLeftInterface();
        }
    }

    public void UnitChoice(int option)
    {
        int t, r;
        if (option == 0)
        {
            t = 2;
            r = 2;
        }
        else
        {
            t = 4;
            r = 3;
        }
        CreateUnit(option, t, r);
    }

    public void CreateUnit(int option, int t, int r)
    {
        if (Spend(t,r))
        {
            map.SpawnUnit(x, y, option);
        }
        else print("We need currency");
    }

    public void Produce(int turn)
    {

        if (residents > 10 && residents> turn/3)
        {
            Change(1 + (turn / 15), 2);
        }
        Change(1 + (turn/20), 1);   
    }

    public bool Spend(int t, int r)
    {
        if (t <= tools && r <= residents)
        {
            Change(-t, -r);
            return true;
        }
        return false;
    }

    public void Change(float t, float r)
    {
        tools += t;
        residents += r;

        int t2 = (int)tools;
        int r2 = (int)residents;

        toolsText.text = "Tools: " + t2;
        residentsText.text = "Residents: " + r2;
    }

}
