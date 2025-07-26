using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;
    public int X
    {
        get
        {
            return x;
        }

    }
    public int Z
    {
        get
        {
            return z;
        }
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public override string ToString()
    {
        return "(X: " + X.ToString() + ", " + Y.ToString() + ", Z: " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    /*
        public void CalculateCellCoordinates(Vector3 position, ref HexCoordinates res)
        {
            res = HexCoordinates.
        }
    */
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
}
