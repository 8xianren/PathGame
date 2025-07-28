using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public static class HexMetrics
{

    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public const int width = 16;
    public const int height = 16;


    public static List<List<HexOwner>> cellOwners = new List<List<HexOwner>>();

    public enum HexOwner
    {
        origin,
        Player,
        AI0,

        AI1,
        AI2

    }



    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (innerRadius * 2f);
        float y = -x;
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }


    public static List<Vector2Int> GetLinkedCells(Vector3 pos)
    {
        HexCoordinates o = FromPosition(pos);
        int x = o.X;
        int y = o.Y;
        int z = o.Z;



        List<Vector3Int> temp3 = new List<Vector3Int>
        {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(2, -2, 0),
            new Vector3Int(2, -1, -1),
            new Vector3Int(1, -2, 1),
            new Vector3Int(2, 0, -2),
            new Vector3Int(1, 1, -2),
            new Vector3Int(0, 2, -2),
            new Vector3Int(-2, 2, 0),
            new Vector3Int(-1, 2, -1),
            new Vector3Int(-2, 1, 1),
            new Vector3Int(0, -2, 2),
            new Vector3Int(-2, 0, 2),
            new Vector3Int(-1, -1, 2)
        };

        for (int i = 0; i < temp3.Count; ++i)
        {
            temp3[i] = new Vector3Int(temp3[i].x + x, temp3[i].y + y, temp3[i].z + z);

        }

        List<Vector2Int> res = new List<Vector2Int>();
        

        for (int i = 0; i < temp3.Count; ++i)
        {
            res.Add(TransformAxisToXZ(temp3[i])) ;

        }
        //Debug.Log(res.Count);


        

        return res;



    }


    public  static Vector2Int TransformAxisToXZ(Vector3Int hex)
    {
        Vector2Int res = new Vector2Int();
        res.x = hex.x + hex.z / 2;

        res.y = hex.z;

        return res;
        
    }
}