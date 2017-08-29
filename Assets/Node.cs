using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    private List<Node> neightbours;
    public int costOfMovement = -1;
    public int x;
    public int y;

    public List<Node> Neightbours
    {
        get
        {
            return neightbours;
        }

        set
        {
            neightbours = value;
        }
    }

    public List<Node> NeightboursLand
    {
        get
        {
            return neightbours.Where(n => n.costOfMovement > -1).ToList();
        }

    }

    public Node()
    {
        Neightbours = new List<Node>();
    }

    public override string ToString()
    {
        return ("(X,Y)= " + x + " " + y);
    }


}
