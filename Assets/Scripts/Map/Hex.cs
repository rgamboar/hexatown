using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex {

    public int size = 4;
    public GameObject GO;
    public ClickableHex ch;

    public Hex(int q, int r)
    {
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
        this.x = Q;
        this.y = R;
    }

    public readonly int Q;
    public readonly int R;
    public readonly int S;
    public readonly int x;
    public readonly int y;
    static readonly float ROOT = Mathf.Sqrt(3);

    public float Elevation = -0.5f;

    public Vector3 Position()
    {
        return new Vector3(2 * (this.Q + this.R / 2f) * size, ROOT * this.R * size, 0);
    }
    public static float Distance(Hex a, Hex b)
    {
        return ((Mathf.Abs(a.R - b.R) + Mathf.Abs(a.S - b.S) + Mathf.Abs(a.Q - b.Q)) / 2);
    }


    public void RemoveUnit()
    {
        ch.owner = -1;
        ch.map.controlMap.nodes[x, y].costOfMovement = ch.map.controlMap.nodes[x, y].normalCostOfMovement;
        ch.currentUnit = null;
    }

}
