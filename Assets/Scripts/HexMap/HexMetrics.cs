using System.Collections;
using System.Collections.Generic;
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
		int iZ = Mathf.RoundToInt(-x -y);

        if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

        return new HexCoordinates(iX, iZ);
    }
}