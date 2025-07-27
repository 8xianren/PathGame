using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGetInfo : MonoBehaviour
{
    public int width = 16;

    public int height = 16;
    
    public Animator aiAnimator;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
