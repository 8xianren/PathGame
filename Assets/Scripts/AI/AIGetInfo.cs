using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGetInfo : MonoBehaviour
{
    public int width = 16;

    public int height = 16;
    
    

    public HexCoordinates GetAxisCoordinates(Vector3 nowPosition)
    {
        // Convert the position to hex coordinates

        Vector3 playerPosition = transform.InverseTransformPoint(nowPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);
        return hexCoords;
    }

    public int GetCellIndex(Vector3 nowPosition)
    {
        // Convert the position to hex coordinates
        Vector3 playerPosition = transform.InverseTransformPoint(nowPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);

        int ox = hexCoords.X + hexCoords.Z / 2;
        int oz = hexCoords.Z;
        int index = oz * width + ox;
        
        return index;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject Grid = GameObject.Find("HexGrid");
        if (Grid != null)
        {
            HexGrid hexGrid = Grid.GetComponent<HexGrid>();
            if (hexGrid != null)
            {
                width = hexGrid.width;
                height = hexGrid.height;
            }
        }
    }

    public List<List<HexMetrics.HexOwner>> GetCellOwner()
    {
        

        return HexMetrics.cellOwners;
    }
    
    public float GetDistanceToCell(Vector2Int src, Vector2Int dest)
    {
        Vector3 srcPos = GetCellPosition(src);
        Vector3 destPos = GetCellPosition(dest);

        // Get the index of the target cell
        

        // Calculate and return the distance
        return Vector3.Distance(srcPos, destPos);
    }
    
    public Vector3 GetCellPosition(Vector2Int coords)
    {
        int x = coords.x;
        int z = coords.y;

        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        return position;
    }




    // Update is called once per frame
    void Update()
    {

    }
}
