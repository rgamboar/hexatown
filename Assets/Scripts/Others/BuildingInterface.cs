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

    public int tools = 5;
    public int residents = 3;
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
            t = 1;
            r = 1;
        }
        else
        {
            t = 2;
            r = 2;
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
        Change(((turn / 2) + residents ),(1 + (turn / 4)));
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

    private void Change(int t, int r)
    {
        tools += t;
        residents += r;

        toolsText.text = "Tools: " + tools;
        residentsText.text = "Residents: " + residents;
    }
}
